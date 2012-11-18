using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MyAnimeListAPI
{
    public static class WebRequest
    {
        public static async Task<string> Create(string url)
        {
            var request = (HttpWebRequest)System.Net.WebRequest.Create(url);

            request.Method = "GET";

            return await GetResponse(request);
        }

        private static async Task<string> GetResponse(HttpWebRequest request)
        {
            string result = null;

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var buffer = new byte[2048];

                        using (var destinationStream = new MemoryStream())
                        {
                            int bytesRead;

                            do
                            {
                                bytesRead = await stream.ReadAsync(buffer, 0, 2048).ConfigureAwait(false);

                                destinationStream.Write(buffer, 0, bytesRead);
                            } while (bytesRead != 0);

                            result = System.Text.Encoding.UTF8.GetString(destinationStream.ToArray());
                        }
                    }
                }
            }

            return result;
        }

        public static async Task<string> Create(string url, string login, string password)
        {
            var request = (HttpWebRequest)System.Net.WebRequest.Create(url);

            request.UseDefaultCredentials = false;

            request.Credentials = new NetworkCredential(login, password);

            request.Method = "GET";

            return await GetResponse(request);
        }
    }
}