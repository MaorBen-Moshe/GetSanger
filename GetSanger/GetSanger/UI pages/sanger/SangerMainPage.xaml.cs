﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace GetSanger.UI_pages.sanger
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SangerMainPage : Xamarin.Forms.TabbedPage
    {
        public SangerMainPage()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }
    }
}