using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using Firebase.Messaging;
using Xamarin.Essentials;

namespace TurnosFutbol.Droid
{
    [Service (Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseIIDService : FirebaseMessagingService
    {
        const string TAG = "Firebase";

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            SendRegistrationToServer(token);
        }

        void SendRegistrationToServer(string token)
        {
            //Preferences.Set("api", token);
        }
    }
}
