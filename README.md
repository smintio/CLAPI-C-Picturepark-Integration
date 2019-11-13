# CLAPI-C-Picturepark-Integration
Sample of integrating Smint.io with Picturepark DAM using the Smint.io Content Licensing Consumer API and the Picturepark SDK

**Please note that this is only a command line application example, not a complete plugin implementation!**

If you want to integrate Smint.io to your software system, you should provide a proper plugin with configuration facilities embedded to your user interface. Find out more about the suggested integration features in the Smint.io Integration Guide, which has been provided to you when you signed up as a Smint.io Solution Partner.

**Implemented features**

- Acquiring access and refresh token from Smint.io
- Synchronization of all required Smint.io generic metadata
- Synchronization of all required content and license metadata
- Support for compound assets (aka „multipart“ assets)
- Handling of updates to license purchase transactions that have already been synchronized before
- Live synchronization whenever an asset is being purchased on Smint.io
- Regular synchronization
- Exponential backoff API consumption pattern
- Warning indicators whenever an asset is subject to editorial use or other license restrictions

**Interesting topics**

*Still missing*

Some things are still missing in the implementation and will come up soon (the example is complete on Smint.io side, the issues still open all solely apply to the Picturepark side of things):

- Automatically parse keywords to Picturepark taxononmy

*Authorization*

