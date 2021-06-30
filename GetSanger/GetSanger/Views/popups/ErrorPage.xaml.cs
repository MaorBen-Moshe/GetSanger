using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPage : PopupPage
    {
        public ErrorPage(string header, string message)
        {
            InitializeComponent();

            headerLabel.Text = header ?? "Error";
            textLabel.Text = message ?? "Something went wrong! :(";
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var top = PopupNavigation.Instance.PopupStack.First();
            if (top.GetType().Equals(this.GetType()))
            {
                PopupNavigation.Instance.PopAsync();
            }
        }
    }
}