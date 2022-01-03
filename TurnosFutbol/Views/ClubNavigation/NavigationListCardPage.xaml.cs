using TurnosFutbol.DataService;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.ClubNavigation
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationListCardPage
    {
        public NavigationListCardPage()
        {
            InitializeComponent();
            this.BindingContext = NavigationDataService.Instance.NavigationViewModel;
        }
    }
}