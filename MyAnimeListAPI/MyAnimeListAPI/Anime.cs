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

            var client = new WebClient();

            //The MAL-API will cause a System.Net.WebException if the login is not found
            var result = await client.DownloadStringTaskAsync(AnimeUrl + "animelist/" + userName).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// The method will return the anime's detail without user's detail
        /// Without: user's score, watched status, watched episodes
        /// </summary>
        public static async Task<string> GetAnimeDetailAsync(int animeId)
        {
            var client = new WebClient();

            var result = await client.DownloadStringTaskAsync(AnimeUrl + "anime/" + animeId).ConfigureAwait(false);

            return result;
        }

        /// <summary>
        /// The method will return the anime's detail with user's detail
        /// With: user's score, watched status, watched episodes
        /// </summary>
        public static async Task<string> GetAnimeDetailAsync(int animeId, string login, string password)
        {
            var client = new WebClient { Credentials = new NetworkCredential(login, password) };

            var result = await client.DownloadStringTaskAsync(AnimeUrl + "anime/" + animeId + "?mine=1").ConfigureAwait(false);

            return result;
        }

        public static async Task<string> SearchAnimeAsync(string query)
        {
            var client = new WebClient();

            var result = await client.DownloadStringTaskAsync(AnimeUrl + "anime/search?q=" + HttpUtility.UrlEncode(query)).ConfigureAwait(false);

            return result;
        }
    }
}