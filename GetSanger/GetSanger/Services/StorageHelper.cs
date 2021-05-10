﻿using GetSanger.Interfaces;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace GetSanger.Services
{
    public class StorageHelper : Service
    {
        private readonly IStorage r_Storage;
        
        public StorageHelper()
        {
            r_Storage = DependencyService.Get<IStorage>();
        }

        public async void SetUserProfileImage(User i_User, Stream i_Stream)
        {
            // check if the path is already exist if it does remove it first
            Uri imageUri = await r_Storage.UploadAndGetCloudFilePath(i_Stream, "ProfilePictures/" + i_User.UserId + ".jpg");
            i_User.ProfilePictureUri = imageUri;
            await FireStoreHelper.UpdateUser(i_User);
        }

        public async void DeleteProfileImage(string i_UserID)
        {
            await r_Storage.DeleteProfileImage("ProfilePictures/" + i_UserID + ".jpg");
        }

        public override void SetDependencies()
        {
        }
    }
}