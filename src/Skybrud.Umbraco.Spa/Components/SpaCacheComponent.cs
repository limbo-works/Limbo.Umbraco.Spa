using Skybrud.Umbraco.Spa.Services;
using Umbraco.Core.Composing;
using Umbraco.Core.Services.Implement;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Spa.Components {

    public class SpaCacheComponent : IComponent {

        private readonly ISpaCacheService _cacheService;

        public SpaCacheComponent(ISpaCacheService cacheService) {
            _cacheService = cacheService;
        }

        public void Initialize() {
            ContentService.Saving += (sender, args) => _cacheService.ClearAll();
            ContentService.Trashing += (sender, args) => _cacheService.ClearAll();
            ContentService.Unpublishing += (sender, args) => _cacheService.ClearAll();
        }

        public void Terminate() { }

    }

}