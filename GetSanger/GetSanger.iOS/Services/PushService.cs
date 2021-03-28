using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using GetSanger.Interfaces;
using Xamarin.Forms;
using GetSanger.iOS.Services;

[assembly: Dependency(typeof(GetSanger.iOS.Services.PushService))]
namespace GetSanger.iOS.Services
{
    class PushService : IPushService
    {
        public void TempMethod(string token)
        {
            throw new NotImplementedException();
        }
    }
}