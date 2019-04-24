using System;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Skybrud.Umbraco.Spa.Models;
using Skybrud.Umbraco.Spa.Models.Flow;

namespace Skybrud.Umbraco.Spa.Exceptions {

    public class SpaActionException : Exception {

        public SpaRequest Request { get; }

        public SpaActionGroup Group { get; }

        public string MethodName { get; }

        public override string Message {
            get {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Failed executing SPA action " + MethodName);
                sb.AppendLine();
                sb.AppendLine(JsonConvert.SerializeObject(Request.Arguments, Formatting.Indented));
                return sb.ToString();
            }
        }

        public SpaActionException(SpaRequest request, SpaActionGroup actionGroup, MethodInfo method) {
            Request = request;
            Group = actionGroup;
            MethodName = method.Name;
        }

        public SpaActionException(SpaRequest request, SpaActionGroup actionGroup, MethodInfo method, Exception inner) : base(null, inner) {
            Request = request;
            Group = actionGroup;
            MethodName = method.Name;
        }

    }

}