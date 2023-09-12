using System;
using System.Reflection;
using Limbo.Umbraco.Spa.Models;
using Limbo.Umbraco.Spa.Models.Flow;

namespace Limbo.Umbraco.Spa.Exceptions {

    /// <summary>
    /// Wrapper class for exceptions triggered during page life cycle.
    /// </summary>
    public class SpaActionException : SpaException {

        #region Properties

        /// <summary>
        /// A reference to the action group in which the exception was triggered.
        /// </summary>
        public SpaActionGroup Group { get; }

        /// <summary>
        /// The name of the action method in which the exception was triggered.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Gets the message of the exception.
        /// </summary>
        public override string Message => "Failed executing SPA action " + MethodName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new exception based on the specified parameters.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        /// <param name="actionGroup">The current action group.</param>
        /// <param name="method">The name of the method.</param>
        public SpaActionException(SpaRequest request, SpaActionGroup actionGroup, MethodInfo method) : base(request) {
            Group = actionGroup;
            MethodName = method.Name;
        }

        /// <summary>
        /// Initializes a new exception based on the specified parameters.
        /// </summary>
        /// <param name="request">The current SPA request.</param>
        /// <param name="actionGroup">The current action group.</param>
        /// <param name="method">The name of the method.</param>
        /// <param name="inner">A reference to the inner exception.</param>
        public SpaActionException(SpaRequest request, SpaActionGroup actionGroup, MethodInfo method, Exception inner) : base(request, inner) {
            Group = actionGroup;
            MethodName = method.Name;
        }

        #endregion

    }

}