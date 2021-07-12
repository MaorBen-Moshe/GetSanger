using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GetSanger.Interfaces;

namespace GetSanger.Services
{
    public class StorageHelper : Service, IStorageHelper
    {
        public StorageHelper()
        {
        }

        public async Task SetUserProfileImage(User i_User, Stream i_Stream)
        {
            if (i_User.UserId == null)
            {
                throw new ArgumentNullException("User must have an ID");
            }

            i_Stream.Position = 0;
            byte[] bytes = new byte[i_Stream.Length];
            await i_Stream.ReadAsync(bytes, 0, bytes.Length);

            string idToken = await AuthHelper.GetIdTokenAsync();
            string requestUri = "https://europe-west3-get-sanger.cloudfunctions.net/SetUserProfilePicture";

            string json = JsonConvert.SerializeObject(new Dictionary<string, object>()
            {
                ["Picture"] = JsonConvert.SerializeObject(bytes),
                ["UserId"] = i_User.UserId
            });

            HttpResponseMessage httpResponseMessage = await HttpClientService.SendHttpRequest(requestUri, json, HttpMethod.Post, idToken);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                string responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();

                throw new Exception(responseMessage);
            }

            i_User.ProfilePictureUri = new Uri($"https://europe-west3-get-sanger.cloudfunctions.net/GetUserProfilePicture?UserId={i_User.UserId}");
        }

        public async Task DeleteProfileImage(string i_UserID)
        {
            string idToken = await AuthHelper.GetIdTokenAsync();
            string requestUri = "https://europe-west3-get-sanger.cloudfunctions.net/DeleteUserProfilePicture";

            string json = JsonConvert.SerializeObject(new Dictionary<string, object>()
            {
                ["UserId"] = i_UserID
            });

            HttpResponseMessage httpResponseMessage = await HttpClientService.SendHttpRequest(requestUri, json, HttpMethod.Delete, idToken);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                string responseMessage = await httpResponseMessage.Content.ReadAsStringAsync();

                throw new Exception(responseMessage);
            }
        }

        public override void SetDependencies()
        {
        }
    }
}