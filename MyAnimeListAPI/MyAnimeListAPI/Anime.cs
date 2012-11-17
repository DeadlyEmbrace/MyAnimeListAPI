using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace MyAnimeListAPI
{
    public static class Anime
    {
        private const string AnimeUrl = "http://mal-api.com/";

        /// <summary> 
        /// Get the user's animelist
        /// </summary>
        public static async Task<string> GetAnimeListAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return string.Empty;

            string result = null;

            var request = (HttpWebRequest)WebRequest.Create(AnimeUrl + "animelist/" + userName);

            request.Method = "GET";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var buffer = new byte[4096];

                        await stream.ReadAsync(buffer, 0, 4096).ConfigureAwait(false);

                        result = System.Text.Encoding.UTF8.GetString(buffer);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The method will return the anime's detail without user's detail
        /// Without: user's score, watched status, watched episodes
        /// </summary>
        public static async Task<string> GetAnimeDetailAsync(int animeId)
        {
            string result = null;

            var request = (HttpWebRequest)WebRequest.Create(AnimeUrl + "anime/" + animeId);

            request.Method = "GET";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var buffer = new byte[4096];

                        await stream.ReadAsync(buffer, 0, 4096).ConfigureAwait(false);

                        result = System.Text.Encoding.UTF8.GetString(buffer);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The method will return the anime's detail with user's detail
        /// With: user's score, watched status, watched episodes
        /// </summary>
        public static async Task<string> GetAnimeDetailAsync(int animeId, string login, string password)
        {
            string result = null;

            var request = (HttpWebRequest)WebRequest.Create(AnimeUrl + "anime/" + animeId + "?mine=1");

            request.Method = "GET";

            request.Credentials = new NetworkCredential(login, password);

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var buffer = new byte[4096];

                        await stream.ReadAsync(buffer, 0, 4096).ConfigureAwait(false);

                        result = System.Text.Encoding.UTF8.GetString(buffer);
                    }
                }
            }

            return result;
        }

        public static async Task<string> SearchAnimeAsync(string query)
        {
            string result = null;

            var request = (HttpWebRequest)WebRequest.Create(AnimeUrl + "anime/search?q=" + HttpUtility.UrlEncode(query));

            request.Method = "GET";

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            if (((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var buffer = new byte[4096];

                        await stream.ReadAsync(buffer, 0, 4096).ConfigureAwait(false);

                        result = System.Text.Encoding.UTF8.GetString(buffer);
                    }
                }
            }

            return result;
        }
    }
}