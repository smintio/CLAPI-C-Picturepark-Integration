using Client.Providers;
using Microsoft.Extensions.Logging;
using Picturepark.SDK.V1.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Contracts;
using System.IO;
using System.Net;
using Client.Contracts.Picturepark;
using Client.Providers.Impl.Models;

namespace Client.Jobs.Impl
{
    public class SyncJobImpl: ISyncJob
    {
        private const string Folder = "temp";

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private readonly ISyncDatabaseProvider _syncDatabaseProvider;

        private readonly ISmintIoApiClientProvider _smintIoClient;
        private readonly IPictureparkApiClientProvider _pictureparkClient;

        private readonly ILogger _logger;

        public SyncJobImpl(
            ISyncDatabaseProvider syncDatabaseProvider,
            ISmintIoApiClientProvider smintIoClient,
            IPictureparkApiClientProvider pictureparkClient,
            ILogger<SyncJobImpl> logger)
        {
            _syncDatabaseProvider = syncDatabaseProvider;

            _smintIoClient = smintIoClient;
            _pictureparkClient = pictureparkClient;

            _logger = logger;
        }

        public async Task SynchronizeAssetsAsync()
        {
            _logger.LogInformation("Starting Smint.io asset synchronization...");

            await Semaphore.WaitAsync();

            var folderName = Folder + new Random().Next(1000, 9999);

            var syncDatabaseModel = _syncDatabaseProvider.GetSyncDatabaseModel();

            DateTimeOffset? minDate = null;
            int offset = 0;

            if (syncDatabaseModel != null)
            {
                // get last committed state

                minDate = syncDatabaseModel.NextMinDate;
                offset = syncDatabaseModel.NextOffset;
            }

            var originalMinDate = minDate;

            try
            {
                IEnumerable<SmintIoAsset> assets = null;

                do
                {
                    assets = await _smintIoClient.GetAssetsAsync(minDate, offset);

                    if (assets != null && assets.Any())
                    {
                        CreateTempFolder(folderName);

                        minDate = assets.Max(asset => asset.LptLastUpdatedAt);

                        var transformedAssets = await TransformAssetsAsync(assets, folderName);

                        await _pictureparkClient.ImportAssetsAsync(transformedAssets);
                    }

                    if (minDate == originalMinDate)
                    {
                        // in this page we have a full page with same dates
                        // which is very unlikely but can happen

                        offset += assets.Count();
                    }
                    else
                    {
                        offset = 0;
                    }

                    // store committed data

                    _syncDatabaseProvider.SetSyncDatabaseModel(new SyncDatabaseModel()
                    {
                        NextMinDate = minDate,
                        NextOffset = offset
                    });

                    _logger.LogInformation($"Synchronized {assets.Count()} Smint.io assets");
                } while (assets != null && assets.Any());

                _logger.LogInformation("Finished Smint.io asset synchronization");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error synchronizing assets");
            }
            finally
            {
                RemoveTempFolder(folderName);

                Semaphore.Release();
            }
        }

        private void CreateTempFolder(string folderName)
        {
            Directory.CreateDirectory(folderName);
        }

        private void RemoveTempFolder(string folderName)
        {
            if (Directory.Exists(folderName))
                Directory.Delete(folderName, true);
        }

        /*
         * Directly transfer the asset binary data from Smint.io to Picturepark
         * This is not working currently, some error occurs on Picturepark side
         */

        /* private IEnumerable<PictureparkAsset> TransformAssetsWeb(IEnumerable<SmintIoAsset> assets, string folderName)
        {
            IList<PictureparkAsset> targetAssets = new List<PictureparkAsset>();
            foreach (var smintAsset in assets)
            {
                var ppAsset = new PictureparkAsset
                {
                    TransferId = smintAsset.CartPTUuid,
                    DownloadUrl = smintAsset.DownloadUrl,
                    Id = smintAsset.LPTUuid,
                    Metadata = new DataDictionary()
                    {
                        {nameof(SmintIoContentLayer), GetContentMetaData(smintAsset)},
                        {nameof(SmintIoLicenseLayer), GetLicenseMetaData(smintAsset)}
                    }
                };


                targetAssets.Add(ppAsset);
            }
            return targetAssets;
        } */

        private async Task<IEnumerable<PictureparkAsset>> TransformAssetsAsync(IEnumerable<SmintIoAsset> assets, string folderName)
        {
            IList<PictureparkAsset> targetAssets = new List<PictureparkAsset>();

            foreach (var smintAsset in assets)
            {
                _logger.LogInformation($"Transforming and downloading Smint.io LPT {smintAsset.LPTUuid}...");

                string fileName = $"{folderName}/{smintAsset.LPTUuid}.{ExtractFileExtension(smintAsset.DownloadUrl)}";

                var transferIdentifier = $"SMINTIO_LPT_{smintAsset.LPTUuid}";

                var ppAsset = new PictureparkAsset()
                {
                    TransferId = transferIdentifier,
                    DownloadUrl = fileName,
                    Id = smintAsset.LPTUuid
                };

                await DownloadFileAsync(new Uri(smintAsset.DownloadUrl), fileName);

                ppAsset.Metadata = new DataDictionary()
                {
                    { nameof(SmintIoContentLayer), GetContentMetaData(smintAsset) },
                    { nameof(SmintIoLicenseLayer), GetLicenseMetaData(smintAsset) }
                };

                targetAssets.Add(ppAsset);

                _logger.LogInformation($"Transformed and downloaded Smint.io LPT {smintAsset.LPTUuid}");
            }

            return targetAssets;
        }

