﻿using GetSanger.Constants;
using GetSanger.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.AppShell
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserShell : Shell
    {
        private Dictionary<string, Type> r_Routes;

        public UserShell()
        {
            InitializeComponent();
            r_Routes = new Dictionary<string, Type>();
            registerRoutes();
        }

        private void registerRoutes()
        {
            r_Routes.Add(ShellRoutes.Settings, typeof(SettingPage));
            r_Routes.Add(ShellRoutes.Profile, typeof(ProfileViewPage));
            r_Routes.Add(ShellRoutes.Map, typeof(MapPage));
            r_Routes.Add(ShellRoutes.Activity, typeof(ActivityDetailPage));
            r_Routes.Add(ShellRoutes.JobOffer, typeof(JobOfferPage));
            r_Routes.Add(ShellRoutes.EditProfile, typeof(EditProfilePage));
            r_Routes.Add(ShellRoutes.LoadingPage, typeof(LoadingPage));
            r_Routes.Add(ShellRoutes.ChangePassword, typeof(ChangePasswordPage));
            r_Routes.Add(ShellRoutes.AddRating, typeof(AddRatingPage));
            r_Routes.Add(ShellRoutes.SangerNotes, typeof(SangerNotesView));

            foreach (var route in r_Routes)
            {
                Routing.RegisterRoute(route.Key, route.Value);
            }
        }
    }
}