using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GetSanger.Interfaces;
using Xamarin.Forms;
using GetSanger.Droid.Services;

[assembly: Dependency(typeof(GetSanger.Droid.Services.PushService))]
namespace GetSanger.Droid.Services
{
    class PushService : IPushService
    { 
        public void TempMethod(string token)
        {
            throw new NotImplementedException();
        }
    }
}