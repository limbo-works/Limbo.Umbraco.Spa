using Skybrud.Umbraco.Spa.Components;
using Skybrud.Umbraco.Spa.Repositories;
using Skybrud.Umbraco.Spa.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Spa.Composers {

    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class SpaComposer : IUserComposer {

        public void Compose(Composition composition) {
            composition.Register<ISpaCacheService, SpaCacheService>();
            composition.Register<SpaDomainRepository>();
            composition.Components().Append<SpaCacheComponent>();
        }

    }

}