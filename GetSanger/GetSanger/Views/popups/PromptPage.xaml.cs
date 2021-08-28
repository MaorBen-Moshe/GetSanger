using GetSanger.ViewModels;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views.popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PromptPage : PopupPage
    {
        public PromptPage(string i_Title, string i_Subtitle, string i_PlaceHolder, Action<string> i_AfterSubmit)
        {
            InitializeComponent();

            BindingContext = new PromptViewModel(i_Title, i_Subtitle, i_PlaceHolder, i_AfterSubmit);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as PopupBaseViewModel).Appearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as PopupBaseViewModel).Disappearing();
        }
    }
}