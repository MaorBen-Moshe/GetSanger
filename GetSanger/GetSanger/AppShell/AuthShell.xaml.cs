﻿using GetSanger.Services;
using GetSanger.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.AppShell
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthShell : Shell
    {
        public AuthShell()
        {
            InitializeComponent();

            AppManager.Instance.SignUpVM = new SignUpPageViewModel();
        }
    }
}