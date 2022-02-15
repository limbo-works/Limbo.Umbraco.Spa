using System;

namespace Skybrud.Umbraco.Spa {

    /// <summary>
    /// Static class representing the overall SPA environment.
    /// </summary>
    public static class SpaEnvironment {

        /// <summary>
        /// Gets or sets the content GUID of the SPA environment.
        ///
        /// When content is updated in Umbraco, the SPA should generate a new GUID, which is then used to tell clients
        /// that they should do a page reload to get the newest version from the API.
        /// </summary>
        public static Guid ContentGuid = Guid.NewGuid();

    }

}