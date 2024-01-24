using System;
using System.Collections.Generic;
using System.Linq;
using Client.Contracts.Picturepark;
using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Target.Impl;

namespace Client.Contracts
{
    public class PictureparkAsset : BaseSyncAsset<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>
    {
        private DataDictionary _contentMetadata;
        private DataDictionary _licenseMetadata;

        public PictureparkAsset()
        {
            _contentMetadata = new DataDictionary();
            _licenseMetadata = new DataDictionary();
        }

        public DataDictionary GetMetadata()
        {
            return new DataDictionary()
            {
                { nameof(SmintIoContentLayer), _contentMetadata },
                { nameof(SmintIoLicenseLayer), _licenseMetadata }
            };
        }

        public override void SetTransactionUuid(string transactionUuid)
        {
            _contentMetadata.Add("licensePurchaseTransactionUuid", transactionUuid);
        }

        public override void SetContentElementUuid(string contentElementUuid)
        {
            _contentMetadata.Add("contentElementUuid", contentElementUuid);
        }

        public override void SetContentType(string contentTypeKey)
        {
            _contentMetadata.Add("contentType", new { _refId = contentTypeKey });
        }

        public override void SetContentProvider(string contentProviderKey)
        {
            _contentMetadata.Add("contentProvider", new { _refId = contentProviderKey });
        }

        public override void SetContentCategory(string contentCategoryKey)
        {
            _contentMetadata.Add("category", new { _refId = contentCategoryKey });
        }

        public override void SetName(IDictionary<string, string> name)
        {
            _contentMetadata.Add("name", name);
        }

        public override void SetDescription(IDictionary<string, string> description)
        {
            _contentMetadata.Add("description", description);
        }

        public override void SetSmintIoUrl(string smintIoUrl)
        {
            _contentMetadata.Add("smintIoUrl", smintIoUrl);
        }

        public override void SetCreatedAt(DateTimeOffset createdAt)
        {
            _contentMetadata.Add("createdAt", createdAt);
        }

        public override void SetLastUpdatedAt(DateTimeOffset lastUpdatedAt)
        {
            _contentMetadata.Add("lastUpdatedAt", lastUpdatedAt);
        }

        public override void SetPurchasedAt(DateTimeOffset purchasedAt)
        {
            _contentMetadata.Add("purchasedAt", purchasedAt);
        }

        public override void SetCartUuid(string cartUuid)
        {
            _contentMetadata.Add("cartPurchaseTransactionUuid", cartUuid);
        }

        public override void SetHasBeenCancelled(bool hasBeenCancelled)
        {
            _contentMetadata.Add("hasBeenCancelled", hasBeenCancelled);
            _licenseMetadata.Add("hasBeenCancelled", hasBeenCancelled);
        }

        public override void SetBinaryUuid(string binaryUuid)
        {
            _contentMetadata.Add("binaryUuid", binaryUuid);
        }

        public override void SetBinaryType(string binaryTypeKey)
        {
            _contentMetadata.Add("binaryType", new { _refId = binaryTypeKey });
        }

        public override void SetBinaryUsage(IDictionary<string, string> binaryUsage)
        {
            // only applicable to compound assets

            BinaryUsage = binaryUsage;
        }

        public override void SetBinaryCulture(string binaryCulture)
        {
            _contentMetadata.Add("binaryCulture", binaryCulture);
        }

        public override void SetBinaryVersion(int binaryVersion)
        {
            _contentMetadata.Add("binaryVersion", binaryVersion);
        }

        public override void SetProjectUuid(string projectUuid)
        {
            _contentMetadata.Add("projectUuid", projectUuid);
        }

        public override void SetProjectName(IDictionary<string, string> projectName)
        {
            _contentMetadata.Add("projectName", projectName);
        }

        public override void SetCollectionUuid(string collectionUuid)
        {
            _contentMetadata.Add("collectionUuid", collectionUuid);
        }

        public override void SetCollectionName(IDictionary<string, string> collectionName)
        {
            _contentMetadata.Add("collectionName", collectionName);
        }

        public override void SetKeywords(IDictionary<string, string[]> keywords)
        {
            var joinedKeywords = JoinValues(keywords);

            _contentMetadata.Add("keywords", joinedKeywords);
        }

        public override void SetCopyrightNotices(IDictionary<string, string> copyrightNotices)
        {
            _contentMetadata.Add("copyrightNotices", copyrightNotices);
        }

        public override void SetIsEditorialUse(bool isEditorialUse)
        {
            _contentMetadata.Add("isEditorialUse", isEditorialUse);
            _licenseMetadata.Add("isEditorialUse", isEditorialUse);
        }

        public override void SetIsAi(bool isAi)
        {
            _contentMetadata.Add("isAi", isAi);
        }

        public override void SetHasRestrictiveLicenseTerms(bool hasRestrictiveLicenseTerms)
        {
            _contentMetadata.Add("hasRestrictiveLicenseTerms", hasRestrictiveLicenseTerms);
            _licenseMetadata.Add("hasRestrictiveLicenseTerms", hasRestrictiveLicenseTerms);
        }

        public override void SetLicenseType(string licenseTypeKey)
        {
            _licenseMetadata.Add("licenseType", new { _refId = licenseTypeKey });
        }

        public override void SetLicenseeUuid(string licenseeUuid)
        {
            _licenseMetadata.Add("licenseeUuid", licenseeUuid);
        }

        public override void SetLicenseeName(string licenseeName)
        {
            _licenseMetadata.Add("licenseeName", licenseeName);
        }

        public override void SetLicenseText(IDictionary<string, string> licenseText)
        {
            _licenseMetadata.Add("licenseText", licenseText);
        }

        public override void SetLicenseUrls(IDictionary<string, string[]> licenseUrls)
        {
            var joinedLicenseUrls = JoinValues(licenseUrls);

            _licenseMetadata.Add("licenseUrls", joinedLicenseUrls);
        }

        public override void SetDownloadConstraints(PictureparkDownloadConstraints downloadConstraints)
        {
            _licenseMetadata.Add("downloadConstraints", downloadConstraints.Metadata);
        }

        public override void SetReleaseDetails(PictureparkReleaseDetails releaseDetails)
        {
            _licenseMetadata.Add("releaseDetails", releaseDetails.Metadata);
        }

        public override void SetLicenseTerms(IList<PictureparkLicenseTerm> licenseTerms)
        {
            _licenseMetadata.Add("licenseTerms", licenseTerms.Select(licenseTerm => licenseTerm.Metadata).ToArray());
        }

        private IDictionary<string, string> JoinValues(IDictionary<string, string[]> dictionary)
        {
            if (dictionary == null || !dictionary.Any())
            {
                return null;
            }

            var result = new Dictionary<string, string>();

            foreach (var (key, value) in dictionary)
            {
                string joinedValues = String.Join(", ", value);

                result[key] = joinedValues;
            }

            return result;
        }
    }
}
