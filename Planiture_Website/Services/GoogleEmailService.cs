using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Planiture_Website.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planiture_Website.Services
{
    public class GoogleEmailService
    {
        public static GmailService GetGmail(string refreshToken)
        {
            GmailService gmailService = null;

            var userCredential = GoogleService.GetGoogleUserCredentialByRefreshToken(refreshToken);

            if(userCredential != null)
            {
                gmailService = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = userCredential,
                    ApplicationName = GoogleApiHelper.ApplicationName
                });
            }
            return gmailService;
        }
    }
}
