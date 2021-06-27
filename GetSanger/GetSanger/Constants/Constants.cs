using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Constants
{
    public static class Constants
    {
        public static string MapsApiKey { get; } = "AIzaSyCtfUx4NamOLsRFRZ0cMrh-cG-C0K_JTVA";

        public static string GenericNotificationTopic { get; } = "Generic";

        public static string GetSangerMail { get; } = "GetSanger@gmail.com";

        public static string GetSangerMailPassword { get; } = "$MRGS44$";

        public static string FirebaseStorage = "gs://get-sanger.appspot.com";

        public static string LocationMessage = "Location";

        public static string ActivatedLocationMessage = "ActivatedLocation";

        public static string AddRatingMessage = "AddRating";

        public static string UsersDB = "UsersFile";

       // public static int MaxDescriptionEditorLength = 100;
    }
}
