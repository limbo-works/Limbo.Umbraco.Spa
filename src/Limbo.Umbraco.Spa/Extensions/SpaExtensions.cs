using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Limbo.Umbraco.Spa.Extensions;

/// <summary>
/// Static class with various extension methods for the SPA.
/// </summary>
public static class SpaExtensions {

    internal static IQueryCollection ToQueryCollection(this Dictionary<string, StringValues> dictionary) {
        return new QueryCollection(dictionary);
    }

}