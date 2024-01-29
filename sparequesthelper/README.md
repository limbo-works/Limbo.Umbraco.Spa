# `SpaRequestHelper` class

The package features a `SpaRequestHelper` which is the primary compontent responsible for generating the response for the `GetData` endpoint. When requesting a page in the SPA setup, there are a number of methods the helper goes through. The methods can be divided into three overall groups.

When looking at a single request, the methods in the first group are always invoked. If micro caching is enabled and the page is already in the cache, the methods in group two will be skipped. The third group offers a method to do anything at the very end of the page flow.

The methods listed below are all virtual, meaning you can create a custom class extending the `SpaRequestHelper` class, and then override the methods that you need.

## First group

- [**InitArguments**](./initarguments.md)  
Responsible for initializing an instance of `SpaRequestOptions` wrapping some of the parameters passed to the `GetData` endpoint as well as reading from the package's configuration. Generally you should use the `UpdateArguments` method if you only need to change some of the arguments, while the `InitArguments` method can be used if you want to use a custom class that extends the default `SpaRequestOptions` class.
  
- [**UpdateArguments**](./updatearguments.md)  
Let's you modify the `SpaRequestOptions` instance created by the `InitArguments`.

- [**ReadFromCache**](./readfromcache.md)  
Tries to read the current page from the internal micro cache - based on the cache key specified by the `SpaRequestOptions` instance.

## Second group

- [**InitSite**](./initsite.md)  
Responsible for resolving the `IPublishedContent` representing the current site.

- **PostInitSite**  
Let's you hook into the page flow right after the `InitSite` method has been called.

- **PreContentLookup**  
Let's you hook into the page flow right before the `ContentLookup` method is called.

- **ContentLookup**  
Method for resolving the `IPublishedContent` representing the current page.

- **PostContentLookup**  
Let's you hook into the page flow right after the `ContentLookup` method has been called. A good place to handle virtual content.

- **PreSetupCulture**  
Let's you hook into the page flow right before the `SetupCulture` method is called.

- **SetupCulture**  
Method for resolving the `IPublishedContent` representing the current culture, if any.

- **PostSetupCulture**  
Let's you hook into the page flow right after the `SetupCulture` method has been called. A good place to handle virtual content.

- **InitSiteModel**  
Method responsible for creating the `SpaSiteModel` returned for the `site` part in the `GetData` endpoint.

- **HandleNotFound**  
Method responsible the handling when the requested URL doesn't match an existing page. This method is also responsible for handling redirects - although this is further handled by the `HandleInboundRedirects` method.

- **InitContentModel**  
Method responsible for creating the `SpaContentModel` returned for the `content` part in the `GetData` endpoint.

- **InitDataModel**  
Method responsible for creating the `SpaDataModel` representing the root object returned for tby the `GetData` endpoint.

- **InitNavigationModel**  
Method responsible for creating the `SpaNavigationModel` returned for the `navigation` part in the `GetData` endpoint.

- **InitCustomModels**  
Here if you need to do something custom on the `SpaDataModel` model.

- **PrePushToCache**  
Method called right before the `SpaDataModel` is pushed to the cache.

- **PushToCache**  
Method responsible for pushing the `SpaDataModel` to the cache.

## Third group

- **RaisinInTheSausageEnd**  
As suggested by the name, this method is called at the very end of the page flow.
