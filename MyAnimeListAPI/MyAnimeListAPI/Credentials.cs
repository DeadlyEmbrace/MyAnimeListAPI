using System;
using System.Net;
using System.Threading.Tasks;

namespace MyAnimeListAPI
{
    public static class Credentials
    {
        private const string CredentialsUrl = "http://myanimelist.net/api/account/verify_credentials.xml";

        public static async Task<bool> VerifyCredentials(string login, string password)
        {
            var result = false;

            try
            {
                var guid = Guid.NewGuid();

                //Force the URL to use new Credentials.
                //If the request is done with a valid Credentials then the next request (with an invalid Credentials) will be accepted... 
                //This queryParameter is a workaround to use the new Credentials
                var queryParameter = "?token=" + guid;

                var request = (HttpWebRequest)System.Net.WebRequest.Create(CredentialsUrl + queryParameter);

                request.Method = "GET";

                request.UseDefaultCredentials = false;

                request.Credentials = new NetworkCredential(login, password);

                var response = await request.GetResponseAsync().ConfigureAwait(false);

                if (((HttpWebResponse)response).StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = true;
                }

                return result;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return false;

                throw;
            }
        }
    }
}