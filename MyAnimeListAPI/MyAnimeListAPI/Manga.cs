using System;
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

            //Workaround: Need to add arbitrary token to get a fresh list and not a cached list
            var guid = Guid.NewGuid();

            var queryParameter = "?token=" + guid;

            var result = await WebRequest.Create(string.Format(MangaUrl + "{0}{1}{2}", "mangalist/", userName, queryParameter), "GET");

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

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
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

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.NotFound)
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
        public static async Task<bool> AddMangaAsync(int mangaId, MangaStatus mangaStatus, int chapterRead, int score,
                                                string login, string password)
        {
            var parameters = string.Format("manga_id={0}&status={1}&chapters={2}&score={3}", mangaId, GetMangaStatusName(mangaStatus),
                                           chapterRead, score);

            try
            {
                return await WebRequest.Create(MangaUrl + "mangalist/manga", parameters, login, password, "POST") == string.Empty;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    return false;

                throw;
            }
        }


        public static async Task<bool> UpdateMangaAsync(int mangaId, MangaStatus mangaStatus, int episodeWatched, int volumes, int score,
                                               string login, string password)
        {
            var parameters = string.Format("&status={0}&chapters={1}&volumes={2}&score={3}", GetMangaStatusName(mangaStatus),
                                           episodeWatched, volumes, score);

            try
            {
                return await WebRequest.Create(MangaUrl + "mangalist/manga/" + mangaId, parameters, login, password, "PUT") == string.Empty;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
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

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
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