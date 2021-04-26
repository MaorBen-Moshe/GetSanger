using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GetSanger.Models;

namespace GetSanger.Services
{
    public enum CollectionType
    {
        Activity,
        JobOffer,
        Rating
    };

    public static class FireStoreHelper
    {
        // experiment

        #region Generic_Methods

        public static async Task<List<T>> GetCollection<T>(string i_UserId,
            CollectionType i_Type)
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

        public static async Task AddActivity(params Activity[] i_Activity)
        {
            if (i_Activity == null)
            {
                throw new ArgumentNullException("Activity is null");
            }

            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/AddActivities";
            string json = JsonSerializer.Serialize(i_Activity);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public async static Task DeleteActivity(Activity i_Activity,
            string i_UserId = null) // delete activity from user list and from server data base
        {
            if (i_Activity == null)
            {
                throw new ArgumentNullException("Activity is null");
            }

            if (i_UserId != null) // we want to remove the activity just from sanger
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

        public static async Task UpdateActivity(params Activity[] i_Activity) // update activity in user list and in server data base
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

        public async static Task AddJobOffer(params JobOffer[] i_JobOffer)
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

        public async static Task DeleteJobOffer(JobOffer i_JobOffer)
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

        public static async Task UpdateJobOffer(params JobOffer[] i_JobOffer) // update jobOffer in user list and in server data base
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
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/GetRatings";
            Dictionary<string, string> id = new Dictionary<string, string>
            {
                ["UserId"] = i_UserID
            };

            string json = JsonSerializer.Serialize(id);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            return JsonSerializer.Deserialize<List<Rating>>(await response.Content.ReadAsStringAsync());
        }

        public static async Task AddRating(params Rating[] i_Rating)
        {
            if (i_Rating == null)
            {
                throw new ArgumentNullException("Rating is null");
            }

            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/AddRating";
            string json = JsonSerializer.Serialize(i_Rating);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public static async Task DeleteRating(params Rating[] i_Rating) // delete rating from user list and from server data base
        {
            if (i_Rating == null)
            {
                throw new ArgumentNullException("Rating is null");
            }

            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/DeleteRating";
            string json = JsonSerializer.Serialize(i_Rating);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public static async Task UpdateRating(params Rating[] i_Rating) // update rating in user list and in server data base
        {
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/UpdateRating";
            string json = JsonSerializer.Serialize(i_Rating);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        #endregion

        #region Reports
        public static async Task AddReport(Report i_Report)
        {
            if (i_Report == null)
            {
                throw new ArgumentNullException("Rating is null");
            }

            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_Report);

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
            string server_uri = "https://europe-west3-get-sanger.cloudfunctions.net/GetUser";
            Dictionary<string, string> details = new Dictionary<string, string>()
            {
                ["UserId"] = i_UserID,
            };
            string json = JsonSerializer.Serialize(details);
            string idToken = await AuthHelper.GetIdTokenAsync();
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(server_uri, json, HttpMethod.Post, idToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }

            User user = JsonSerializer.Deserialize<User>(await response.Content.ReadAsStringAsync());
            user.Activities = new ObservableCollection<Activity>(await GetActivities(user.UserID));
            user.JobOffers = new ObservableCollection<JobOffer>(await GetJobOffers(user.UserID));
            user.Ratings = new ObservableCollection<Rating>(await GetRatings(user.UserID));
            return user;
        }

        public static async Task AddUser(User i_User)
        {
            string server_uri = "https://europe-west3-get-sanger.cloudfunctions.net/AddUserToDatabase";

            Dictionary<string, User> requestDictionary = new Dictionary<string, User>()
            {
                ["User"] = i_User
            };

            string json = JsonSerializer.Serialize(requestDictionary);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response =
                await HttpClientService.SendHttpRequest(server_uri, json, HttpMethod.Post, idToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public static async Task DeleteUser(string i_UserId)
        {
            string uri = "https://europe-west3-get-sanger.cloudfunctions.net/DeleteUser";
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["UserId"] = i_UserId
            };

            string json = JsonSerializer.Serialize(data);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public static async Task UpdateUser(User i_User)
        {
            string server_uri = "https://europe-west3-get-sanger.cloudfunctions.net/UpdateUser";

            // the three are not serialized with the user, we update the manually
            await UpdateActivity(i_User.Activities.ToArray());
            await UpdateJobOffer(i_User.JobOffers.ToArray());
            await UpdateRating(i_User.Ratings.ToArray());

            string json = JsonSerializer.Serialize(i_User);
            string idToken = await AuthHelper.GetIdTokenAsync();

            HttpResponseMessage response = await HttpClientService.SendHttpRequest(server_uri, json, HttpMethod.Post, idToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        #endregion
    }
}