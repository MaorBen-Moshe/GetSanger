using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetSanger.Models;

namespace GetSanger.Services
{
    public static class FireStoreHelper
    {
        public static async Task<List<Activity>> GetActivities(string i_UserID)
        {
            throw new NotImplementedException();
        }

        public static async Task<Activity> GetActivity(int i_ClientID, int i_SangerID)
        {
            throw new NotImplementedException();
        }

        public static void AddActivity(Activity i_Activity)
        {
            throw new NotImplementedException();
        }

        public static void DeleteActivity(Activity i_Activity) // delete activity from user list and from server data base
        {
            throw new NotImplementedException();
        }

        public static async void UpdateActivity(Activity i_Activity) // update activity in user list and in server data base
        {
            throw new NotImplementedException();
        }

        public static async Task<List<JobOffer>> GetJobOffers(int i_UserID)
        {
            throw new NotImplementedException();
        }

        public static void AddUser(User i_User)
        {
            throw new NotImplementedException();
        }

        public static async Task<User> GetUser(string i_UserID)
        {
            throw new NotImplementedException();
        }

        public static async void UpdateUser(User i_User)
        {
            throw new NotImplementedException();
        }
    }
}
