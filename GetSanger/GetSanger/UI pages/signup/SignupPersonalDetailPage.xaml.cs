﻿using GetSanger.Interfaces;
using GetSanger.Services;
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

            BindingContext = AppManager.Instance.SignUpVM;
        }
    }
}