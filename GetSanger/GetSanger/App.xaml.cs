﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GetSanger.UI_pages.signup;
using GetSanger.UI_pages;
using GetSanger.UI_pages.common;

namespace GetSanger
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new JobOfferPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
