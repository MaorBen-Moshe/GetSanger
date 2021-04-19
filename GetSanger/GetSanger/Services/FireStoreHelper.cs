using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using GetSanger.Models;

namespace GetSanger.Services
{
    public static class FireStoreHelper
    {
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

        public static async Task<Activity> GetActivity(int i_ActivityId, string i_ClientID, string i_SangerID)
        {
            List<Activity> client_activities = await GetActivities(i_ClientID);
            List<Activity> sanger_activities = await GetActivities(i_SangerID);
            Activity client_activity = client_activities.Where(activity => activity.ActivityId.Equals(i_ActivityId)).FirstOrDefault();
            Activity sanger_activity = sanger_activities.Where(activity => activity.ActivityId.Equals(i_ActivityId)).FirstOrDefault();
            if (client_activity.Equals(sanger_activity))
            {
                return client_activity;
            }

            throw new ArgumentException("Could not handle the request"); // need to change the message
        }

        public async static void AddActivity(Activity i_Activity)
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

            User client = await GetUser(i_Activity.ClientID);
            i_Activity.ActivityId = await response.Content.ReadAsStringAsync();
            client.Activities.Add(i_Activity); // check if the activity is not already inside
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

        public static async void UpdateActivity(Activity i_Activity) // update activity in user list and in server data base
        {
            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_Activity);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        public static async void UpdateJobOffer(JobOffer i_JobOffer) // update jobOffer in user list and in server data base
        {
            string uri = "uri here";
            string json = JsonSerializer.Serialize(i_JobOffer);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

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

        public static async Task<JobOffer> GetJobOffer(int i_JobId, string i_ClientID)
        {
            List<JobOffer> client_jobOffers = await GetJobOffers(i_ClientID);
            JobOffer client_jobOffer = client_jobOffers.Where(jobOffer => jobOffer.JobId.Equals(i_JobId)).FirstOrDefault();
            if(client_jobOffer != null)
            {
                return client_jobOffer;
            }

            throw new ArgumentException("Could not handle the request"); // need to change the message
        }

        public async static void AddJobOffer(JobOffer i_JobOffer)
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

            User client = await GetUser(i_JobOffer.ClientID);
            i_JobOffer.JobId = await response.Content.ReadAsStringAsync();
            client.JobOffers.Add(i_JobOffer); // check if the activity is not already inside
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

            return JsonSerializer.Deserialize<User>(await response.Content.ReadAsStringAsync());
        }

        public static async void UpdateUser(User i_User)
        {
            string server_uri = "Cloud Function Of FireStore Here";
            string json = JsonSerializer.Serialize(i_User);
            HttpResponseMessage response = await HttpClientService.SendHttpRequest(server_uri, json, HttpMethod.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
