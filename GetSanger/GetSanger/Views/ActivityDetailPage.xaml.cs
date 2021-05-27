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
    public partial class ActivityDetailPage : ContentPage
    {
        public ActivityDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as BaseViewModel).Appearing();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as ActivityViewModel).Disappearing();
            base.OnDisappearing();
        }
    }
}