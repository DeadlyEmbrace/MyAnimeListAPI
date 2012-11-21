using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace MyAnimeListAPI
{
    public static class Manga
    {
        private const string MangaUrl = "http://mal-api.com/";

        public enum MangaStatus
        {
            Reading = 1,
            Completed = 2,
            OnHold = 3,
            Dropped = 4,
            PlanToRead = 6
        }

        /// <summary> 
        /// Get the user's mangalist
        /// </summary>
        public static async Task<string> GetMangaListAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return string.Empty;

            var result = await WebRequest.Create(MangaUrl + "mangalist/" + userName, "GET");

            return result;
        }

        /// <summary>
        /// The method will return the manga's detail without user's detail.
        /// Without: user's score, watched status, watched episodes
        /// </summary>
        public static async Task<string> GetMangaDetailAsync(int mangaId)
        {
            string result = null;

            try
            {
                result = await WebRequest.Create(MangaUrl + "manga/" + mangaId, "GET");

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
        /// The method will return the manga's detail with user's detail.
        /// With: user's score, watched status, watched episodes
        /// </summary>
        public static async Task<string> GetMangaDetailAsync(int mangaId, string login, string password)
        {
            string result = null;

            try
            {
                result = await WebRequest.Create(MangaUrl + "manga/" + mangaId + "?mine=1", login, password, "GET");

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

        public static async Task<string> SearchMangaAsync(string query)
        {
            var result = await WebRequest.Create(MangaUrl + "manga/search?q=" + HttpUtility.UrlEncode(query), "GET");

            return result;
        }

        /// <summary>
        /// The method will add an manga in the user's list.
        /// </summary>
        public static async Task<bool> AddMangaAsync(int mangaId, MangaStatus mangaStatus, int episodeWatched, int score,
                                                string login, string password)
        {
            var parameters = string.Format("manga_id={0}&status={1}&episodes={2}&score={3}", mangaId, GetMangaStatusName(mangaStatus),
                                           episodeWatched, score);

            try
            {
                return await WebRequest.Create(MangaUrl + "mangalist/manga", parameters, login, password, "POST") == string.Empty;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest)
                    return false;

                throw;
            }
        }


        public static async Task<bool> UpdateMangaAsync(int mangaId, MangaStatus mangaStatus, int episodeWatched, int score,
                                               string login, string password)
        {
            var parameters = string.Format("&status={0}&episodes={1}&score={2}", GetMangaStatusName(mangaStatus),
                                           episodeWatched, score);

            try
            {
                return await WebRequest.Create(MangaUrl + "mangalist/manga/" + mangaId, parameters, login, password, "PUT") == string.Empty;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest)
                    return false;

                throw;
            }
        }

        /// <summary>
        /// Delete an manga from a user's manga list.
        /// Return the original manga if the manga was successfully deleted from mangalist.
        /// </summary>
        public static async Task<string> DeleteMangaAsync(int mangaId, string login, string password)
        {
            string result = null;

            try
            {
                result = await WebRequest.Create(MangaUrl + "mangalist/manga/" + mangaId, login, password, "DELETE");

                return result;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.InternalServerError)
                    return result;

                throw;
            }
        }


        private static string GetMangaStatusName(MangaStatus mangaStatus)
        {
            var status = string.Empty;

            switch (mangaStatus)
            {
                case MangaStatus.Reading:
                    status = "reading";
                    break;

                case MangaStatus.Completed:
                    status = "completed";
                    break;

                case MangaStatus.Dropped:
                    status = "dropped";
                    break;

                case MangaStatus.OnHold:
                    status = "on-hold";
                    break;

                case MangaStatus.PlanToRead:
                    status = "plan to read";
                    break;
            }

            return status;
        }
    }
}