# `UpdateArguments`

When generating the API output for a given request/page, the overall SPA request is represented by the [**`SpaRequest`**](./../sparequest.md) class, while the arguments and options of the request is represented by the [**`SpaRequestOptions`**](./sparequestoptions.md) class. The [**`SpaRequest`**](./../sparequest.md) class has a `Arguments` property that exposes the [**`SpaRequestOptions`**](./../sparequestoptions.md) instance.

If you need to update the arguments/options, the ideal place to do this is via the `UpdateArguments` method. 

```csharp
// TODO: find a good example to put here 
```

The `UpdateArguments` method is executed right after the [**`InitArguments`**](./initarguments.md). If you wish to use a derived type instead the default [**`SpaRequestOptions`**](./../sparequestoptions.md), you should use the [**`InitArguments`**](./initarguments.md) method to do this first.
