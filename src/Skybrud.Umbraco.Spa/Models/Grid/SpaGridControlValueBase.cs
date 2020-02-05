using Skybrud.Umbraco.GridData;
using Skybrud.Umbraco.GridData.Interfaces;

namespace Skybrud.Umbraco.Spa.Models.Grid {

    /// <summary>
    /// This class serves as a special base class that can be used in a SPA context, as classes inheriting from class
    /// can override the <see cref="GetControlForSpa"/> controlling how it's value is outputted in the SPA API endpoint.
    /// </summary>
    public abstract class SpaGridControlValueBase : IGridControlValue {

        #region Properties

        /// <summary>
        /// Gets a reference to the control.
        /// </summary>
        public GridControl Control { get; }

        /// <summary>
        /// Gets whether the value is considered valid.
        /// </summary>
        public virtual bool IsValid => true;

        /// <summary>
        /// Gets a searchable text representing the control value.
        /// </summary>
        /// <returns></returns>
        public virtual string GetSearchableText() {
            return string.Empty;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The control.</param>
        protected SpaGridControlValueBase(GridControl control) {
            Control = control;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Returns an instance of <see cref="SpaGridControl"/> that can be returned via the SPA API endpoint.
        /// </summary>
        /// <returns>An instance of <see cref="SpaGridControl"/>.</returns>
        public virtual SpaGridControl GetControlForSpa() {
            return new SpaGridControl(this, Control.Editor.Alias);
        }

        #endregion

    }

}