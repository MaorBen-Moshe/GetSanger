using GetSanger.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.UI_pages.signup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPersonalDetailPage : ContentPage
    {
        public SignupPersonalDetailPage()
        {
            InitializeComponent();
        }

        async void OnPickPhotoButtonClicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;

            Stream stream = await DependencyService.Get<IPhotoPicker>().GetImageStreamAsync();
            if (stream != null)
            {
                image.Source = ImageSource.FromStream(() => stream);
            }
            else
            {
                await DisplayAlert("Error", "Something went wrong, please try again later", "Ok");
            }

            (sender as Button).IsEnabled = true;
        }
    }
}