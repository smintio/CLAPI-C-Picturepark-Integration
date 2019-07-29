﻿using System.Collections.Generic;

namespace Client.Contracts
{
    public class SmintIoGenericMetadata
    {
        public IList<SmintIoMetadataElement> ContentProviders;

        public IList<SmintIoMetadataElement> ContentTypes;
        public IList<SmintIoMetadataElement> BinaryTypes;

        public IList<SmintIoMetadataElement> ContentCategories;

        public IList<SmintIoMetadataElement> LicenseTypes;
        public IList<SmintIoMetadataElement> ReleaseStates;

        public IList<SmintIoMetadataElement> LicenseUsages;
        public IList<SmintIoMetadataElement> LicenseSizes;
        public IList<SmintIoMetadataElement> LicensePlacements;
        public IList<SmintIoMetadataElement> LicenseDistributions;
        public IList<SmintIoMetadataElement> LicenseGeographies;
        public IList<SmintIoMetadataElement> LicenseVerticals;
    }
}
