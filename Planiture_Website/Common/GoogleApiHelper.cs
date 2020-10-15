using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Planiture_Website.Common
{
    public class GoogleApiHelper
    {
        public static string ApplicationName = "Google Api DotNetCore Web Client";

        public static string ClientId = "207344143010-4180jnm17jqfo8u1flejtn9f0cjiancu.apps.googleusercontent.com";

        public static string ClientSecret = "nvUqZIesTr9wje03kEvRrbwe";

        public static string RedirectUri = "http://localhost";

        public static string OauthUri = "https://accounts.google.com/o/oauth2/auth";

        public static string TokenUri = "https://oauth2.googleapis.com/token";

        public static List<string> GetScopes()
        {
            List<string> scopes = new List<string>();
            scopes.Add("https://www.googleapis.com/auth/userinfo.email");
            scopes.Add("https://www.googleapis.com/auth/gmail.compose");
            scopes.Add("https://www.googleapis.com/auth/gmail.send");
            scopes.Add("https://www.googleapis.com/auth/gmail.modify");

            return scopes;
        }

        public static string GetAuthScopes()
        {
            string scopes = string.Empty;

            foreach (var scope in GetScopes())
            {
                scopes += scope + " ";
            }

            return scopes;
        }

        public static string GetOauthUri(string extraParam)
        {
            StringBuilder sbUri = new StringBuilder(OauthUri);
            sbUri.Append("client_id=" + ClientId);
            sbUri.Append("&redirect_uri=" + RedirectUri);
            sbUri.Append("&response_type=" + "code");
            sbUri.Append("&scope=" + GetAuthScopes());
            sbUri.Append("&access_type=" + "offline");
            sbUri.Append("&state=" + extraParam);
            sbUri.Append("&approval_prompt=" + "force");

            return sbUri.ToString();
        }

        public static async Task<GoogleToken> GetTokenByCode(string code)
        {
            GoogleToken token = null;

            var postData = new
            {
                code = code,
                client_id = ClientId,
                client_secret = ClientSecret,
                redirect_uri = RedirectUri,
                grant_type = "authorization_code"
            };

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(TokenUri, content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        token = JsonConvert.DeserializeObject<GoogleToken>(responseString);
                    }
                }
            }

            return token;
        }


    }
}
