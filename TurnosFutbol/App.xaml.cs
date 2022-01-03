using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Firebase.Database.Query;
using Plugin.FirebasePushNotification;
using Rg.Plugins.Popup.Services;
using Syncfusion.XForms.Themes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TurnosFutbol
{
    public partial class App : Application
    {
        public static string BaseImageUrl { get; } = "https://cdn.syncfusion.com/essential-ui-kit-for-xamarin.forms/common/uikitimages/";
        public static Theme AppTheme { get; set; }
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDEzODI4QDMxMzgyZTM0MmUzMFRzZnlVaTV5UXpmT1dSUjZkcjBUZS8wLzhDa2hId0FYeVp4UXJWMEJNYmc9");


            InitializeComponent();

            bool registered = Preferences.Get("registered", false);
            bool nightMode = Preferences.Get("NightMode", false);
            SetTheme(nightMode);

            MainPage = registered ? new NavigationPage(new TurnosFutbol.Views.Navigation.BottomNavigationPage()) : new NavigationPage(new TurnosFutbol.Views.Forms.LoginPage());


        }

        protected override void OnStart()
        {
            base.OnStart();
            CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            {
                System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}");
                Preferences.Set("api_id", p.Token);
            };

            CrossFirebasePushNotification.Current.RegisterForPushNotifications();
            CrossFirebasePushNotification.Current.Subscribe("all");


        }

        protected override void OnSleep()
        {
            
        }

        protected override void OnResume()
        {

        }

        public enum Theme
        {
            Light,
            Dark
        }

        void SetTheme(bool status)
        {
            var themeRequested = status ? Theme.Dark : Theme.Light;

            DependencyService.Get<IAppTheme>().SetAppTheme(themeRequested);
        }
        
    }
}
