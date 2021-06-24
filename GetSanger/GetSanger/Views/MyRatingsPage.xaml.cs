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
    public partial class MyRatingsPage : ContentPage
    {
        public MyRatingsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as MyRatingsViewModel).Appearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as MyRatingsViewModel).Disappearing();
            base.OnDisappearing();
        }
    }
}