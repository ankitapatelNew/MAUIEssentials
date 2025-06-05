using System;
using Android.Content;
using Firebase;
using FirebaseEssentials.Shared;

namespace FirebaseEssentials.Platforms.Android
{
    public static class FirebaseEssentialsManager
    {
        public static void Initialize(Context context)
        {
            FirebaseApp.InitializeApp(context);

            if (CrossFirebaseEssentials.Notifications != null)
            {
                FirebasePushNotificationManager.Initialize(context, false);
            }
        }
    }
}