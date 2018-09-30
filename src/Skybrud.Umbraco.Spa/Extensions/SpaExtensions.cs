using System;

namespace Skybrud.Umbraco.Spa.Extensions {

    public static class SpaExtensions {

		public static bool IsPreviewUrl(this string url) {
			return url.GetPreviewId() > 0;
		}

		public static int GetPreviewId(this string url) {

            int nodeId;

			if (url.Contains("/umbraco/dialogs")) {
				
                Int32.TryParse(url.Split('=')[1], out nodeId);

            } else {

				string[] urlFolders = url.Split('/');

				//tjek om der er indhold i arrayet
				if (urlFolders.Length <= 0) return -1;

				//find nodeId og returnér til nodeId var
				int.TryParse(urlFolders[1].Split('.')[0], out nodeId);
			}

			return nodeId;

        }

    }

}