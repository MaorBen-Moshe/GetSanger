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
            User user = await GetUser(i_UserID);
            return (List<Activity>)user.Activities;
        }

        public static async Task<Activity> GetActivity(int i_ActivityId, string i_ClientID, string i_SangerID)
        {
            User client = await GetUser(i_ClientID);
            User sanger = await GetUser(i_SangerID);
            Activity client_activity = client.Activities.Where(activity => activity.ActivityId.Equals(i_ActivityId)).FirstOrDefault();
            Activity sanger_activity = sanger.Activities.Where(activity => activity.ActivityId.Equals(i_ActivityId)).FirstOrDefault();
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

            User client = await GetUser(i_Activity.ClientID);
            User sanger = await GetUser(i_Activity.SangerID);
            client.Activities.Add(i_Activity); // check if the activity is not already inside
            sanger.Activities.Add(i_Activity); // check if the activity is not already inside
            UpdateUser(client);
            UpdateUser(sanger);

        }

        public async static void DeleteActivity(Activity i_Activity, string i_UserId = null) // delete activity from user list and from server data base
        {
            if (i_UserId != null)
            {
                User user = await GetUser(i_UserId);
                user.Activities.Remove(i_Activity);
                UpdateUser(user);
                return;
            }
            // by default activity is removed from both users
            User client = await GetUser(i_Activity.ClientID);
            User sanger = await GetUser(i_Activity.SangerID);
            client.Activities.Remove(i_Activity); // check if the activity is not already inside
            sanger.Activities.Remove(i_Activity); // check if the activity is not already inside
            UpdateUser(client);
            UpdateUser(sanger);
        }

        public static async void UpdateActivity(Activity i_Activity) // update activity in user list and in server data base
        {
            User client = await GetUser(i_Activity.ClientID);
            User sanger = await GetUser(i_Activity.SangerID);
            client.Activities = (from activity in client.Activities where activity.Equals(i_Activity) == false select activity).ToList();
            sanger.Activities = (from activity in sanger.Activities where activity.Equals(i_Activity) == false select activity).ToList();
            client.Activities.Add(i_Activity);
            sanger.Activities.Add(i_Activity);
            UpdateUser(client);
            UpdateUser(sanger);
        }

        public static async void UpdateJobOffer(JobOffer i_JobOffer) // update jobOffer in user list and in server data base
        {
            User user = await GetUser(i_JobOffer.ClientID);
            user.JobOffers = (from job in user.JobOffers where job.Equals(i_JobOffer) == false select job).ToList();
            user.JobOffers.Add(i_JobOffer);
            UpdateUser(user);
        }

        public static async Task<List<JobOffer>> GetJobOffers(string i_UserID)
        {
            User user = await GetUser(i_UserID);
            return (List<JobOffer>)user.JobOffers;
        }

        public async static void AddJobOffer(JobOffer i_JobOffer)
        {
            if (i_JobOffer == null)
            {
                throw new ArgumentNullException("JobDetails is null");
            }

            User client = await GetUser(i_JobOffer.ClientID);
            client.JobOffers.Add(i_JobOffer); // check if the activity is not already inside
            UpdateUser(client);
        }

        public async static void DeleteJobOffer(JobOffer i_JobOffer)
        {
            if (i_JobOffer == null)
            {
                throw new ArgumentNullException("Activity is null");
            }

            User client = await GetUser(i_JobOffer.ClientID);
            client.JobOffers = (from job in client.JobOffers where job.Equals(i_JobOffer) == false select job).ToList();
            UpdateUser(client);
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
