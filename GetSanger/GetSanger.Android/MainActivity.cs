using Android.App;
using Android.Runtime;
using Android.OS;
using System.Threading.Tasks;
using System.IO;
using Android.Content;
using GetSanger.Droid.Services;
using GetSanger.Services;
using Plugin.CurrentActivity;

namespace GetSanger.Droid
{
    [Activity(Label = "GetSanger", Icon = "@mipmap/getSangerIcon", Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = Android.Content.PM.LaunchMode.SingleTask, WindowSoftInputMode = Android.Views.SoftInput.AdjustResize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static readonly string CHANNEL_ID = "notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;
        internal static MainActivity Instance { get; private set; }
        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Firebase.Crashlytics.FirebaseCrashlytics.Instance.Log("Entered OnCreate");
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Instance = this;
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            base.OnCreate(savedInstanceState);
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
        }

        protected override async void OnNewIntent(Intent intent)
        {
            Firebase.Crashlytics.FirebaseCrashlytics.Instance.Log("Entered OnNewIntent");
            base.OnNewIntent(intent);
            PushService.PushHelper(intent);
            await PushServices.handleMessageReceived(null, null, PushServices.BackgroundPushData);
        }
    }
}