        private DataDictionary GetContentMetaData(SmintIoAsset asset)
        {
            var keywords = JoinValues(asset.Keywords);
            var categories = JoinValues(asset.Categories);            

            return new DataDictionary
            {
                { "provider", asset.Provider },
                { "name", asset.Name?.Count > 0 ? asset.Name : null },
                { "description", asset.Description?.Count > 0 ? asset.Description : null },
                { "keywords", keywords?.Count > 0 ? keywords : null },
                { "categories", categories?.Count > 0 ? categories : null },
                { "copyrightNotices", asset.CopyrightNotices?.Count > 0 ? asset.CopyrightNotices : null },
                { "projectUuid", asset.ProjectUuid },
                { "projectName", asset.ProjectName },
                { "collectionUuid", asset.CollectionUuid },
                { "collectionName", asset.CollectionName },
                { "smintIoUrl", asset.SmintIoUrl },
                { "purchasedAt", asset.PurchasedAt },
                { "createdAt", asset.CreatedAt }              
            };
        }

        private DataDictionary GetLicenseMetaData(SmintIoAsset asset)
        {
            return new DataDictionary
            {
                { "licenseeName", asset.LicenseeName },
                { "licenseeUuid", asset.LicenseeUuid },
                { "licenseType", asset.LicenseType },
                { "licenseOptions", GetLicenseOptions(asset.LicenseOptions) },
                { "usageRestrictions", GetUsageRestrictions(asset.UsageRestrictions) },
                { "downloadRestrictions", GetDownloadRestrictions(asset.DownloadRestrictions) },
                { "releaseDetails", asset.ReleaseDetail == null ? null : GetReleaseDetailsMetaData(asset.ReleaseDetail) },
                { "effectiveIsEditorialUse", asset.EffectiveIsEditorialUse }
            };
        }

        private DataDictionary[] GetLicenseOptions(IList<SmintIoLicenseOptions> options)
        {
            if (options == null || !options.Any())
            {
                return null;
            }

            return options.Select(option => new DataDictionary
            {
                { "optionName", option.OptionName },
                { "licenseText", option.LicenseText },
            }).ToArray();
        }

        private DataDictionary[] GetUsageRestrictions(IList<SmintIoUsageRestrictions> restrictions)
        {
            if (restrictions == null || !restrictions.Any())
            {
                return null;
            }

            return restrictions.Select(restr => new DataDictionary
            {
                { "effectiveAllowedGeographies", restr.EffectiveAllowedGeographies },
                { "effectiveRestrictedGeographies", restr.EffectiveRestrictedGeographies },
                { "effectiveValidFrom", restr.EffectiveValidFrom },
                { "effectiveValidUntil", restr.EffectiveValidUntil },
                { "effectiveToBeUsedUntil", restr.EffectiveToBeUsedUntil }
            }).ToArray();
        }

        private DataDictionary GetDownloadRestrictions(SmintIoDownloadRestrictions restriction)
        {
            if (restriction == null)
            {
                return null;
            }

            return new DataDictionary()
            {
                {"effectiveMaxUsers", restriction.EffectiveMaxUsers},
                {"effectiveMaxUsages", restriction.EffectiveMaxUsages},
                {"effectiveMaxDownloads", restriction.EffectiveMaxDownloads}
            };
        }

        private DataDictionary GetReleaseDetailsMetaData(SmintIoReleaseDetail detail)
        {
            return new DataDictionary()
            {
                { "modelReleaseState", detail.ModelReleaseState },
                { "propertyReleaseState", detail.PropertyReleaseState },
                { "providerAllowedUseComment", detail.ProviderAllowedUseComment?.Count > 0 ? detail.ProviderAllowedUseComment : null },
                { "providerReleaseComment", detail.ProviderReleaseComment?.Count > 0 ? detail.ProviderReleaseComment : null },
                { "providerUsageRestrictions", detail.ProviderUsageRestrictions?.Count > 0 ? detail.ProviderUsageRestrictions : null }
            };
        }

        private IDictionary<string, string> JoinValues(IDictionary<string, string[]> dict)
        {
            if (dict == null || !dict.Any())
            {
                return null;
            }

            var result = new Dictionary<string, string>();

            foreach (var (key, value) in dict)
            {
                string joinedValues = String.Join(", ", value);

                result[key] = joinedValues;
            }

            return result;
        }

        private string ExtractFileExtension(string url)
        {
            url = url.Substring(0, url.IndexOf("?"));

            string fileName = url.Substring(url.LastIndexOf("/") + 1);

            return fileName.Substring(fileName.LastIndexOf(".") + 1);
        }

        private async Task DownloadFileAsync(Uri uri, string fileName)
        {
            try
            {
                WebClient wc = new WebClient();

                await wc.DownloadFileTaskAsync(uri, fileName);
            }
            catch (WebException we)
            {
                _logger.LogError(we, "Error downloading asset");

                throw;
            }
        }
    }
}
