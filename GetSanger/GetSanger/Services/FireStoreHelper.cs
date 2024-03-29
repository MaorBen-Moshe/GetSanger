﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using GetSanger.Models;
using Xamarin.Essentials;
using GetSanger.Exceptions;

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
        #region Activities

        public static async Task<List<Activity>> GetActivities(string i_UserID)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/GetActivities";
                Dictionary<string, string> id = new Dictionary<string, string>
                {
                    ["UserId"] = i_UserID
                };

                string json = ObjectJsonSerializer.SerializeForServer(id);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                string responseJson = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(responseJson);
                }

                List<Activity> activities = ObjectJsonSerializer.DeserializeForServer<List<Activity>>(responseJson);
                convertDateTimeToLocalTime(activities);
                return activities;
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        private static void convertDateTimeToLocalTime(List<Activity> i_Activities)
        {
            foreach (var activity in i_Activities)
            {
                activity.JobDetails.Date = activity.JobDetails.Date.ToLocalTime();
            }
        }

        public static async Task<Activity> GetActivity(string i_ActivityId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/GetActivity";
                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    ["ActivityId"] = i_ActivityId
                };

                string json = ObjectJsonSerializer.SerializeForServer(data);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to get activity.");
                }

                Activity activity = ObjectJsonSerializer.DeserializeForServer<Activity>(await response.Content.ReadAsStringAsync());
                activity.JobDetails.Date = activity.JobDetails.Date.ToLocalTime();
                return activity;
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task<List<Activity>> AddActivity(params Activity[] i_Activity)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (i_Activity == null)
                {
                    throw new ArgumentNullException("Activity is null");
                }

                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/AddActivities";
                string json = ObjectJsonSerializer.SerializeForServer(i_Activity);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }

                return ObjectJsonSerializer.DeserializeForServer<List<Activity>>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public async static Task DeleteActivity(Activity i_Activity, string i_UserId = null) // delete activity from user list and from server data base
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
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
                string json = ObjectJsonSerializer.SerializeForServer(i_Activity);
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task UpdateActivity(params Activity[] i_Activity) // update activity in user list and in server data base
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/UpdateActivity";
                string json = ObjectJsonSerializer.SerializeForServer(i_Activity);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        #endregion

        #region JobOffers

        public static async Task<List<JobOffer>> GetUserJobOffers(string i_UserID)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/GetUserJobOffers";
                Dictionary<string, string> id = new Dictionary<string, string>
                {
                    ["UserId"] = i_UserID
                };

                string json = ObjectJsonSerializer.SerializeForServer(id);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                string responseMessage = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(responseMessage);
                }

                List<JobOffer> jobOffers = ObjectJsonSerializer.DeserializeForServer<List<JobOffer>>(responseMessage);
                convertDateTimeToLocalTime(jobOffers);
                return jobOffers;
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task<List<JobOffer>> GetAllJobOffers(List<eCategory> i_Categories = null)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                // if i_Categories == null than get all job offers in data base else filter by the categories
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/GetAllJobOffers";
                Dictionary<string, List<eCategory>> categoriesDictionary = new Dictionary<string, List<eCategory>>
                {
                    ["Categories"] = i_Categories
                };

                string json = ObjectJsonSerializer.SerializeForServer(categoriesDictionary);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
                List<JobOffer> jobOffers = ObjectJsonSerializer.DeserializeForServer<List<JobOffer>>(await response.Content.ReadAsStringAsync());
                convertDateTimeToLocalTime(jobOffers);
                return jobOffers;
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task<JobOffer> GetJobOffer(string i_JobId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/GetJobOffer";
                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    ["JobId"] = i_JobId
                };

                string json = ObjectJsonSerializer.SerializeForServer(data);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to get job offer.");
                }

                JobOffer jobOffer = ObjectJsonSerializer.DeserializeForServer<JobOffer>(await response.Content.ReadAsStringAsync());
                jobOffer.Date = jobOffer.Date.ToLocalTime();
                return jobOffer;
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task<List<JobOffer>> AddJobOffer(params JobOffer[] i_JobOffer)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (i_JobOffer == null)
                {
                    throw new ArgumentNullException("JobDetails is null");
                }

                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/AddJobOffer";
                string json = ObjectJsonSerializer.SerializeForServer(i_JobOffer);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }

                List<JobOffer> jobOffers = ObjectJsonSerializer.DeserializeForServer<List<JobOffer>>(await response.Content.ReadAsStringAsync());
                convertDateTimeToLocalTime(jobOffers);
                return jobOffers;
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        private static void convertDateTimeToLocalTime(List<JobOffer> i_JobOffers)
        {
            foreach (var jobOffer in i_JobOffers)
            {
                jobOffer.Date = jobOffer.Date.ToLocalTime();
            }
        }

        public static async Task DeleteJobOffer(string i_JobId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                Dictionary<string, string> data = new Dictionary<string, string>()
                {
                    ["JobId"] = i_JobId
                };

                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/DeleteJobOffer";
                string json = ObjectJsonSerializer.SerializeForServer(data);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task UpdateJobOffer(params JobOffer[] i_JobOffer) // update jobOffer in user list and in server data base
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/UpdateJobOffer";
                string json = ObjectJsonSerializer.SerializeForServer(i_JobOffer);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        #endregion

        #region Ratings

        public static async Task<List<Rating>> GetRatings(string i_UserID)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/GetRatings";
                Dictionary<string, string> id = new Dictionary<string, string>
                {
                    ["UserId"] = i_UserID
                };

                string json = ObjectJsonSerializer.SerializeForServer(id);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }

                return ObjectJsonSerializer.DeserializeForServer<List<Rating>>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task<List<Rating>> AddRating(params Rating[] i_Rating)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (i_Rating == null)
                {
                    throw new ArgumentNullException("Rating is null");
                }

                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/AddRating";
                string json = ObjectJsonSerializer.SerializeForServer(i_Rating);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }

                return ObjectJsonSerializer.DeserializeForServer<List<Rating>>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task DeleteRating(params Rating[] i_Rating) // delete rating from user list and from server data base
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (i_Rating == null)
                {
                    throw new ArgumentNullException("Rating is null");
                }

                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/DeleteRating";
                string json = ObjectJsonSerializer.SerializeForServer(i_Rating);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task UpdateRating(params Rating[] i_Rating) // update rating in user list and in server data base
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/UpdateRating";
                string json = ObjectJsonSerializer.SerializeForServer(i_Rating);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        #endregion

        #region Reports

        public static async Task AddReport(Report i_Report)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (i_Report == null)
                {
                    throw new ArgumentNullException("Report is null");
                }

                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/AddReport";
                string json = ObjectJsonSerializer.SerializeForServer(i_Report);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        #endregion

        #region User

        public static async Task<User> GetUser(string i_UserID)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string server_uri = "https://europe-west3-get-sanger.cloudfunctions.net/GetUser";
                Dictionary<string, string> details = new Dictionary<string, string>()
                {
                    ["UserId"] = i_UserID,
                };

                string json = ObjectJsonSerializer.SerializeForServer(details);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(server_uri, json, HttpMethod.Post, idToken);
                string responseJson = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(responseJson);
                }

                User user = ObjectJsonSerializer.DeserializeForServer<User>(responseJson);
                user.Activities = new ObservableCollection<Activity>(await GetActivities(i_UserID));
                user.JobOffers = new ObservableCollection<JobOffer>(await GetUserJobOffers(i_UserID));
                user.Ratings = new ObservableCollection<Rating>(await GetRatings(i_UserID));
                return user;
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task AddUser(User i_User)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string server_uri = "https://europe-west3-get-sanger.cloudfunctions.net/AddUserToDatabase";
                Dictionary<string, User> requestDictionary = new Dictionary<string, User>()
                {
                    ["User"] = i_User
                };

                string json = ObjectJsonSerializer.SerializeForServer(requestDictionary);
                string idToken = await AuthHelper.GetIdTokenAsync();

                HttpResponseMessage response = await HttpClientService.SendHttpRequest(server_uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task DeleteUser(string i_UserId)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = "https://europe-west3-get-sanger.cloudfunctions.net/DeleteUser";
                Dictionary<string, string> data = new Dictionary<string, string>
                {
                    ["UserId"] = i_UserId
                };

                string json = ObjectJsonSerializer.SerializeForServer(data);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(uri, json, HttpMethod.Post, idToken);
                AuthHelper.SignOut();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        public static async Task UpdateUser(User i_User)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string server_uri = "https://europe-west3-get-sanger.cloudfunctions.net/UpdateUser";
                string json = ObjectJsonSerializer.SerializeForServer(i_User);
                string idToken = await AuthHelper.GetIdTokenAsync();
                HttpResponseMessage response = await HttpClientService.SendHttpRequest(server_uri, json, HttpMethod.Post, idToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
        }

        #endregion
    }
}