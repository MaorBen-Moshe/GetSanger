using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Firebase.CloudMessaging;
using Firebase.InstanceID;
using ObjCRuntime;
using UserNotifications;

namespace GetSanger.iOS.Push
{
    public class MessagingDelegate
    {
        //public IntPtr Handle { get; }

        //public void Dispose()
        //{
        //}

        //void InstanceIdResultHandler(InstanceIdResult result, NSError error)
        //{
        //    if (error != null)
        //    {
        //        LogInformation(nameof(InstanceIdResultHandler), $"Error: {error.LocalizedDescription}");
        //        return;
        //    }

        //    LogInformation(nameof(InstanceIdResultHandler), $"Remote Instance Id token: {result.Token}");
        //}

        //[Export("messaging:didReceiveRegistrationToken:")]
        //public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        //{
        //    // Monitor token generation: To be notified whenever the token is updated.

        //    LogInformation(nameof(DidReceiveRegistrationToken), $"Firebase registration token: {fcmToken}");

        //    // TODO: If necessary send token to application server.
        //    // Note: This callback is fired at each app startup and whenever a new token is generated.
        //}

        //// You'll need this method if you set "FirebaseAppDelegateProxyEnabled": NO in GoogleService-Info.plist
        ////public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
        ////{
        ////	Messaging.SharedInstance.ApnsToken = deviceToken;
        ////}


        //[Export("messaging:didReceiveMessage:")]
        //public void DidReceiveMessage(Messaging messaging, RemoteMessage remoteMessage)
        //{
        //    // Handle Data messages for iOS 10 and above.

        //    LogInformation(nameof(DidReceiveMessage), remoteMessage.AppData);
        //}

        //void LogInformation(string methodName, object information) => Console.WriteLine($"\nMethod name: {methodName}\nInformation: {information}");

        //public void RegisterForRemoteNotifications()
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
    }
}