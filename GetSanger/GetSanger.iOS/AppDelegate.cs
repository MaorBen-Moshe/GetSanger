using Foundation;
using UIKit;
using Firebase.Core;
using UserNotifications;
using Firebase.CloudMessaging;
using System;

namespace GetSanger.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // Client Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (Xamarin.Essentials.Platform.OpenUrl(app, url, options))
                return true;

            return base.OpenUrl(app, url, options);
        }

        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
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
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            Xamarin.FormsGoogleMaps.Init(Constants.Constants.MapsApiKey);
            Messaging.SharedInstance.ShouldEstablishDirectChannel = true;
            LoadApplication(new App());
            Firebase.Core.App.Configure();
            Messaging.SharedInstance.Delegate = this as Firebase.CloudMessaging.IMessagingDelegate;

            RegisterForRemoteNotifications();

            //TEMPORARY
            Messaging.SharedInstance.Subscribe("Topic");

            // force Right to left flow direction in app
            ObjCRuntime.Selector selector = new ObjCRuntime.Selector("setSemanticContentAttribute:");

            return base.FinishedLaunching(app, options); ;
        }

        internal void RegisterForRemoteNotifications()
        {
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this as UserNotifications.IUNUserNotificationCenterDelegate;

                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {
                    Console.WriteLine("in RegisterForRemoteNotifications, {1}", granted);
                });
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string newToken)
        {
            string oldToken = Services.PushService.FCMToken;
            Services.PushService.FCMToken = newToken;
            SendRegistrationToServer(newToken, oldToken); 

            var token = Messaging.SharedInstance.FcmToken ?? "";
            Console.WriteLine($"Current FCM token: {token}");
        }

        // For when Method Swizzling is disabled
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Messaging.SharedInstance.ApnsToken = deviceToken;
        }

        internal void SendRegistrationToServer(string newToken, string oldToken)
        {
            // Send token to server if needed
            // Server should resubscribe the user(token) to the previous topics he was subscribed to
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired till the user taps on the notification launching the application.
            // TODO: Handle data of notification

            // With swizzling disabled you must let Messaging know about the message, for Analytics
            //Messaging.SharedInstance.AppDidReceiveMessage (userInfo);

            // Print full message.
            Console.WriteLine(userInfo);
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired till the user taps on the notification launching the application.
            // TODO: Handle data of notification

            // With swizzling disabled you must let Messaging know about the message, for Analytics
            //Messaging.SharedInstance.AppDidReceiveMessage (userInfo);

            // Print full message.
            Console.WriteLine(userInfo);

            Messaging.SharedInstance.AppDidReceiveMessage(userInfo);
            completionHandler(UIBackgroundFetchResult.NewData);
        }

        [Foundation.Export("didReceiveMessage:conversation:")]
        public virtual void DidReceiveMessage(Messages.MSMessage message, Messages.MSConversation conversation)
        {// Handles messages while in the foreground
            
        }

        // Receive displayed notifications for iOS 10 devices.
        // Handle incoming notification messages while app is in the foreground.
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            var userInfo = notification.Request.Content.UserInfo;

            // With swizzling disabled you must let Messaging know about the message, for Analytics
            //Messaging.SharedInstance.AppDidReceiveMessage (userInfo);

            // Print full message.
            Console.WriteLine(userInfo);

            // Change this to your preferred presentation option
            completionHandler(UNNotificationPresentationOptions.None);
        }

        // Handle notification messages after display notification is tapped by the user.
        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var userInfo = response.Notification.Request.Content.UserInfo;

            // Print full message.
            Console.WriteLine(userInfo);

            completionHandler();
        }



    }

}
