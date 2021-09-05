using System;
using Foundation;
using UIKit;
using Firebase.CloudMessaging;
using Firebase.InstanceID;
using GetSanger.iOS.Services;
using UserNotifications;
using CarouselView.FormsPlugin.iOS;

namespace GetSanger.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // Client Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate,
        IMessagingDelegate
    {
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (Xamarin.Essentials.Platform.OpenUrl(app, url, options))
                return true;

            return base.OpenUrl(app, url, options);
        }

        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity,
            UIApplicationRestorationHandler completionHandler)
        {
            if (Xamarin.Essentials.Platform.ContinueUserActivity(application, userActivity, completionHandler))
                return true;
            return base.ContinueUserActivity(application, userActivity, completionHandler);
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            CarouselViewRenderer.Init();
            Xamarin.FormsGoogleMaps.Init(Constants.Constants.MapsApiKey);
            Firebase.Core.App.Configure();
            Firebase.Crashlytics.Crashlytics.SharedInstance.Init();
            Firebase.Auth.Auth.DefaultInstance.Init();
            Messaging.SharedInstance.Init();
            RegisterForRemoteNotifications();
            LoadApplication(new App());

            //TEMPORARY
            Messaging.SharedInstance.Subscribe("Topic");

            // force Right to left flow direction in app
            ObjCRuntime.Selector selector = new ObjCRuntime.Selector("setSemanticContentAttribute:");

            return base.FinishedLaunching(app, options);
        }

        void InstanceIdResultHandler(InstanceIdResult result, NSError error)
        {
            if (error != null)
            {
                LogInformation(nameof(InstanceIdResultHandler), $"Error: {error.LocalizedDescription}");
                return;
            }

            LogInformation(nameof(InstanceIdResultHandler), $"Remote Instance Id token: {result.Token}");
        }

        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
            // Monitor token generation: To be notified whenever the token is updated.

            LogInformation(nameof(DidReceiveRegistrationToken), $"Firebase registration token: {fcmToken}");

            // TODO: If necessary send token to application server.
            // Note: This callback is fired at each app startup and whenever a new token is generated.
        }

        // You'll need this method if you set "FirebaseAppDelegateProxyEnabled": NO in GoogleService-Info.plist
        //public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
        //{
        //	Messaging.SharedInstance.ApnsToken = deviceToken;
        //}


        [Export("messaging:didReceiveMessage:")]
        public void DidReceiveMessage(Messaging messaging, RemoteMessage remoteMessage)
        {
            // Handle Data messages for iOS 10 and above.

            NSDictionary messageAppData = remoteMessage.AppData;
            LogInformation(nameof(DidReceiveMessage), messageAppData);

            PushService.PushHelper(messageAppData);
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo,
            Action<UIBackgroundFetchResult> completionHandler)
        {
            // Handle Notification messages in the background and foreground.
            // Handle Data messages for iOS 9 and below.

            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired till the user taps on the notification launching the application.
            // TODO: Handle data of notification

            // With swizzling disabled you must let Messaging know about the message, for Analytics
            //Messaging.SharedInstance.AppDidReceiveMessage (userInfo);


            // Print full message.
            LogInformation(nameof(DidReceiveRemoteNotification), userInfo);

            PushService.PushHelper(userInfo);

            completionHandler(UIBackgroundFetchResult.NewData);
        }

        void LogInformation(string methodName, object information) => Console.WriteLine($"\nMethod name: {methodName}\nInformation: {information}");

        public void RegisterForRemoteNotifications()
        {
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => { Console.WriteLine(granted); });
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            Messaging.SharedInstance.Delegate = this;

            InstanceId.SharedInstance.GetInstanceId(InstanceIdResultHandler);
        }

        //internal void RegisterForRemoteNotifications()
        //{
        //    // Register your app for remote notifications.
        //    if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
        //    {
        //        // For iOS 10 display notification (sent via APNS)
        //        UNUserNotificationCenter.Current.Delegate = this;

        //        var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
        //        UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => { Console.WriteLine(granted); });
        //    }
        //    else
        //    {
        //        // iOS 9 or before
        //        var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
        //        var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
        //        UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
        //    }

        //    UIApplication.SharedApplication.RegisterForRemoteNotifications();

        //    Messaging.SharedInstance.Delegate = this;

        //    InstanceId.SharedInstance.GetInstanceId(InstanceIdResultHandler);
        //}

        //[Export("messaging:didReceiveRegistrationToken:")]
        //public void DidReceiveRegistrationToken(Messaging messaging, string newToken)
        //{
        //    string oldToken = Services.PushService.FCMToken;
        //    Services.PushService.FCMToken = newToken;
        //    SendRegistrationToServer(newToken, oldToken); 

        //    var token = Messaging.SharedInstance.FcmToken ?? "";
        //    Console.WriteLine($"Current FCM token: {token}");
        //}

        //// For when Method Swizzling is disabled
        //public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        //{
        //    Messaging.SharedInstance.ApnsToken = deviceToken;
        //}

        //internal void SendRegistrationToServer(string newToken, string oldToken)
        //{
        //    // Send token to server if needed
        //    // Server should resubscribe the user(token) to the previous topics he was subscribed to
        //}

        //public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        //{
        //    // If you are receiving a notification message while your app is in the background,
        //    // this callback will not be fired till the user taps on the notification launching the application.
        //    // TODO: Handle data of notification

        //    // With swizzling disabled you must let Messaging know about the message, for Analytics
        //    //Messaging.SharedInstance.AppDidReceiveMessage (userInfo);

        //    // Print full message.
        //    Console.WriteLine(userInfo);
        //}

        //public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        //{
        //    // If you are receiving a notification message while your app is in the background,
        //    // this callback will not be fired till the user taps on the notification launching the application.
        //    // TODO: Handle data of notification

        //    // With swizzling disabled you must let Messaging know about the message, for Analytics
        //    //Messaging.SharedInstance.AppDidReceiveMessage (userInfo);

        //    // Print full message.
        //    Console.WriteLine(userInfo);

        //    Messaging.SharedInstance.AppDidReceiveMessage(userInfo);
        //    completionHandler(UIBackgroundFetchResult.NewData);
        //}

        //[Foundation.Export("didReceiveMessage:conversation:")]
        //public virtual void DidReceiveMessage(Messages.MSMessage message, Messages.MSConversation conversation)
        //{// Handles messages while in the foreground

        //}

        //// Receive displayed notifications for iOS 10 devices.
        //// Handle incoming notification messages while app is in the foreground.
        //[Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        //public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        //{
        //    var userInfo = notification.Request.Content.UserInfo;

        //    // With swizzling disabled you must let Messaging know about the message, for Analytics
        //    //Messaging.SharedInstance.AppDidReceiveMessage (userInfo);

        //    // Print full message.
        //    Console.WriteLine(userInfo);

        //    // Change this to your preferred presentation option
        //    completionHandler(UNNotificationPresentationOptions.None);
        //}

        //// Handle notification messages after display notification is tapped by the user.
        //[Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        //public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        //{
        //    var userInfo = response.Notification.Request.Content.UserInfo;

        //    // Print full message.
        //    Console.WriteLine(userInfo);

        //    completionHandler();
        //}
    }
}