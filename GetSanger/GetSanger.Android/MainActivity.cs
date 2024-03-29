﻿using Android.App;
using Android.Runtime;
using Android.OS;
using System.Threading.Tasks;
using System.IO;
using Android.Content;
using GetSanger.Droid.Services;
using GetSanger.Services;
using Plugin.CurrentActivity;
using Android.Content.PM;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps.Android;
using CarouselView.FormsPlugin.Droid;

namespace GetSanger.Droid
{
    [Activity(Label = "Get Sanger", Icon = "@mipmap/getSangerIcon", Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.ColorMode | ConfigChanges.UiMode | ConfigChanges.ScreenLayout, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }
        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Forms.Init(this, savedInstanceState);
            Firebase.Crashlytics.FirebaseCrashlytics.Instance.Log("Entered OnCreate");
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Instance = this;
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this);
            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState, new PlatformConfig
            {
                BitmapDescriptorFactory = new BitmapConfig()
            });

            CarouselViewRenderer.Init();
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            PushService.InitializePushService(this);
            PushService.PushHelper(Intent);
            LoadApplication(new App());
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnResume()
        {
            base.OnResume();
            Instance = this;
        }

        protected override void OnPause()
        {
            base.OnPause();
            Instance = this;
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnBackPressed()
        {
            Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (intent != null))
                {
                    Android.Net.Uri uri = intent.Data;
                    Stream stream = ContentResolver.OpenInputStream(uri);

                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(stream);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }

            Auth.FacebookCallbackManager.OnActivityResult(requestCode, (int)resultCode, intent);
        }

        protected override async void OnNewIntent(Intent intent)
        {
            Firebase.Crashlytics.FirebaseCrashlytics.Instance.Log("Entered OnNewIntent");
            base.OnNewIntent(intent);
            PushService.PushHelper(intent);
            await PushServices.HandleMessageReceived(null, null, PushServices.BackgroundPushData);
        }
    }
}