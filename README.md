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

*Generic set-up*

The example is set up in .NET Core, using [HostBuilder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.2). This gives us all benefits of ASP.NET Core (like dependency injection), without all the web based overhead that we do not need.

It is based on the [Smint.io Consumer Integration Core package for .NET Core](https://github.com/smintio/CLAPI-C-Integration-Core) that we also use for our production integrations and that you can reuse in your projects as well.

*Regular scheduling*

We regularily schedule our sync job. This makes sure that in all cases, at some point in time the assets from Smint.io end up in Picturepark. Upon the regularily running sync, we also synchronize generic metadata (see below).

The regular scheduling is implemented in [TimedSynchronizerService.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Services/TimedSynchronizerService.cs).

*On-demand scheduling*

A synchronization also runs on demand, whenever an asset is being purchased on Smint.io. The notification about the purchase is received through the Pusher channel. The on-demand sync does NOT synchronize generic metadata, because this takes some time and would delay the purchased assets streaming into Picturepark in near real-time.

The on-demand scheduling is implemented in [PusherService.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Services/PusherService.cs).

*Synchronizing generic metadata*

Whenever generic metadata needs to be synchronized, we first query the generic metadata from Smint.io using the Smint.io `getGenericMetadataSync` API. It gives back a lot of generic metadata, in different languages, with one call. The most important metadata is listed here:

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

We then transform the metadata to `(key) -> {culture, name}` representation and push it to Picturepark as translated value lists. 

The values are also cached locally for further reference, as Picturepark maintains proprietary ID values that we'll need to lookup when synchronizing assets from Smint.io to Picturepark.

Reading generic and transforming generic metadata from Smint.io is implemented in [SmintIoApiClientProviderImpl.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Providers/Impl/SmintIoApiClientProviderImpl.cs).

*Getting assets for synchronization*

When synchronizing assets from Smint.io, you need to query so-called *license purchase transactions* by using the `getLicensePurchaseTransactionsForSync` API. It has just two optional parameters. One is the number of records given in `limit` and the `continuationUuid` parameter.

We first check if we had a previous synchronization run. From previous runs we always remember the `continuationUuid` value that we received in the query result of the last run, and use it as the `continuationUuid` parameter when the next synchronization runs (see `SynchronizeAssetsAsync` in [SyncJobImpl.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Jobs/Impl/SyncJobImpl.cs)).

*Interpreting data*

Please look at `LoadAssetsAsync` in [SmintIoApiClientProviderImpl.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Providers/Impl/SmintIoApiClientProviderImpl.cs), and also study the Smint.io Integration Guide which has been provided to you when you signed up as a Smint.io Solution Partner.

**Warning: NEVER modify the meaning of data:** See e.g. the evaluation of the `isEditorialUse` variable in `LoadAssetsAsync`. We only assume state if the value is actually being set (not NULL) in the data received from Smint.io. If the value is not present this does NOT mean FALSE or TRUE or whatever. It simply means: *WE DO NOT KNOW*. It can be very dangerous for your users to rely on data that actually is not based on solid facts.

*Mapping data*

Mapping data is specific to your software system. In the example we map the data to Picturepark schemas.

Find the Picturepark schema definitions [here](https://github.com/smintio/CLAPI-C-Picturepark-Integration/tree/master/PictureparkIntegration/Client/Contracts/Picturepark). 

**Recommendation:** It is recommended that you build a similarily comprehensive structure in your system. Do not take any shortcuts - the data WILL be required by your users!

The actual mapping is being done in [PictureparkSyncTargetImpl.cs](https://github.com/smintio/CLAPI-C-Picturepark-Integration/blob/master/PictureparkIntegration/Client/Target/Impl/PictureparkSyncTargetImpl.cs).

*Downloading and storing the binaries*

The download of the binaries is rathery easy. Once you got the license purchase transaction metadata downloaded, you can use the `GetLicensePurchaseTransactionBinariesForSyncAsync` to get the binaries that belong to the asset (implemented in [SmintIoApiClientProviderImpl.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Providers/Impl/SmintIoApiClientProviderImpl.cs)).

**Warning:** Please note that all binary download URLs are secured and will only stay valid for up to one hour. The download must start within that time interval to succeed.

**Warning:** Please note that you may receive more than one binary in the response. The binaries you receive contain a `uuid` field which is unique *in the license purchase transaction context*. The `uuid` helps you to differentiate the binaries within the license purchase transaction context.

**Warning:** Please note that we do not only support images, but also (*large*) videos, templates, texts and a lot of other file types. Be prepared to handle those!

**Recommendation:** The binaries are stored on our systems with no user-friendly name. Please use the `recommended_file_name` that is being provided in the binary metadata object to store the files on your side.

*Exponential backoff*

As mentioned in the Smint.io Integration Guide, it is important to use an Exponential Backoff Strategy when consuming foreign APIs. We use [Polly](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly) to do that in our example. Please check out `GetRetryStrategy` in [SmintIoApiClientProviderImpl.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Providers/Impl/SmintIoApiClientProviderImpl.cs) to learn more about how this can be done.

*UI indicators*

The UI indicators that are mentioned in the Smint.io Integration Guide (e.g. editorial use or license term warnings) have been implemented through Picturepark name and thumbnail display patterns.

The license term warning flag has been pre-calculated during import, considering all relevant terms that could cause problems for the user. Please check out [SmintIoApiClientProviderImpl.cs](https://github.com/smintio/CLAPI-C-Integration-Core/blob/master/NetCore/Providers/Impl/SmintIoApiClientProviderImpl.cs) from line `198` for more information.

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

*Study the Smint.io Integration Guide*

Please also study the Smint.io Integration Guide which has been provided to you when you signed up as a Smint.io Solution Partner. It contains a lot of more details especially on the SDK in general, on Single Sign-On, authorization and on all the metadata that you need to process.

**That's it!**

Have fun working with Smint.io! If there is any issues do not hesitate to drop us an email to [support@smint.io](mailto:support@smint.io) and we'll be happy to help!

**Contributors**

- Linda Gratzer, Dataformers GmbH
- Reinhard Holzner, Smint.io Smarter Interfaces GmbH

© 2019 Smint.io Smarter Interfaces GmbH

Licensed under the MIT License