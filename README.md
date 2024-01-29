# Limbo SPA

SPA (single page application) package for Umbraco 9+.




<br /><br />

## Setup

### Rewriting the `appHost` parameter

For requests to the `GetData` end point, developers can specify an `appHost` parameter indicating the domain / host name of the inbound request. If not specified, the actual host name of the inbound request is used instead.

Normally host name of the inbound request is the one you want to use, but in scenarios where multiple sites in the same solution all use the same API domain, or the backend is behind a reverse proxy, the `appHost` parameter may differ from the actual host name.

In such cases, it might be important that the backend sees the host name specified `appHost` instead of the host name of the request. This can done by adding the following lines to the `Configure` methond in your solution's `Startup.cs` file:

```csharp
app.UseRewriter(new RewriteOptions()
    .Add(RewriteAppHost)
);
```

And then adding this method to the `Startup` class as well:

```csharp
  private static void RewriteAppHost(RewriteContext context) {
      HttpRequest request = context.HttpContext.Request;
      if (!request.Query.TryGetString("appHost", out string? appHost)) return;
      context.Result = RuleResult.SkipRemainingRules;
      request.Host = new HostString(appHost);
  }
```

For a reverse proxy, this means that if the user accesses the proxy through `https://www.mydomain.com/`, the `appHost` parameter with a value of `www.mydomain.com` is sent to the site behind the proxy, so that site will see the requested host name as `www.mydomain.com` rather than something like `backend.mydomain.com`.


<br /><br /><br />

## `SpaRequestHelper` class

The package features a `SpaRequestHelper` which is the primary compontent responsible for generating the response for the `GetData` endpoint. When requesting a page in the SPA setup, there are a number of methods the helper goes through. The methods can be divided into three overall groups.

When looking at a single request, the methods in the first group are always invoked. If micro caching is enabled and the page is already in the cache, the methods in group two will be skipped. The third group offers a method to do anything at the very end of the page flow.

The methods listed below are all virtual, meaning you can create a custom class extending the `SpaRequestHelper` class, and then override the methods that you need.

### First group

- **InitArguments**
Responsible for initializing an instance of `SpaRequestOptions` wrapping some of the parameters passed to the `GetData` endpoint as well as reading from the package's configuration. Generally you should use the `UpdateArguments` method if you only need to change some of the arguments, while the `InitArguments` method can be used if you want to use a custom class that extends the default `SpaRequestOptions` class.
  
- **UpdateArguments**  
Let's you modify the `SpaRequestOptions` instance created by the `InitArguments`.

- **ReadFromCache**
Tries to read the current page from the internal micro cache - based on the cache key specified by the `SpaRequestOptions` instance.

### Second group

- **InitSite**  
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

### Third group

- **RaisinInTheSausageEnd**  
As suggested by the name, this method is called at the very end of the page flow.



<br /><br /><br />

## `GetData` endpoint

For legacy reasons, the SPA `GetData` is available at the following URLs:

```
/api/spa
/api/spa/GetData
/umbraco/api/spa
/umbraco/api/spa/GetData
```

All four URLs support both `GET` and `OPTIONS` requests.

### Parameters

- `appHost`  
The host name of the frontend request. The host name of the API request is used as fallback if not specified.

- `appProtocol`  
The prococol of the frontend request.  The protocol of the API request is used as fallback if not specified.

- `parts`  
A comma separated value indentifying the parts to be returned by the API. Possible values are `site`, `navigation` and `content`. All three parts are returned if not specified.

- `url`  
The URL of the requested page. May be omitted if both `siteId` and `pageId` are specified.

- `navContext`  
A boolean value indicating whether the `navigation.context` object should be returned.

- `navLevels`  
The maximum level to return for items in the `navigation.context` object.

- `siteId`  
The ID of the site. Must be specified when using `pageId` instead of `url`.

- `pageId`  
The ID of the requested page - may be used as an alternative to `url`.

- `culture`  
As culture code of the current culture. Only relevant when working culture variant content.

- `cache`  
A boolean flag indicating whether micro caching should be enabled.
































