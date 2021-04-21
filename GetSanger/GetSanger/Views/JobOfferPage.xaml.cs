using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GetSanger.ViewModels;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobOfferPage : ContentPage
    {
        public JobOfferPage()
        {
            InitializeComponent();

            BindingContext = new JobOfferViewModel();
        }
    }
}