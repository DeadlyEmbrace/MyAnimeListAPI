using System;
using System.Net;
using System.Threading.Tasks;

namespace MyAnimeListAPI
{
    public static class Helper
    {
        private const string CredentialsUrl = "http://myanimelist.net/api/account/verify_credentials.xml";

        public static async Task<bool> VerifyCredentials(string login, string password)
        {
            var result = false;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(CredentialsUrl);

                request.Method = "GET";

                request.UseDefaultCredentials = false;

                request.Credentials = new NetworkCredential(login, password);

                var response = await request.GetResponseAsync().ConfigureAwait(false);

                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    result = true;
                }

                return result;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return false;

                throw;
            }
        }
    }
}