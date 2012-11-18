﻿using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace MyAnimeListAPI
{
    public static class Anime
    {
        private const string AnimeUrl = "http://mal-api.com/";

        public enum AnimeStatus
        {
            Watching = 1,
            Completed = 2,
            OnHold = 3,
            Dropped = 4,
            PlanToWatch = 6
        }

        /// <summary> 
        /// Get the user's animelist
        /// </summary>
        public static async Task<string> GetAnimeListAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return string.Empty;

            var result = await WebRequest.Create(AnimeUrl + "animelist/" + userName, "GET");

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
                result = await WebRequest.Create(AnimeUrl + "anime/" + animeId, "GET");

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
                result = await WebRequest.Create(AnimeUrl + "anime/" + animeId + "?mine=1", login, password, "GET");

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
            var result = await WebRequest.Create(AnimeUrl + "anime/search?q=" + HttpUtility.UrlEncode(query), "GET");

            return result;
        }

        /// <summary>
        /// The method will add an anime in the user's list
        /// If you add an existing animeId, the request will throw a BadRequest exception
        /// </summary>
        public static async Task<bool> AddAnimeAsync(int animeId, AnimeStatus animeStatus, int episodeWatched, int score,
                                                string login, string password)
        {
            var parameters = string.Format("anime_id={0}&status={1}&episodes={2}&score={3}", animeId, GetAnimeStatusName(animeStatus),
                                           episodeWatched, score);

            try
            {
                return await WebRequest.Create(AnimeUrl + "animelist/anime", parameters, login, password, "POST") == string.Empty;
            }
            catch (WebException ex)
            {
                var response = (HttpWebResponse)ex.Response;

                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.BadRequest)
                    return false;

                throw;
            }
        }


        public static async Task<bool> UpdateAnimeAsync(int animeId, AnimeStatus animeStatus, int episodeWatched, int score,
                                               string login, string password)
        {
            var parameters = string.Format("&status={0}&episodes={1}&score={2}", GetAnimeStatusName(animeStatus),
                                           episodeWatched, score);

            try
            {
                return await WebRequest.Create(AnimeUrl + "animelist/anime/" + animeId, parameters, login, password, "PUT") == string.Empty;
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
        /// Delete an anime from a user's anime list.
        /// Return the original anime if the anime was successfully deleted from animelist.
        /// </summary>
        public static async Task<string> DeleteAnimeAsync(int animeId, string login, string password)
        {
            string result = null;

            try
            {
                result = await WebRequest.Create(AnimeUrl + "animelist/anime/" + animeId, login, password, "DELETE");

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


        private static string GetAnimeStatusName(AnimeStatus animeStatus)
        {
            var status = string.Empty;

            switch (animeStatus)
            {
                case AnimeStatus.Watching:
                    status = "watching";
                    break;

                case AnimeStatus.Completed:
                    status = "completed";
                    break;

                case AnimeStatus.Dropped:
                    status = "dropped";
                    break;

                case AnimeStatus.OnHold:
                    status = "on-hold";
                    break;

                case AnimeStatus.PlanToWatch:
                    status = "plan to watch";
                    break;
            }

            return status;
        }
    }
}