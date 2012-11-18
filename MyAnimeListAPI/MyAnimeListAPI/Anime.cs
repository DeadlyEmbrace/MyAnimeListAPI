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

            var result = await WebRequest.Create(AnimeUrl + "animelist/" + userName);

            return result;
        }

        /// <summary>
        /// The method will return the anime's detail without user's detail
        /// Without: user's score, watched status, watched episodes
        /// </summary>
        public static async Task<string> GetAnimeDetailAsync(int animeId)
        {
            string result = null;

            try
            {
                result = await WebRequest.Create(AnimeUrl + "anime/" + animeId);

                return result;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return result;

                throw;
            }
        }

        /// <summary>
        /// The method will return the anime's detail with user's detail
        /// With: user's score, watched status, watched episodes
        /// </summary>
        public static async Task<string> GetAnimeDetailAsync(int animeId, string login, string password)
        {
            string result = null;

            try
            {
                result = await WebRequest.Create(AnimeUrl + "anime/" + animeId + "?mine=1", login, password);

                return result;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound)
                    return result;

                throw;
            }
        }

        public static async Task<string> SearchAnimeAsync(string query)
        {
            var result = await WebRequest.Create(AnimeUrl + "anime/search?q=" + HttpUtility.UrlEncode(query));

            return result;
        }
    }
}