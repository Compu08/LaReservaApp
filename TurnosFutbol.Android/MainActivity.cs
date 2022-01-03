
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Util;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Iid;
using Firebase.Messaging;
using Java.IO;
using Plugin.FirebasePushNotification;
using Syncfusion.XForms.Themes;
using Xamarin.Essentials;

namespace TurnosFutbol.Droid
{
    [Activity(Label = "LaReserva", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            ///Initialize Popup Plugin
            Rg.Plugins.Popup.Popup.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Firebase.FirebaseApp.InitializeApp(ApplicationContext);

            InitFontScale();

            LoadApplication(new App());

            FirebasePushNotificationManager.Initialize(this,false,true,true);

            FirebasePushNotificationManager.ProcessIntent(this, Intent);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void InitFontScale()
        {
            var configuration = Resources?.Configuration;
            if (configuration == null) return;
            configuration.FontScale = (float) 1.00;
            var metrics = new DisplayMetrics();
            WindowManager?.DefaultDisplay?.GetMetrics(metrics);

            metrics.ScaledDensity = configuration.FontScale * metrics.Density;
            if (BaseContext == null) return;
            BaseContext.ApplicationContext?.CreateConfigurationContext(configuration);
            BaseContext.Resources?.DisplayMetrics?.SetTo(metrics);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            if (Xamarin.Forms.Application.Current.RequestedTheme == Xamarin.Forms.OSAppTheme.Dark)
            {
                this.SetTheme(Resource.Style.NightTheme);
            }
            else
            {
                this.SetTheme(Resource.Style.MainTheme);
            }
        }

    }
}