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
using System.Net;

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

        private IEnumerable<PictureparkListItem> _contentProviderCache;
        private IEnumerable<PictureparkListItem> _contentCategoryCache;

        private IEnumerable<PictureparkListItem> _licenseTypeCache;
        private IEnumerable<PictureparkListItem> _releaseStateCache;

        private IEnumerable<PictureparkListItem> _licenseUsageCache;
        private IEnumerable<PictureparkListItem> _licenseSizeCache;
        private IEnumerable<PictureparkListItem> _licensePlacementCache;
        private IEnumerable<PictureparkListItem> _licenseDistributionCache;
        private IEnumerable<PictureparkListItem> _licenseGeographyCache;
        private IEnumerable<PictureparkListItem> _licenseVerticalCache;

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

                        if (ex is ApiException apiEx)
                        {
                            if (apiEx.StatusCode == (int)HttpStatusCode.TooManyRequests)
                            {
                                // too many requests, backoff and try again

                                return;
                            }

                            // expected error happened server side, most likely our problem, cancel

                            throw ex;
                        }

                        // some server side or communication issue, backoff and try again
                    });
        }

        public void ClearCache()
        {
            _contentProviderCache = null;
            _contentCategoryCache = null;

            _licenseTypeCache = null;
            _releaseStateCache = null;

            _licenseUsageCache = null;
            _licenseSizeCache = null;
            _licensePlacementCache = null;
            _licenseDistributionCache = null;
            _licenseGeographyCache = null;
            _licenseVerticalCache = null;
        }

        public async Task<IEnumerable<PictureparkListItem>> GetContentProvidersAsync()
        {
            if (_contentProviderCache != null)
                return _contentProviderCache;

            _contentProviderCache = await GetListItemsAsync(nameof(ContentProvider));

            return _contentProviderCache;
        }
        
        public async Task ImportContentProvidersAsync(IEnumerable<PictureparkListItem> contentProviders)
        {
            _logger.LogInformation("Importing content providers to Picturepark...");

            await ImportListItemsAsync(nameof(ContentProvider), contentProviders);

            _logger.LogInformation($"Imported {contentProviders.Count()} content providers to Picturepark");
        }

        public async Task<IEnumerable<PictureparkListItem>> GetContentCategoriesAsync()
        {
            if (_contentCategoryCache != null)
                return _contentCategoryCache;

            _contentCategoryCache = await GetListItemsAsync(nameof(ContentCategory));

            return _contentCategoryCache;
        }

        public async Task ImportContentCategoriesAsync(IEnumerable<PictureparkListItem> contentCategories)
        {
            _logger.LogInformation("Importing content categories to Picturepark...");

            await ImportListItemsAsync(nameof(ContentCategory), contentCategories);

            _logger.LogInformation($"Imported {contentCategories.Count()} content categories to Picturepark");
        }

        public async Task<IEnumerable<PictureparkListItem>> GetLicenseTypesAsync()
        {
            if (_licenseTypeCache != null)
                return _licenseTypeCache;

            _licenseTypeCache = await GetListItemsAsync(nameof(LicenseType));

            return _licenseTypeCache;
        }

        public async Task ImportLicenseTypesAsync(IEnumerable<PictureparkListItem> licenseTypes)
        {
            _logger.LogInformation("Importing license types to Picturepark...");

            await ImportListItemsAsync(nameof(LicenseType), licenseTypes);

            _logger.LogInformation($"Imported {licenseTypes.Count()} license types to Picturepark");
        }

        public async Task<IEnumerable<PictureparkListItem>> GetReleaseStatesAsync()
        {
            if (_releaseStateCache != null)
                return _releaseStateCache;

            _releaseStateCache = await GetListItemsAsync(nameof(ReleaseState));

            return _releaseStateCache;
        }

        public async Task ImportReleaseStatesAsync(IEnumerable<PictureparkListItem> releaseStates)
        {
            _logger.LogInformation("Importing release states to Picturepark...");

            await ImportListItemsAsync(nameof(ReleaseState), releaseStates);

            _logger.LogInformation($"Imported {releaseStates.Count()} release states to Picturepark");
        }

        public async Task<IEnumerable<PictureparkListItem>> GetLicenseUsagesAsync()
        {
            if (_licenseUsageCache != null)
                return _licenseUsageCache;

            _licenseUsageCache = await GetListItemsAsync(nameof(LicenseUsage));

            return _licenseUsageCache;
        }

        public async Task ImportLicenseUsagesAsync(IEnumerable<PictureparkListItem> licenseUsages)
        {
            _logger.LogInformation("Importing license usages to Picturepark...");

            await ImportListItemsAsync(nameof(LicenseUsage), licenseUsages);

            _logger.LogInformation($"Imported {licenseUsages.Count()} license usages to Picturepark");
        }

        public async Task<IEnumerable<PictureparkListItem>> GetLicenseSizesAsync()
        {
            if (_licenseSizeCache != null)
                return _licenseSizeCache;

            _licenseSizeCache = await GetListItemsAsync(nameof(LicenseSize));

            return _licenseSizeCache;
        }

        public async Task ImportLicenseSizesAsync(IEnumerable<PictureparkListItem> licenseSizes)
        {
            _logger.LogInformation("Importing license sizes to Picturepark...");

            await ImportListItemsAsync(nameof(LicenseSize), licenseSizes);

            _logger.LogInformation($"Imported {licenseSizes.Count()} license sizes to Picturepark");
        }

        public async Task<IEnumerable<PictureparkListItem>> GetLicensePlacementsAsync()
        {
            if (_licensePlacementCache != null)
                return _licensePlacementCache;

            _licensePlacementCache = await GetListItemsAsync(nameof(LicensePlacement));

            return _licensePlacementCache;
        }

        public async Task ImportLicensePlacementsAsync(IEnumerable<PictureparkListItem> licensePlacements)
        {
            _logger.LogInformation("Importing license placements to Picturepark...");

            await ImportListItemsAsync(nameof(LicensePlacement), licensePlacements);

            _logger.LogInformation($"Imported {licensePlacements.Count()} license placements to Picturepark");
        }

        public async Task<IEnumerable<PictureparkListItem>> GetLicenseDistributionsAsync()
        {
            if (_licenseDistributionCache != null)
                return _licenseDistributionCache;

            _licenseDistributionCache = await GetListItemsAsync(nameof(LicenseDistribution));

            return _licenseDistributionCache;
        }

        public async Task ImportLicenseDistributionsAsync(IEnumerable<PictureparkListItem> licenseDistributions)
        {
            _logger.LogInformation("Importing license distributions to Picturepark...");

            await ImportListItemsAsync(nameof(LicenseDistribution), licenseDistributions);

            _logger.LogInformation($"Imported {licenseDistributions.Count()} license distributions to Picturepark");
        }

        public async Task<IEnumerable<PictureparkListItem>> GetLicenseGeographiesAsync()
        {
            if (_licenseGeographyCache != null)
                return _licenseGeographyCache;

            _licenseGeographyCache = await GetListItemsAsync(nameof(LicenseGeography));

            return _licenseGeographyCache;
        }

        public async Task ImportLicenseGeographiesAsync(IEnumerable<PictureparkListItem> licenseGeographies)
        {
            _logger.LogInformation("Importing license geographies to Picturepark...");

            await ImportListItemsAsync(nameof(LicenseGeography), licenseGeographies);

            _logger.LogInformation($"Imported {licenseGeographies.Count()} license geographies to Picturepark");
        }

        public async Task<IEnumerable<PictureparkListItem>> GetLicenseVerticalsAsync()
        {
            if (_licenseVerticalCache != null)
                return _licenseVerticalCache;

            _licenseVerticalCache = await GetListItemsAsync(nameof(LicenseVertical));

            return _licenseVerticalCache;
        }

        public async Task ImportLicenseVerticalsAsync(IEnumerable<PictureparkListItem> licenseVerticals)
        {
            _logger.LogInformation("Importing license verticals to Picturepark...");

            await ImportListItemsAsync(nameof(LicenseVertical), licenseVerticals);

            _logger.LogInformation($"Imported {licenseVerticals.Count()} license verticals to Picturepark");
        }

        private async Task<IEnumerable<PictureparkListItem>> GetListItemsAsync(string schemaId)
        {
            try
            {
                var schemaIds = new[] { schemaId };

                var searchResult = await _client.ListItem.SearchAsync(new ListItemSearchRequest()
                {
                    SchemaIds = schemaIds,
                    IncludeContentData = true
                });

                return searchResult.Results.Select(searchResultInner =>
                {
                    var dataDictionary = searchResultInner.ConvertTo<DataDictionary>();

                    return new PictureparkListItem()
                    {
                        PictureparkListItemId = searchResultInner.Id,
                        SmintIoKey = (string)dataDictionary["key"],
                        Content = dataDictionary      
                    };
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error getting list items");

                throw;
            }
        }

        private async Task ImportListItemsAsync(string schemaId, IEnumerable<PictureparkListItem> listItems)
        {
            try
            {
                var existingListItems = await GetListItemsAsync(schemaId);

                foreach (var listItem in listItems)
                {                    
                    var existingListItem = existingListItems.FirstOrDefault(existingListItemInner => 
                        string.Equals(existingListItemInner.SmintIoKey, listItem.SmintIoKey));

                    if (existingListItem != null)
                    {
                        var listItemUpdateRequest = new ListItemUpdateRequest()
                        {
                            Content = listItem.Content
                        };

                        await _client.ListItem.UpdateAsync(existingListItem.PictureparkListItemId, listItemUpdateRequest);
                    }
                    else
                    {
                        var listItemCreateRequest = new ListItemCreateRequest()
                        {
                            Content = listItem.Content,
                            ContentSchemaId = schemaId
                        };

                        await _client.ListItem.CreateAsync(listItemCreateRequest);
                    }
                }

                var noMoreExistingListItems = existingListItems.Where(existingListItemInner =>
                    !listItems.Any(listItem => string.Equals(existingListItemInner.SmintIoKey, listItem.SmintIoKey)));

                foreach (var noMoreExistingListItem in noMoreExistingListItems)
                {
                    await _client.ListItem.DeleteAsync(noMoreExistingListItem.PictureparkListItemId);
                }

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error importing list items");

                throw;
            }
        }

        public async Task ImportAssetsAsync(IEnumerable<PictureparkAsset> assets)
        {
            _logger.LogInformation("Importing assets to Picturepark...");

            var assetsByTransfer = assets.GroupBy(asset => asset.TransferId).ToDictionary(group => group.Key, group => group.ToList());

            foreach (var (key, value) in assetsByTransfer)
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await ImportByTransferAsync(key, value);
                });
            }

            _logger.LogInformation($"Imported {assets.Count()} assets to Picturepark");
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

                var transferResult = await CreateFileTransferAsync(transferIdentifier, assets);

                var files = await _client.Transfer.SearchFilesByTransferIdAsync(transferResult.Transfer.Id);

                foreach (FileTransfer file in files.Results)
                {
                    var asset = assets.FirstOrDefault(a => a.DownloadUrl.Contains(file.Name));

                    fileTransfers.Add(new FileTransferCreateItem
                    {
                        FileId = file.Id,
                        LayerSchemaIds = new[] {
                            nameof(ContentLayer),
                            nameof(LicenseLayer)
                        },
                        Metadata = asset.Metadata
                    });
                }

                var partialRequest = new ImportTransferPartialRequest()
                {
                    Items = fileTransfers
                };

                var importResult = await _client.Transfer.PartialImportAsync(transferResult.Transfer.Id, partialRequest);

                await _client.BusinessProcess.WaitForCompletionAsync(importResult.BusinessProcessId);

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

        private async Task<TransferSearchResult> FindTransferByIdentifierAsync(string transferIdentifier)
        {
            var searchRequest = new TransferSearchRequest()
            {
                SearchString = transferIdentifier
            };

            return await _client.Transfer.SearchAsync(searchRequest);
        }

        public async Task InitSchemasAsync()
        {
            bool updateSchemaOnStart = _options.UpdateSchemaOnStart;

            if (!updateSchemaOnStart)
            {
                _logger.LogInformation("Updating the Picturepark schema on start has been turned off");

                return;
            }

            _logger.LogInformation("Initializing Picturpark schemas...");

            _logger.LogInformation("Initializing Picturepark content layer schema...");

            await InitSchemaAsync(typeof(ContentLayer));

            _logger.LogInformation("Initialized Picturepark content layer schema");

            _logger.LogInformation("Initializing Picturepark license layer schema...");

            await InitSchemaAsync(typeof(LicenseLayer));

            _logger.LogInformation("Initialized Picturepark license layer schema");

            _logger.LogInformation("Initialized Picturepark schemas");
        }

        private async Task InitSchemaAsync(Type type)
        {
            var schemas = await _client.Schema.GenerateSchemasAsync(type);

            var schemasToCreate = new List<SchemaDetail>();
            var schemasToUpdate = new List<SchemaDetail>();

            foreach (var schema in schemas)
            {
                if (!await _client.Schema.ExistsAsync(schema.Id))
                {
                    schemasToCreate.Add(schema);
                } 
                else
                {
                    schemasToUpdate.Add(schema);
                }
            }

            if (schemasToCreate.Any())
            {
                var result = await _client.Schema.CreateManyAsync(schemasToCreate, false);

                await _client.BusinessProcess.WaitForCompletionAsync(result.BusinessProcessId);

                foreach (var schema in schemasToCreate)
                {
                    if (schema.Id == nameof(ContentLayer))
                    {
                        await AddSchemaToFileTypes(nameof(ContentLayer));
                    }
                    else if (schema.Id == nameof(LicenseLayer))
                    {
                        await AddSchemaToFileTypes(nameof(LicenseLayer));
                    }
                }
            }

            if (schemasToUpdate.Any())
            {
                foreach (var schema in schemasToUpdate)
                {
                    await _client.Schema.UpdateAsync(schema, false);

                    if (schema.Id == nameof(ContentLayer))
                    {
                        await AddSchemaToFileTypes(nameof(ContentLayer));
                    }
                    else if (schema.Id == nameof(LicenseLayer))
                    {
                        await AddSchemaToFileTypes(nameof(LicenseLayer));
                    }
                }
            }
        }

        private async Task AddSchemaToFileTypes(string schemaName)
        {
            foreach (var type in _options.PictureparkFileTypes)
            {
                var typeData = await _client.Schema.GetAsync(type);

                if (typeData.LayerSchemaIds == null)
                {
                    typeData.LayerSchemaIds = new List<string>();
                }

                typeData.LayerSchemaIds.Add(schemaName);

                await _client.Schema.UpdateAsync(typeData, false);
            }
        }

        /*
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
            return await _client.Transfer.CreateAndWaitForCompletionAsync(request);
        }
        */

        private async Task<CreateTransferResult> CreateFileTransferAsync(string transferIdentifier, IEnumerable<PictureparkAsset> assets)
        {
            var filePaths = assets.Select(asset => new FileLocations(asset.DownloadUrl)).ToList();

            var request = new CreateTransferRequest
            {
                Name = transferIdentifier,
                TransferType = TransferType.FileUpload,
                Files = assets.Select(asset => new TransferUploadFile() {
                        FileName = asset.RecommendedFileName
                    }).ToList()
            };

            var uploadOptions = new UploadOptions
            {
                ChunkSize = 1024 * 1024,
                ConcurrentUploads = 4,
                SuccessDelegate = Console.WriteLine,
                ErrorDelegate = Console.WriteLine,
                WaitForTransferCompletion = true
            };

            return await _client.Transfer.UploadFilesAsync(transferIdentifier, filePaths, uploadOptions);
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
