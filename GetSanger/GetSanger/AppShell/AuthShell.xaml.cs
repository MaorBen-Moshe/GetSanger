using GetSanger.Services;
using GetSanger.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GetSanger.Constants;
using GetSanger.UI_pages.signup;
using GetSanger.UI_pages.common;
using System.Linq;

namespace GetSanger.AppShell
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthShell : Shell
    {
        private readonly Dictionary<string, Type> r_Routes;

        public AuthShell()
        {
            InitializeComponent();

            r_Routes = new Dictionary<string, Type>();
            AppManager.Instance.SignUpVM = new SignUpPageViewModel();
            registerRoutes();
        }

        private void registerRoutes()
        {
            r_Routes.Add(ShellRoutes.Login, typeof(RegistrationPage));
            r_Routes.Add(ShellRoutes.SignupEmail, typeof(SignupEmailPage));
            r_Routes.Add(ShellRoutes.SignupPersonalDetails, typeof(SignupPersonalDetailPage));
            r_Routes.Add(ShellRoutes.SignupCategories, typeof(SignupCategoriesPage));
            r_Routes.Add(ShellRoutes.ModePage, typeof(ModePage));
            r_Routes.Add(ShellRoutes.ForgotPassword, typeof(ForgotPasswordPage));
            r_Routes.Add(ShellRoutes.LoadingPage, typeof(LoadingPage));

            foreach(var route in r_Routes)
            {
                Routing.RegisterRoute(route.Key, route.Value); 
            }
        }
    }
}