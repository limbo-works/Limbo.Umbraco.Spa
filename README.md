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
