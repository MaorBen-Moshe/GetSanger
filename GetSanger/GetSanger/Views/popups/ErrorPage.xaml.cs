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
        private readonly Action<bool> r_UserChoseOptionAction;

        public ErrorPage(string header, 
                         string message,
                         string okText = "OK",
                         string cancelText = null,
                         Action<bool> action = null)
        {
            InitializeComponent();

            CloseWhenBackgroundIsClicked = false;
            headerLabel.Text = header ?? "Error";
            textLabel.Text = message ?? "Something went wrong! :(";
            okLabel.Text = okText;
            cancelLabel.Text = cancelText;
            cancelLabel.IsVisible = cancelText != null;
            r_UserChoseOptionAction = action;
        }

        private void OkTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            tapHelper(true);
        }

        private void CancelTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            tapHelper(false);
        }

        private async void tapHelper(bool okClicked)
        {
            await PopupNavigation.Instance.PopAsync();
            r_UserChoseOptionAction?.Invoke(okClicked);
        }
    }
}