using Skybrud.Umbraco.Spa.Models;

namespace Skybrud.Umbraco.Spa.Exceptions {

    public class SpaSiteNotFoundException : SpaException {

        #region Constructors

        public SpaSiteNotFoundException(SpaRequest request) : base(request) {
            
        }

        public SpaSiteNotFoundException(SpaRequest request, string message) : base(request, message) {
            
        }

        #endregion

    }

}