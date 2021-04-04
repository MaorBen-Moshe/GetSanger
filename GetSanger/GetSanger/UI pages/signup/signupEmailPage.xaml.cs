using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetSanger.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.UI_pages.signup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupEmailPage : ContentPage
    {
        public SignupEmailPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (password.Text.Equals(confirm.Text))
                {
                    await AuthHelper.RegisterViaEmail(email.Text, password.Text);
                    await DisplayAlert("Note", "Success", "Ok");
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }
    }
}