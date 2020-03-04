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

        private readonly PictureparkAppOptions _appOptions;
        private readonly PictureparkAuthOptions _authOptions;

        private readonly AsyncRetryPolicy _retryPolicy;

        private readonly HttpClient _httpClient;
        private PictureparkService _client;

        private bool _disposed;

        private readonly ILogger _logger;

        public PictureparkApiClientProviderImpl(
            IOptionsMonitor<PictureparkAppOptions> appOptionsAccessor,
            IOptionsMonitor<PictureparkAuthOptions> authOptionsAccessor,
            ILogger<PictureparkApiClientProviderImpl> logger)
        {
            _appOptions = appOptionsAccessor.CurrentValue;
            _authOptions = authOptionsAccessor.CurrentValue;

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

        public async Task<IList<PictureparkListItem>> GetContentProvidersAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoContentProvider));
        }

        public async Task<IList<PictureparkListItem>> GetContentTypesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoContentType));
        }

        public async Task<IList<PictureparkListItem>> GetBinaryTypesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoBinaryType));
        }

        public async Task ImportContentProvidersAsync(IList<PictureparkListItem> contentProviders)
        {
            _logger.LogInformation("Importing content providers to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoContentProvider), contentProviders);

            _logger.LogInformation($"Imported {contentProviders.Count()} content providers to Picturepark");
        }

        public async Task ImportContentTypesAsync(IList<PictureparkListItem> contentTypes)
        {
            _logger.LogInformation("Importing content types to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoContentType), contentTypes);

            _logger.LogInformation($"Imported {contentTypes.Count()} content types to Picturepark");
        }

        public async Task ImportBinaryTypesAsync(IList<PictureparkListItem> binaryTypes)
        {
            _logger.LogInformation("Importing binary types to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoBinaryType), binaryTypes);

            _logger.LogInformation($"Imported {binaryTypes.Count()} binary types to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetContentCategoriesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoContentCategory));
        }

        public async Task ImportContentCategoriesAsync(IList<PictureparkListItem> contentCategories)
        {
            _logger.LogInformation("Importing content categories to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoContentCategory), contentCategories);

            _logger.LogInformation($"Imported {contentCategories.Count()} content categories to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetLicenseTypesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoLicenseType));
        }

        public async Task ImportLicenseTypesAsync(IList<PictureparkListItem> licenseTypes)
        {
            _logger.LogInformation("Importing license types to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoLicenseType), licenseTypes);

            _logger.LogInformation($"Imported {licenseTypes.Count()} license types to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetReleaseStatesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoReleaseState));
        }

        public async Task ImportReleaseStatesAsync(IList<PictureparkListItem> releaseStates)
        {
            _logger.LogInformation("Importing release states to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoReleaseState), releaseStates);

            _logger.LogInformation($"Imported {releaseStates.Count()} release states to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetLicenseExclusivitiesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoLicenseExclusivity));
        }

        public async Task ImportLicenseExclusivitiesAsync(IList<PictureparkListItem> licenseExclusivities)
        {
            _logger.LogInformation("Importing license exclusivities to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoLicenseExclusivity), licenseExclusivities);

            _logger.LogInformation($"Imported {licenseExclusivities.Count()} license exclusivities to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetLicenseUsagesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoLicenseUsage));
        }

        public async Task ImportLicenseUsagesAsync(IList<PictureparkListItem> licenseUsages)
        {
            _logger.LogInformation("Importing license usages to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoLicenseUsage), licenseUsages);

            _logger.LogInformation($"Imported {licenseUsages.Count()} license usages to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetLicenseSizesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoLicenseSize));
        }

        public async Task ImportLicenseSizesAsync(IList<PictureparkListItem> licenseSizes)
        {
            _logger.LogInformation("Importing license sizes to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoLicenseSize), licenseSizes);

            _logger.LogInformation($"Imported {licenseSizes.Count()} license sizes to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetLicensePlacementsAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoLicensePlacement));
        }

        public async Task ImportLicensePlacementsAsync(IList<PictureparkListItem> licensePlacements)
        {
            _logger.LogInformation("Importing license placements to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoLicensePlacement), licensePlacements);

            _logger.LogInformation($"Imported {licensePlacements.Count()} license placements to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetLicenseDistributionsAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoLicenseDistribution));
        }

        public async Task ImportLicenseDistributionsAsync(IList<PictureparkListItem> licenseDistributions)
        {
            _logger.LogInformation("Importing license distributions to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoLicenseDistribution), licenseDistributions);

            _logger.LogInformation($"Imported {licenseDistributions.Count()} license distributions to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetLicenseGeographiesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoLicenseGeography));
        }

        public async Task ImportLicenseGeographiesAsync(IList<PictureparkListItem> licenseGeographies)
        {
            _logger.LogInformation("Importing license geographies to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoLicenseGeography), licenseGeographies);

            _logger.LogInformation($"Imported {licenseGeographies.Count()} license geographies to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetLicenseIndustriesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoLicenseIndustry));
        }

        public async Task ImportLicenseIndustriesAsync(IList<PictureparkListItem> licenseIndustries)
        {
            _logger.LogInformation("Importing license industries to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoLicenseIndustry), licenseIndustries);

            _logger.LogInformation($"Imported {licenseIndustries.Count()} license Industries to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetLicenseLanguagesAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoLicenseLanguage));
        }

        public async Task ImportLicenseLanguagesAsync(IList<PictureparkListItem> licenseLanguages)
        {
            _logger.LogInformation("Importing license languages to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoLicenseLanguage), licenseLanguages);

            _logger.LogInformation($"Imported {licenseLanguages.Count()} license languages to Picturepark");
        }

        public async Task<IList<PictureparkListItem>> GetLicenseUsageLimitsAsync()
        {
            return await GetListItemsAsync(nameof(SmintIoLicenseUsageLimit));
        }

        public async Task ImportLicenseUsageLimitsAsync(IList<PictureparkListItem> licenseUsageLimits)
        {
            _logger.LogInformation("Importing license usage limits to Picturepark...");

            await ImportListItemsAsync(nameof(SmintIoLicenseUsageLimit), licenseUsageLimits);

            _logger.LogInformation($"Imported {licenseUsageLimits.Count()} license usage limits to Picturepark");
        }

        private async Task<IList<PictureparkListItem>> GetListItemsAsync(string schemaId)
        {
            try
            {
                var schemaIds = new[] { schemaId };

                // very likely we'll not have > 1000 items per key

                var searchResult = await _client.ListItem.SearchAsync(new ListItemSearchRequest()
                {
                    SchemaIds = schemaIds,
                    ResolveBehaviors = new[] { ListItemResolveBehavior.Content},
                    Limit = 1000
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
                }).ToList();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error getting list items");

                throw;
            }
        }

        private async Task ImportListItemsAsync(string schemaId, IList<PictureparkListItem> listItems)
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

                        listItem.SmintIoMetadataElement.TargetMetadataUuid = existingListItem.PictureparkListItemId;
                    }
                    else
                    {
                        var listItemCreateRequest = new ListItemCreateRequest()
                        {
                            Content = listItem.Content,
                            ContentSchemaId = schemaId
                        };

                        var listItemDetail = await _client.ListItem.CreateAsync(listItemCreateRequest);

                        listItem.SmintIoMetadataElement.TargetMetadataUuid = listItemDetail.Id;
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

        public async Task<string> GetExistingPictureparkCompoundAssetUuidAsync(string licensePurchaseTransactionUuid)
        {
            var filters = new List<FilterBase>
            {
                FilterBase.FromExpression<Content>(i => i.LayerSchemaIds, new string[] { nameof(SmintIoContentLayer) }),
                FilterBase.FromExpression<SmintIoContentLayer>(i => i.LicensePurchaseTransactionUuid, new string[] { licensePurchaseTransactionUuid })
            };

            filters.Add(FilterBase.FromExpression<Content>(i => i.ContentSchemaId, new string[] { nameof(SmintIoCompoundAsset) }));

            return await GetFilterResultAsync(filters);
        }

        public async Task<string> GetExistingPictureparkAssetBinaryUuidAsync(string licensePurchaseTransactionUuid, string binaryUuid)
        {
            var filters = new List<FilterBase>
            {
                FilterBase.FromExpression<Content>(i => i.LayerSchemaIds, new string[] { nameof(SmintIoContentLayer) }),
                FilterBase.FromExpression<SmintIoContentLayer>(i => i.LicensePurchaseTransactionUuid, new string[] { licensePurchaseTransactionUuid })
            };

            filters.Add(FilterBase.FromExpression<SmintIoContentLayer>(i => i.BinaryUuid, new string[] { binaryUuid }));

            return await GetFilterResultAsync(filters);
        }

        private async Task<string> GetFilterResultAsync(List<FilterBase> filters)
        {
            var contentSearchRequest = new ContentSearchRequest()
            {
                Filter = new AndFilter
                {
                    Filters = filters
                }
            };

            var searchResults = await _client.Content.SearchAsync(contentSearchRequest);

            var count = searchResults.Results.Count;

            if (count == 0)
                return null;

            if (count > 1)
                throw new Exception($"Unexpected number of Picturepark asset search results ({searchResults.Results.Count} instead of 0 or 1)");

            var searchResult = searchResults.Results.First();

            return searchResult.Id;
        }

        public async Task CreateAssetsAsync(IList<PictureparkAsset> newTargetAssets)
        {
            _logger.LogInformation("Creating assets in Picturepark...");

            var transferIdentifier = $"Smint.io Import {Guid.NewGuid().ToString()}";

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await CreateAssetsByTransferAsync(transferIdentifier, newTargetAssets);
            });

            _logger.LogInformation($"Created {newTargetAssets.Count()} assets in Picturepark");
        }

        private async Task CreateAssetsByTransferAsync(string transferIdentifier, IList<PictureparkAsset> newTargetAssets)
        {
            try
            {
                _logger.LogInformation($"Starting import of transfer {transferIdentifier}...");

                var fileTransfers = new List<FileTransferCreateItem>();

                var transferResult = await CreateFileTransferAsync(transferIdentifier, newTargetAssets);

                var files = await _client.Transfer.SearchFilesByTransferIdAsync(transferResult.Transfer.Id);

                foreach (FileTransfer file in files)
                {
                    var assetForCreation = newTargetAssets.FirstOrDefault(assetForCreationInner => string.Equals(assetForCreationInner.RecommendedFileName, file.Identifier));

                    var fileTransferCreateItem = new FileTransferCreateItem
                    {
                        FileId = file.Id,
                        LayerSchemaIds = new[] {
                            nameof(SmintIoContentLayer),
                            nameof(SmintIoLicenseLayer)
                        },
                        Metadata = assetForCreation.GetMetadata()
                    };

                    fileTransfers.Add(fileTransferCreateItem);
                }

                var partialRequest = new ImportTransferPartialRequest()
                {
                    Items = fileTransfers
                };

                var importResult = await _client.Transfer.PartialImportAsync(transferResult.Transfer.Id, partialRequest);

                await _client.BusinessProcess.WaitForCompletionAsync(importResult.BusinessProcessId);

                files = await _client.Transfer.SearchFilesByTransferIdAsync(transferResult.Transfer.Id);

                foreach (FileTransfer file in files)
                {
                    var assetForCreation = newTargetAssets.FirstOrDefault(assetForCreationInner => string.Equals(assetForCreationInner.RecommendedFileName, file.Identifier));

                    assetForCreation.TargetAssetUuid = file.ContentId;
                }

                _logger.LogInformation($"Finished import of transfer {transferIdentifier}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing files for transfer {transferIdentifier}");

                await TryDeleteTransferAsync(transferIdentifier);

                throw;
            }
        }

        public async Task UpdateAssetsAsync(IList<PictureparkAsset> updatedTargetAssets)
        {
            _logger.LogInformation("Updating assets in Picturepark...");

            var transferIdentifier = $"Smint.io Import {Guid.NewGuid().ToString()}";

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await UpdateAssetsByTransferAsync(transferIdentifier, updatedTargetAssets);
            });

            _logger.LogInformation($"Updated {updatedTargetAssets.Count()} assets in Picturepark");
        }

        private async Task UpdateAssetsByTransferAsync(string transferIdentifier, IList<PictureparkAsset> updatedTargetAssets)
        {
            try
            {
                _logger.LogInformation($"Starting import of transfer {transferIdentifier}...");

                foreach (PictureparkAsset updatedTargetAsset in updatedTargetAssets)
                {
                    var contentDetail = await _client.Content.GetAsync(updatedTargetAsset.TargetAssetUuid, new ContentResolveBehavior[] { ContentResolveBehavior.Metadata });

                    var smintIoContentLayer = contentDetail.Layer<SmintIoContentLayer>("smintIoContentLayer");
                    int? binaryVersion = smintIoContentLayer.BinaryVersion;

                    if (binaryVersion != updatedTargetAsset.BinaryVersion)
                    {
                        var transferResult = await UpdateFileTransferAsync(updatedTargetAsset);

                        await _client.Content.UpdateFileAsync(updatedTargetAsset.TargetAssetUuid, new ContentFileUpdateRequest()
                        {
                            FileTransferId = transferResult.Transfer.Id
                        });
                    }
                }

                var contentMetadataUpdateManyRequest = new ContentMetadataUpdateManyRequest();

                foreach (PictureparkAsset assetForUpdate in updatedTargetAssets)
                {
                    var contentMetadataUpdateItem = new ContentMetadataUpdateItem()
                    {
                        Id = assetForUpdate.TargetAssetUuid,
                        LayerSchemaIds = new[] {
                            nameof(SmintIoContentLayer),
                            nameof(SmintIoLicenseLayer)
                        },
                        Metadata = assetForUpdate.GetMetadata(),
                        LayerSchemasUpdateOptions = UpdateOption.Merge,
                        ContentFieldsUpdateOptions = UpdateOption.Replace
                    };

                    contentMetadataUpdateManyRequest.Items.Add(contentMetadataUpdateItem);
                }

                var result = await _client.Content.UpdateMetadataManyAsync(contentMetadataUpdateManyRequest);

                await _client.BusinessProcess.WaitForCompletionAsync(result.BusinessProcessId);

                _logger.LogInformation($"Finished import of transfer {transferIdentifier}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing files for transfer {transferIdentifier}");

                await TryDeleteTransferAsync(transferIdentifier);

                throw;
            }
        }

        public async Task CreateCompoundAssetsAsync(IList<PictureparkAsset> newTargetCompoundAssets)
        {
            _logger.LogInformation("Creating compound assets in Picturepark...");

            var transferIdentifier = $"Smint.io Import {Guid.NewGuid().ToString()}";

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await CreateCompoundAssetsByTransferAsync(transferIdentifier, newTargetCompoundAssets);
            });

            _logger.LogInformation($"Created {newTargetCompoundAssets.Count()} compound assets in Picturepark");
        }

        private async Task CreateCompoundAssetsByTransferAsync(string transferIdentifier, IList<PictureparkAsset> newTargetCompoundAssets)
        {
            try
            {
                _logger.LogInformation($"Starting import of transfer {transferIdentifier}...");

                await CreateOrUpdateCompoundAssetsAsync(newTargetCompoundAssets);

                _logger.LogInformation($"Finished import of transfer {transferIdentifier}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing files for transfer {transferIdentifier}");

                await TryDeleteTransferAsync(transferIdentifier);

                throw;
            }
        }

        public async Task UpdateCompoundAssetsAsync(IList<PictureparkAsset> updatedTargetCompoundAssets)
        {
            _logger.LogInformation("Updating compound assets in Picturepark...");

            var transferIdentifier = $"Smint.io Import {Guid.NewGuid().ToString()}";

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await UpdateCompoundAssetsByTransferAsync(transferIdentifier, updatedTargetCompoundAssets);
            });

            _logger.LogInformation($"Updated {updatedTargetCompoundAssets.Count()} compound assets in Picturepark");
        }

        private async Task UpdateCompoundAssetsByTransferAsync(string transferIdentifier, IList<PictureparkAsset> updatedTargetCompoundAssets)
        {
            try
            {
                _logger.LogInformation($"Starting import of transfer {transferIdentifier}...");

                await CreateOrUpdateCompoundAssetsAsync(updatedTargetCompoundAssets);

                _logger.LogInformation($"Finished import of transfer {transferIdentifier}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing files for transfer {transferIdentifier}");

                await TryDeleteTransferAsync(transferIdentifier);

                throw;
            }
        }

        private async Task CreateOrUpdateCompoundAssetsAsync(IList<PictureparkAsset> targetAssets)
        {
            foreach (PictureparkAsset targetAsset in targetAssets)
            {
                var targetAssetParts = targetAsset.AssetParts;

                if (targetAssetParts.Count == 0)
                    continue;

                if (string.IsNullOrEmpty(targetAsset.TargetAssetUuid))
                {
                    var contentCreateRequest = new ContentCreateRequest()
                    {
                        ContentSchemaId = nameof(SmintIoCompoundAsset),
                        Content = GetCompoundAssetsMetadata(targetAsset, targetAssetParts),
                        LayerSchemaIds = new[] {
                                nameof(SmintIoContentLayer),
                                nameof(SmintIoLicenseLayer)
                            },
                        Metadata = targetAsset.GetMetadata()
                    };

                    await _client.Content.CreateAsync(contentCreateRequest);
                }
                else
                {
                    var contentUpdateRequest = new ContentMetadataUpdateRequest()
                    {
                        Content = GetCompoundAssetsMetadata(targetAsset, targetAssetParts),
                        LayerSchemaIds = new[] {
                            nameof(SmintIoContentLayer),
                            nameof(SmintIoLicenseLayer)
                        },
                        Metadata = targetAsset.GetMetadata(),
                        LayerSchemasUpdateOptions = UpdateOption.Merge,
                        ContentFieldsUpdateOptions = UpdateOption.Replace
                    };

                    await _client.Content.UpdateMetadataAsync(targetAsset.TargetAssetUuid, contentUpdateRequest);
                }
            }
        }

        private DataDictionary GetCompoundAssetsMetadata(PictureparkAsset asset, IList<PictureparkAsset> assetParts)
        {
            var dataDictionary = new DataDictionary()
            {
                { "name", asset.Name },
                { "compoundAssetParts", assetParts.Select(assetPart => GetCompoundAssetPartMetadata(assetPart)).ToArray() }
            };

            return dataDictionary;
        }

        private object GetCompoundAssetPartMetadata(PictureparkAsset assetPart)
        {
            var dataDictionary = new DataDictionary()
            {
                { "_relationType", "CompoundAssetPart" },
                { "_sourceDocType", "Content" },
                { "_targetDocType", "Content" },
                { "_targetId", assetPart.TargetAssetUuid }
            };

            if (assetPart.BinaryUsage?.Count > 0)
                dataDictionary.Add("usage", assetPart.BinaryUsage);

            return dataDictionary;
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
            bool updateSchemaOnStart = _appOptions.UpdateSchemaOnStart;

            if (!updateSchemaOnStart)
            {
                _logger.LogInformation("Updating the Picturepark schema on start has been turned off");

                return;
            }

            _logger.LogInformation("Initializing Picturpark schemas...");

            _logger.LogInformation("Initializing Picturepark compound asset schema...");

            await InitSchemaAsync(typeof(SmintIoCompoundAsset));

            _logger.LogInformation("Initialized Picturepark compound asset schema");

            _logger.LogInformation("Initializing Picturepark content layer schema...");

            await InitSchemaAsync(typeof(SmintIoContentLayer));

            _logger.LogInformation("Initialized Picturepark content layer schema");

            _logger.LogInformation("Initializing Picturepark license layer schema...");

            await InitSchemaAsync(typeof(SmintIoLicenseLayer));

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
                    if (schema.Id == nameof(SmintIoContentLayer))
                    {
                        await AddSchemaToFileTypesAsync(nameof(SmintIoContentLayer));
                    }
                    else if (schema.Id == nameof(SmintIoLicenseLayer))
                    {
                        await AddSchemaToFileTypesAsync(nameof(SmintIoLicenseLayer));
                    }
                }
            }

            if (schemasToUpdate.Any())
            {
                foreach (var schema in schemasToUpdate)
                {
                    await _client.Schema.UpdateAsync(schema, false);

                    if (schema.Id == nameof(SmintIoContentLayer))
                    {
                        await AddSchemaToFileTypesAsync(nameof(SmintIoContentLayer));
                    }
                    else if (schema.Id == nameof(SmintIoLicenseLayer))
                    {
                        await AddSchemaToFileTypesAsync(nameof(SmintIoLicenseLayer));
                    }
                }
            }
        }

        private async Task AddSchemaToFileTypesAsync(string schemaName)
        {
            foreach (var type in _appOptions.PictureparkFileTypes)
            {
                var typeData = await _client.Schema.GetAsync(type);

                if (typeData.LayerSchemaIds == null)
                {
                    typeData.LayerSchemaIds = new List<string>();
                }

                typeData.LayerSchemaIds.Add(schemaName);

                await _client.Schema.UpdateAsync(typeData, false);
            }

            var compoundTypeData = await _client.Schema.GetAsync(nameof(SmintIoCompoundAsset));

            if (compoundTypeData.LayerSchemaIds == null)
            {
                compoundTypeData.LayerSchemaIds = new List<string>();
            }

            compoundTypeData.LayerSchemaIds.Add(schemaName);

            await _client.Schema.UpdateAsync(compoundTypeData, false);
        }

        /*
        private async Task<CreateTransferResult> CreateWebTransferAsync(string transferIdentifier, IList<PictureparkAsset> assets)
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

        private async Task<CreateTransferResult> CreateFileTransferAsync(string transferIdentifier, IList<PictureparkAsset> assets)
        {
            var filePaths = new List<FileLocations>();

            foreach (var asset in assets)
            {
                if (asset.IsCompoundAsset)
                    continue;

                var localFile = await asset.GetDownloadedFileAsync();

                var filePath = new FileLocations(localFile.FullName, asset.RecommendedFileName, asset.RecommendedFileName);

                filePaths.Add(filePath);
            }

            var request = new CreateTransferRequest
            {
                Name = transferIdentifier,
                TransferType = TransferType.FileUpload,
                Files = assets.Select(asset => new TransferUploadFile()
                {
                    FileName = asset.RecommendedFileName
                }).ToList()
            };

            var uploadOptions = new UploadOptions
            {
                ChunkSize = 1024 * 1024,
                ConcurrentUploads = 4,
                SuccessDelegate = Console.WriteLine,
                ErrorDelegate = ErrorDelegate,
                WaitForTransferCompletion = true
            };

            return await _client.Transfer.UploadFilesAsync(transferIdentifier, filePaths, uploadOptions);
        }

        private async Task<CreateTransferResult> UpdateFileTransferAsync(PictureparkAsset asset)
        {
            var transferIdentifier = $"Smint.io Update Import {Guid.NewGuid().ToString()}";

            var localFile = await asset.GetDownloadedFileAsync();

            var filePaths = new FileLocations[]
            {
                new FileLocations(localFile.FullName, asset.RecommendedFileName, asset.RecommendedFileName)
            };
            
            var request = new CreateTransferRequest
            {
                Name = transferIdentifier,
                TransferType = TransferType.FileUpload,
                Files = new TransferUploadFile[] {
                    new TransferUploadFile()
                    {
                        FileName = asset.RecommendedFileName
                    }
                }
            };

            var uploadOptions = new UploadOptions
            {
                ChunkSize = 1024 * 1024,
                ConcurrentUploads = 4,
                SuccessDelegate = Console.WriteLine,
                ErrorDelegate = ErrorDelegate,
                WaitForTransferCompletion = true
            };

            return await _client.Transfer.UploadFilesAsync(transferIdentifier, filePaths, uploadOptions);
        }

        private void ErrorDelegate((FileLocations File, Exception Exception) exception)
        {
            _logger.LogError("Error during upload to PicturePark: {0}", exception);
        }

        private void InitPictureparkService()
        {
            string accessToken = _authOptions.AccessToken;

            var authClient = new AccessTokenAuthClient(_appOptions.ApiBaseUrl, accessToken, _appOptions.CustomerAlias);

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
