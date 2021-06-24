﻿using GetSanger.ViewModels;
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
    public partial class JobOffersListView : ContentPage
    {
        public JobOffersListView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as JobOffersViewModel).Appearing();
            searchHandler.JobOffers = (BindingContext as JobOffersViewModel).Collection;
            base.OnAppearing();
        }
    }
}