Authorization with Smint.io is done through the [OAuth2 authorization code flow](https://oauth.net/2/grant-types/authorization-code/). Please study the Smint.io Integration Guide which has been provided to you when you signed up as a Smint.io Solution Partner. It contains a lot of details on authorization and Single Sign On with Smint.io.

*Generic set-up*

The example is set up in .NET Core, using [HostBuilder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.2). This gives us all benefits of ASP.NET Core (like dependency injection), without all the web based overhead that we do not need.

It is based on the [Smint.io Consumer Integration Core package for .NET Core](https://github.com/smintio/CLAPI-C-Integration-Core) that we also use for our production integrations and that you can reuse in your projects as well.

To use the Smint.io Consumer Integration Core package to our application, we provide the required implementations:

- A provider for the reading the settings based on `ISettingsDatabaseProvider`
- A provider for the storing and reading access tokens based on `ITokenDatabaseProvider`
- A provider for the storing and reading synchronization state based on `ISyncDatabaseProvider`

- The sync target implementation based on  `ISyncTarget`

- Different classes representing our target data structures: `PictureparkAsset`, `PictureparkLicenseOption`, `PictureparkLicenseTerm`, `PictureparkReleaseDetails` and `PictureparkDownloadConstraints`

The we need to register all library specific dependencies by calling `AddSmintIoClapicIntegrationCore` in [Program.cs](https://github.com/smintio/CLAPI-C-Picturepark-Integration/blob/master/PictureparkIntegration/Client/Program.cs)

*Regular scheduling*

We let the Smint.io Consumer Integration Core package take care of the regular scheduling of our sync job. If you are interested in it: the regular scheduling of the Smint.io Consumer Integration Core package is implemented in [TimedSynchronizerService.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Services/TimedSynchronizerService.cs).

*On-demand scheduling*

Synchronizations also run on demand, whenever an asset is being purchased on Smint.io. The notification about the purchase is received through a push notification channel. The Smint.io Consumer Integration Core package registers a listener for that channels and handles the notifications.

If you are interested in it: the on-demand scheduling of the Smint.io Consumer Integration Core package is implemented in [PusherService.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Services/PusherService.cs).

*Synchronizing generic metadata*

The Smint.io Consumer Integration Core package provides the logic for querying generic metadata (e.g. binary type enums, license type enums, and so on) from Smint.io and for forwarding the data to the sync target.

The sync target needs to implement methods for creating those enumeration values within the target system, and to get references to those enumeration values from the target system later:

- Content providers
- Content types
- Binary types
- Content categories
- License types
- Release states
- License exclusivities
- License usages
- License sizes
- License placements
- License distributions
- License geographies
- License industries
- License languages
- License usage limits

*Getting assets for synchronization*

The Smint.io Consumer Integration Core package reads the asset metadata from Smint.io. It uses the `ISyncDatabaseProvider` implementation to store the current synchronization state, which allows our code to only synchronize the changes that happened since the last synchronization run.

*Mapping data*

The Smint.io Consumer Integration Core package maps the asset metadata to our Picturepark target data structures.

**Warning: NEVER modify the meaning of data:** Please only assume state if the value is actually being set (not NULL) in the data received from Smint.io. If the value is not present this does NOT mean FALSE or TRUE or whatever. It simply means: *WE DO NOT KNOW*. It can be very dangerous for your users to rely on data that actually is not based on solid facts.

Mapping data is specific to Picturepark, so we map the data to the Picturepark schemas. 

Find the Picturepark schema definitions [here](https://github.com/smintio/CLAPI-C-Picturepark-Integration/tree/master/PictureparkIntegration/Client/Contracts/Picturepark). Those schema classes are being used to create the metadata schema structure in Picturepark.

For receiving the data from the Smint.io Consumer Integration Core package, and to upload it to Picturepark, we then need to also provide the data classes:

- `PictureparkAsset` 
- `PictureparkLicenseOption`
- `PictureparkLicenseTerm`
- `PictureparkReleaseDetails`
- `PictureparkDownloadConstraints`

**Recommendation:** It is recommended that you build a similar structure in your system. Do not take any shortcuts - the data WILL be required by your users!

*Downloading and storing the binaries*

The Smint.io Consumer Integration Core package calls our `ISyncTarget` implementation methods to store all metadata and binaries, and from there we are uploading the metadata and binaries to Picturepark.

**Warning:** Please note that you may receive more than one binary in the response.

**Warning:** The "worldwide" unique ID of a binary is the combination of the `AssetUuid` and the `BinaryUuid` (which is only unique in the scope of the `AssetUuid`), because each asset can contain one or more binaries.

**Warning:** Please note that all binary download URLs are secured and will only stay valid for up to one hour. The download must start within that time interval to succeed.

**Warning:** Please note that we do not only support images, but also (*large*) videos, templates, texts and a lot of other file types. Be prepared to handle those, and do **not** store binary data in memory!

**Recommendation:** The binaries are stored on our systems with no user-friendly name. Please use the `RecommendedFileName` to store the files on your side.

*UI indicators*

The UI indicators that are mentioned in the Smint.io Integration Guide (e.g. editorial use or license term warnings) have been implemented through Picturepark name and thumbnail display patterns.

The license term warning flag considers all relevant terms that could cause problems for the user, and is precalculated and provided by the Smint.io Consumer Integration Core package logic. Please check out [SmintIoApiClientProviderImpl.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Providers/Impl/SmintIoApiClientProviderImpl.cs) from line `198` if you are interested in how the warning flag is being calculated.

For the Smint.io specific layers, the name display pattern set-up is done through the layer schema definitions in code. For compound assets, the thumbnail display pattern set-up is done through the virtual content type definition in code as well.

For the thumbnail views for Picturepark file types, it is necessary to modify the Picturepark file type thumbnail display patterns manually:

- File
- Image
- Video
- Audio
- Document

Add this code to the Picturepark file type thumbnail display patterns to display the Smint.io logo and a warning icon for all assets that are either for editorial use only or that are subject to license terms:

```
{% if data.smintIoContentLayer %}
   &nbsp;<img src="https://www.smint.io/images/favicon.png" width="16"/>
{% endif %}

{% if data.smintIoLicenseLayer.isEditorialUse or 
   data.smintIoLicenseLayer.hasLicenseTerms %} 
   &nbsp;<font color="#ff9800"><i class="material-icons icon-alert md-16"></i></font>
{% endif %}
```

**That's it!**

Have fun working with Smint.io! If there is any issues do not hesitate to drop us an email to [support@smint.io](mailto:support@smint.io) and we'll be happy to help!

**Contributors**

- Linda Gratzer, Dataformers GmbH
- Reinhard Holzner, Smint.io Smarter Interfaces GmbH

© 2019 Smint.io Smarter Interfaces GmbH

Licensed under the MIT License