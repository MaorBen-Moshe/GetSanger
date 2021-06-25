using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views;
using GetSanger.Views.chat;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
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
            CurrentItem = CategoriesPage;
            r_Routes = new Dictionary<string, Type>();
            registerRoutes();
        }

        private void registerRoutes()
        {
            r_Routes.Add(ShellRoutes.Settings, typeof(SettingPage));
            r_Routes.Add(ShellRoutes.Profile, typeof(ProfileViewPage));
            r_Routes.Add(ShellRoutes.Map, typeof(MapPage));
            r_Routes.Add(ShellRoutes.Activity, typeof(ActivityDetailPage));
            r_Routes.Add(ShellRoutes.EditJobOffer, typeof(EditJobOfferPage));
            r_Routes.Add(ShellRoutes.ViewJobOffer, typeof(ViewJobOfferPage));
            r_Routes.Add(ShellRoutes.EditProfile, typeof(EditProfilePage));
            r_Routes.Add(ShellRoutes.LoadingPage, typeof(LoadingPage));
            r_Routes.Add(ShellRoutes.ChangePassword, typeof(ChangePasswordPage));
            r_Routes.Add(ShellRoutes.AddRating, typeof(AddRatingPage));
            r_Routes.Add(ShellRoutes.SangerNotes, typeof(SangerNotesView));
            r_Routes.Add(ShellRoutes.LinkEmail, typeof(LinkEmailPage));
            r_Routes.Add(ShellRoutes.MyRatings, typeof(MyRatingsPage));
            r_Routes.Add(ShellRoutes.ChatView, typeof(ChatView));

            foreach (var route in r_Routes)
            {
                Routing.RegisterRoute(route.Key, route.Value);
            }
        }
    }
}