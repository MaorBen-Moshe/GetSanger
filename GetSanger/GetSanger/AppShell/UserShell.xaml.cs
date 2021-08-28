using GetSanger.Constants;
using GetSanger.Views;
using GetSanger.Views.chat;
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
            r_Routes.Add(ShellRoutes.Ratings, typeof(RatingsPage));
            r_Routes.Add(ShellRoutes.ChatView, typeof(ChatView));

            foreach (var route in r_Routes)
            {
                Routing.RegisterRoute(route.Key, route.Value);
            }
        }
    }
}