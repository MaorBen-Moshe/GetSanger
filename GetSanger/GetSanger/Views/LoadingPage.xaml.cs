﻿using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage(string text = null)
        {
            InitializeComponent();

            loadingText.Text = text ?? "Loading...";
        }
    }
}