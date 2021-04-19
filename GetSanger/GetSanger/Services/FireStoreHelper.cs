﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GetSanger.Models;

namespace GetSanger.Services
{
    public enum CollectionType { Activity, JobOffer, Rating };

    public static class FireStoreHelper
    {
        // experiment
        #region Generic_Methods

        public static async Task<List<T>> GetCollection<T>(string i_UserId, CollectionType i_Type)
        {
            string uri = "uri here";
            Dictionary<string, string> id = new Dictionary<string, string>
            {
                ["userid"] = i_UserId,
                ["type"] = i_Type.ToString()
            };

            string json = JsonSerializer.Serialize(id);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            return JsonSerializer.Deserialize<List<T>>(await response.Content.ReadAsStringAsync());
        }

        #endregion

        #region Activities
        public static async Task<List<Activity>> GetActivities(string i_UserID)
        {
            string uri = "uri here";
            Dictionary<string, string> id = new Dictionary<string, string>
            {
                ["userid"] = i_UserID
            };

            string json = JsonSerializer.Serialize(id);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            return JsonSerializer.Deserialize<List<Activity>>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<Activity> GetActivity(string i_ActivityId)
        {
            string uri = "uri here";
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["activityid"] = i_ActivityId
            };

            string json = JsonSerializer.Serialize(data);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            return JsonSerializer.Deserialize<Activity>(await response.Content.ReadAsStringAsync());
        }

        public async static void AddActivity(params Activity[] i_Activity)
        {
            if(i_Activity == null)
            {
                throw new ArgumentNullException("Activity is null");
            }

            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_Activity);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async static void DeleteActivity(Activity i_Activity, string i_UserId = null) // delete activity from user list and from server data base
        {
            if (i_Activity == null)
            {
                throw new ArgumentNullException("Activity is null");
            }

            if(i_UserId != null) // we want to remove the activity just from sanger
            {
                if (i_Activity.ClientID.Equals(i_UserId)) // here we want to delete from client
                {
                    i_Activity.SangerID = null;
                }
                else if (i_Activity.SangerID.Equals(i_UserId)) // here we want to delete from sanger
                {
                    i_Activity.ClientID = null;
                }
            }

            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_Activity);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public static async void UpdateActivity(params Activity[] i_Activity) // update activity in user list and in server data base
        {
            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_Activity);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
        #endregion

        #region JobOffers
        public static async Task<List<JobOffer>> GetJobOffers(string i_UserID)
        {
            string uri = "uri here";
            Dictionary<string, string> id = new Dictionary<string, string>
            {
                ["userid"] = i_UserID
            };

            string json = JsonSerializer.Serialize(id);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            return JsonSerializer.Deserialize<List<JobOffer>>(await response.Content.ReadAsStringAsync());
        }

        public static async Task<JobOffer> GetJobOffer(string i_JobId)
        {
            string uri = "uri here";
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["jobid"] = i_JobId
            };

            string json = JsonSerializer.Serialize(data);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            return JsonSerializer.Deserialize<JobOffer>(await response.Content.ReadAsStringAsync());
        }

        public async static void AddJobOffer(params JobOffer[] i_JobOffer)
        {
            if (i_JobOffer == null)
            {
                throw new ArgumentNullException("JobDetails is null");
            }

            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_JobOffer);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async static void DeleteJobOffer(JobOffer i_JobOffer)
        {
            if (i_JobOffer == null)
            {
                throw new ArgumentNullException("Job offer is null");
            }

            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_JobOffer);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public static async void UpdateJobOffer(params JobOffer[] i_JobOffer) // update jobOffer in user list and in server data base
        {
            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_JobOffer);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
        #endregion

        #region Ratings
        public static async Task<List<Rating>> GetRatings(string i_UserID)
        {
            string uri = "uri here";
            Dictionary<string, string> id = new Dictionary<string, string>
            {
                ["userid"] = i_UserID
            };

            string json = JsonSerializer.Serialize(id);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            return JsonSerializer.Deserialize<List<Rating>>(await response.Content.ReadAsStringAsync());
        }

        public async static void AddRating(params Rating[] i_Rating)
        {
            if (i_Rating == null)
            {
                throw new ArgumentNullException("Activity is null");
            }

            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_Rating);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async static void DeleteRating(Rating i_Rating) // delete activity from user list and from server data base
        {
            if (i_Rating == null)
            {
                throw new ArgumentNullException("Activity is null");
            }

            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_Rating);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public static async void UpdateRating(params Rating[] i_Rating) // update activity in user list and in server data base
        {
            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_Rating);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        #endregion

        #region User
        public static async Task<User> GetUser(string i_UserID)
        {
            string server_uri = "Cloud Function Of FireStore Here";
            Dictionary<string, string> details = new Dictionary<string, string>()
            {
                ["userid"] = i_UserID,
            };
            string json = JsonSerializer.Serialize(details);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(server_uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            User user = JsonSerializer.Deserialize<User>(await response.Content.ReadAsStringAsync());
            user.Activities = await GetActivities(user.UserID);
            user.JobOffers = await GetJobOffers(user.UserID);
            user.Ratings = await GetRatings(user.UserID);
            return user;
        }

        public async static void AddUser(User i_User)
        {
            string server_uri = "Cloud Function Of FireStore Here";
            string json = JsonSerializer.Serialize(i_User);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(server_uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async static void DeleteUser(string i_UserId)
        {
            string uri = "server uri here";
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["userid"] = i_UserId
            };

            string json = JsonSerializer.Serialize(data);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public static async void UpdateUser(User i_User)
        {
            string server_uri = "Cloud Function Of FireStore Here";
            // the three are not serialized with the user, we update the manually
            UpdateActivity(i_User.Activities.ToArray());
            UpdateJobOffer(i_User.JobOffers.ToArray());
            UpdateRating(i_User.Ratings.ToArray());

            string json = JsonSerializer.Serialize(i_User);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(server_uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
        #endregion
    }
}
