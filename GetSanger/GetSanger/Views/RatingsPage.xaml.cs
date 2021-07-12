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
    public partial class RatingsPage : ContentPage
    {
        public RatingsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as RatingsViewModel).Appearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as BaseViewModel).Disappearing();
            base.OnDisappearing();
        }
    }
}