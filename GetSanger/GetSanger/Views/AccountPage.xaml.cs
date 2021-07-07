using GetSanger.Services;
using GetSanger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as BaseViewModel).Appearing();
            Label label = AppManager.Instance.CurrentMode.Equals(eAppMode.Client) ? ClientLabel : SangerLabel;
            label.BackgroundColor = Color.Red;
            base.OnAppearing();
        }
    }
}