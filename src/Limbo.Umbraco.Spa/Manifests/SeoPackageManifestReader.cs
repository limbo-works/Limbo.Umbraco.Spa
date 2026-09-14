using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

using static Limbo.Umbraco.Spa.SpaPackage;

namespace Limbo.Umbraco.Spa.Manifests;

public class SpaPackageManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        PackageManifest manifest = new() {
            Id = Alias,
            Name = Name,
            AllowTelemetry = true,
            Version = InformationalVersion,
            Extensions = []
        };

        return await Task.FromResult(new List<PackageManifest> { manifest });

    }

}