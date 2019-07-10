using Client.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Picturepark.SDK.V1;
using Picturepark.SDK.V1.Authentication;
using Picturepark.SDK.V1.Contract;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Client.Contracts;
using Client.Contracts.Picturepark;

namespace Client.Providers.Impl
{
    public sealed class PictureparkApiClientProviderImpl : IDisposable, IPictureparkApiClientProvider
    {
        private const int MaxRetryAttempts = 5;

        private readonly IAuthDataProvider _authDataProvider;

        private readonly PictureparkAppOptions _options;

        private readonly AsyncRetryPolicy _retryPolicy;

        private readonly HttpClient _httpClient;
        private PictureparkService _client;

        private bool _disposed;

        private readonly ILogger _logger;

        public PictureparkApiClientProviderImpl(
            IOptionsMonitor<PictureparkAppOptions> optionsAccessor,
            ILogger<PictureparkApiClientProviderImpl> logger,
            IAuthDataProvider authDataProvider)
        {
            _options = optionsAccessor.CurrentValue;
            _authDataProvider = authDataProvider;
            _disposed = false;

            _httpClient = new HttpClient(new PictureparkRetryHandler()) {
                Timeout = Timeout.InfiniteTimeSpan
            };

            _retryPolicy = GetRetryStrategy();

            _logger = logger;

            InitPictureparkService();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private AsyncRetryPolicy GetRetryStrategy()
        {
            return Policy
                .Handle<ApiException>()
                .Or<Exception>()
                .WaitAndRetryAsync(
                    MaxRetryAttempts,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, timespan, context) =>
                    {
                        _logger.LogError(ex, "Error communicating to Picturepark");
                    });
        }

