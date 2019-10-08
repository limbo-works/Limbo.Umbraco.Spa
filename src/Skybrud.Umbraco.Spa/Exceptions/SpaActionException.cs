using System;
using System.Reflection;
using Skybrud.Umbraco.Spa.Models;
using Skybrud.Umbraco.Spa.Models.Flow;

namespace Skybrud.Umbraco.Spa.Exceptions {

    public class SpaActionException : SpaException {

        #region Properties
        
        public SpaActionGroup Group { get; }

        public string MethodName { get; }

        public override string Message => "Failed executing SPA action " + MethodName;

        #endregion

        public SpaActionException(SpaRequest request, SpaActionGroup actionGroup, MethodInfo method) : base(request) {
            Group = actionGroup;
            MethodName = method.Name;
        }

        public SpaActionException(SpaRequest request, SpaActionGroup actionGroup, MethodInfo method, Exception inner) : base(request, inner) {
            Group = actionGroup;
            MethodName = method.Name;
        }

    }

}