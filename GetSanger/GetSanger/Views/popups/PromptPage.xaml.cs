using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PromptPage : PopupPage
    {
        public PromptPage(string i_Title, string i_Subtitle, string i_PlaceHolder, Action<string> i_AfterSubmit)
        {
            InitializeComponent();
            title.Text = i_Title;
            subTitle.Text = i_Subtitle;
            editor.Placeholder = i_PlaceHolder;
            submit.Command = new Command(() => 
            {
                i_AfterSubmit.Invoke(editor.Text);
                PopupNavigation.Instance.PopAsync();
            });
        }

        protected override void OnAppearing()
        {
            if (Device.RuntimePlatform.Equals(Device.Android))
            {
                Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            }

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            if (Device.RuntimePlatform.Equals(Device.Android))
            {
                Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Pan);
            }

            base.OnDisappearing();
        }
    }
}