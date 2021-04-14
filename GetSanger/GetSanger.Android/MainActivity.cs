using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Gms.Common;
using ImageCircle.Forms.Plugin.Droid;
using System.Threading.Tasks;
using System.IO;
using Android.Content;

//TEMPORARY
using Firebase.Messaging;

namespace GetSanger.Droid
{
    [Activity(Label = "GetSanger", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static readonly string TAG = "MainActivity";
        internal static readonly string CHANNEL_ID = "notification_channel";

        internal static readonly int NOTIFICATION_ID = 100;
        internal static MainActivity Instance { get; private set; }

        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { set; get; }

        protected override void OnCreate(Bundle savedInstanceState)
        {

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState);
            ImageCircleRenderer.Init();
            LoadApplication(new App());
            Services.PushService pushService = new Services.PushService();
            pushService.PushHelper(Intent, this);

            //TEMPORARY
            FirebaseMessaging.Instance.SubscribeToTopic("Topic");
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnPause()
        {
            base.OnPause();
            Instance = this;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}