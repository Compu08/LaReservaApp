using Rg.Plugins.Popup.Services;
using Syncfusion.ListView.XForms;
using TurnosFutbol.DataService;
using TurnosFutbol.ViewModels.Navigation;
using TurnosFutbol.Views.Popups;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Notification
{
    /// <summary>
    /// Page to display E-Commerce notifications list.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ECommerceNotificationPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ECommerceNotificationPage" /> class.
        /// </summary>
        public ECommerceNotificationPage()
        {
            InitializeComponent();
            PopupNavigation.Instance.PushAsync(new Loading());
            lstview.BindingContext = new NotificationsViewModel();
         //   if (lstview.DataSource.Items.Count == 0) LabelEmpty.IsVisible = true;
            PopupNavigation.Instance.PopAsync();
           
        }

        private void GoBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OpenNotificationPopup(object sender, ItemTappedEventArgs e)
        {
            if (e.ItemData is Models.Navigation.Notification obj)
                PopupNavigation.Instance.PushAsync(new NotificationPopup(obj.Title, obj.Description, obj.TimeDif,obj.Place, obj.Topic));
        }
    }
}