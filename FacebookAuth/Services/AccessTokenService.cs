using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace FacebookAuth.Services
{
    public class AccessTokenService
    {
        private const string accessTokenKey = "access_token";
        private string graphUrl = "https://graph.facebook.com/oauth/";

        /// <summary>
        /// Will contain the next format of the url
        /// https://graph.facebook.com/oauth/authorize?client_id={{app_id}}&redirect_uri={{redirect_uri}}&scope={{scope_post_to_wall}}
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public string GetAuthorizationCodeUrl(string scope, string redirectUri)
        {
            if (string.IsNullOrEmpty(scope))
            {
                return string.Empty;
            }
            var queryParams = new Dictionary<string, string>()
            {
                {"client_id", Startup.FacebookSettings.AppId},
                {"redirect_uri", redirectUri},
                {"scope", scope},
            };
            return QueryHelpers.AddQueryString(string.Concat(graphUrl, "authorize"), queryParams);
        }

        public string GetAccessToken(string scope, string redirectUri, string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return string.Empty;
            }
            //removing the query string
            redirectUri = new UriBuilder(redirectUri) { Query = string.Empty }.ToString();

            var queryParams = new Dictionary<string, string>()
            {
                {"client_id", Startup.FacebookSettings.AppId},
                {"redirect_uri", redirectUri},
                {"scope", scope},
                {"code", code},
                {"client_secret", Startup.FacebookSettings.AppSecret}
            };

            var url = QueryHelpers.AddQueryString(string.Concat(graphUrl, accessTokenKey), queryParams);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            if (request == null)
            {
                return string.Empty;
            }
            using (var response = request.GetResponseAsync().Result)
            {
                if (response == null)
                {
                    return string.Empty;
                }

                var reader = new StreamReader(response.GetResponseStream());
                var jsonResult = reader.ReadToEnd();

                var vals = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult);
                if (vals.ContainsKey(accessTokenKey))
                {
                    return vals[accessTokenKey];
                }
            }
            return string.Empty;
        }
    }
}