        public async Task ImportAssetsAsync(IEnumerable<PictureparkAsset> assets)
        {
            _logger.LogInformation("Importing assets to Picturepark...");

            var assetsPerTransfer = assets.GroupBy(asset => asset.TransferId).ToDictionary(group => group.Key, group => group.ToList());

            foreach (var (key, value) in assetsPerTransfer)
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await ImportByTransferAsync(key, value);
                });
            }

            _logger.LogInformation($"Imported {assetsPerTransfer.Count} assets to Picturepark");
        }

        #region PictureparkHelpers
        private async Task ImportByTransferAsync(string transferIdentifier, IEnumerable<PictureparkAsset> assets)
        {
            try
            {
                if (await TransferExistsAsync(transferIdentifier))
                {
                    // TODO handle update of metadata if transfer already exists

                    _logger.LogInformation($"Transfer {transferIdentifier} already exists. Skipping import");

                    return;
                }

                _logger.LogInformation($"Starting import of transfer {transferIdentifier}...");

                var fileTransfers = new List<FileTransferCreateItem>();

                var transferResult = await CreateFileTransferAsync(transferIdentifier, assets).ConfigureAwait(false);

                var files = await _client.Transfer.SearchFilesByTransferIdAsync(transferResult.Transfer.Id).ConfigureAwait(false);

                foreach (FileTransfer file in files.Results)
                {
                    var asset = assets.FirstOrDefault(a => a.DownloadUrl.Contains(file.Name));

                    fileTransfers.Add(new FileTransferCreateItem
                    {
                        FileId = file.Id,
                        LayerSchemaIds = new[] { nameof(SmintIoContentLayer), nameof(SmintIoLicenseLayer) },
                        Metadata = asset.Metadata
                    });
                }

                var partialRequest = new ImportTransferPartialRequest()
                {
                    Items = fileTransfers
                };

                var importResult = await _client.Transfer.PartialImportAsync(transferResult.Transfer.Id, partialRequest).ConfigureAwait(false);

                await _client.BusinessProcess.WaitForCompletionAsync(importResult.BusinessProcessId).ConfigureAwait(false);

                _logger.LogInformation($"Finished import of transfer {transferIdentifier}");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error importing files for transfer {transferIdentifier}");

                await TryDeleteTransferAsync(transferIdentifier);

                throw;
            }
        }

        private async Task TryDeleteTransferAsync(string transferIdentifier)
        {
            var result = await FindTransferByIdentifierAsync(transferIdentifier);

            if (result.Results.Count == 0)
            {
                return;
            }

            Transfer transfer = result.Results.First();

            try
            {
                await _client.Transfer.DeleteAsync(transfer.Id);

                _logger.LogInformation($"Deleted transfer {transferIdentifier}");

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting transfer {transferIdentifier}");
                return;
            }
        }

        private async Task<bool> TransferExistsAsync(string transferIdentifier)
        {
            var result = await FindTransferByIdentifierAsync(transferIdentifier);

            return result.Results.Count > 0;
        }

        private async Task<TransferSearchResult> FindTransferByIdentifierAsync(string identifier)
        {
            var searchRequest = new TransferSearchRequest()
            {
                SearchString = identifier
            };

            return await _client.Transfer.SearchAsync(searchRequest);
        }

        public async Task InitSchemasAsync()
        {
            await InitSchemaAsync(typeof(SmintIoContentLayer));
            await InitSchemaAsync(typeof(SmintIoLicenseLayer));
        }

        private async Task InitSchemaAsync(Type type)
        {
            var schemas = await _client.Schema.GenerateSchemasAsync(type).ConfigureAwait(false);

            var schemasToCreate = new List<SchemaDetail>();

            foreach (var schema in schemas)
            {
                if (!await _client.Schema.ExistsAsync(schema.Id).ConfigureAwait(false))
                {
                    schemasToCreate.Add(schema);
                    
                    if(schema.Id == nameof(SmintIoContentLayer))
                    {
                        schema.Names = new TranslatedStringDictionary(_options.ContentLayerNames);
                    }

                    if (schema.Id == nameof(SmintIoLicenseLayer))
                    {
                        schema.Names = new TranslatedStringDictionary(_options.LicenseLayerNames);
                    }
                }
            }

            if (schemasToCreate.Any())
            {
                var result = await _client.Schema.CreateManyAsync(schemasToCreate, false).ConfigureAwait(false);

                await _client.BusinessProcess.WaitForCompletionAsync(result.BusinessProcessId);

                await AddSchemaToFileTypes(nameof(SmintIoContentLayer));
                await AddSchemaToFileTypes(nameof(SmintIoLicenseLayer));
            }
        }

        private async Task AddSchemaToFileTypes(string schemaName)
        {
            foreach (var type in _options.PictureparkFileTypes)
            {
                var typeData = await _client.Schema.GetAsync(type).ConfigureAwait(false);

                if (typeData.LayerSchemaIds == null)
                {
                    typeData.LayerSchemaIds = new List<string>();
                }

                typeData.LayerSchemaIds.Add(schemaName);

                await _client.Schema.UpdateAsync(typeData, false).ConfigureAwait(false);
            }
        }

        private async Task<CreateTransferResult> CreateWebTransferAsync(string transferIdentifier, IEnumerable<PictureparkAsset> assets)
        {
            var request = new CreateTransferRequest
            {
                Name = transferIdentifier,

                TransferType = TransferType.WebDownload,

                WebLinks = assets.Select(asset => new TransferWebLink
                {
                    Url = asset.DownloadUrl,
                    Identifier = asset.Id
                }).ToList()
            };
            return await _client.Transfer.CreateAndWaitForCompletionAsync(request).ConfigureAwait(false);
        }

        private async Task<CreateTransferResult> CreateFileTransferAsync(string transferIdentifier, IEnumerable<PictureparkAsset> assets)
        {
            var filePaths = assets.Select(asset => new FileLocations(asset.DownloadUrl)).ToList();

            var request = new CreateTransferRequest
            {
                Name = transferIdentifier,
                TransferType = TransferType.FileUpload,
                Files = assets.Select(asset => new TransferUploadFile() { FileName = asset.Id }).ToList()
            };

            var uploadOptions = new UploadOptions
            {
                ChunkSize = 1024 * 1024,
                ConcurrentUploads = 4,
                SuccessDelegate = Console.WriteLine,
                ErrorDelegate = Console.WriteLine
            };

            return await _client.Transfer.UploadFilesAsync(transferIdentifier, filePaths, uploadOptions).ConfigureAwait(false);
        }
        #endregion

        private void InitPictureparkService()
        {
            string accessToken = _authDataProvider.Picturepark.AccessToken;

            var authClient = new AccessTokenAuthClient(_options.ApiBaseUrl, accessToken, _options.CustomerAlias);

            var settings = new PictureparkServiceSettings(authClient);

            _client = new PictureparkService(settings, _httpClient);            
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _client?.Dispose();
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
