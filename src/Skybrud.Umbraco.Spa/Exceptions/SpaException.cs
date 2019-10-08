using System;
using Skybrud.Umbraco.Spa.Models;

namespace Skybrud.Umbraco.Spa.Exceptions  {

    public class SpaException : Exception {

        #region Properties

        public SpaRequest Request { get; }

        #endregion

        public SpaException(SpaRequest request) {
            Request = request;
        }

        public SpaException(SpaRequest request, Exception inner) : base(null, inner) {
            Request = request;
        }

        public SpaException(SpaRequest request, string message) : base(message) {
            Request = request;
        }

    }

}