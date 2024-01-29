# `InitArguments`

When generating the API outout for a given page, the `InitArguments` method is the first one to be called. The purpose of the method is to parse various input - eg. from the request URI - and process that into an instance `SpaRequestOptions`.

Normally it should it shouldn't be necessary to override this method, but if you wish to use a derived type instead of the default `SpaRequestOptions`, you can do something like in the example below:

```csharp
protected override void InitArguments(SpaRequest request) {

    // We're not calling the base method here as we need to set the
    // arguments to an instance of OdSpaRequestOptions instead of the
    // default SpaRequestOptions
    request.Arguments = new MySpaRequestOptions(request, this);

}
```

When overriding the method, it generally shouldn't be necessary to call the base method as we'd want to set `request.Arguments` on our own. If you instead wish to change any of the properties of the default `SpaRequestOptions` instance, you should override the [`UpdateArguments`](./updatearguments.md) instead.
