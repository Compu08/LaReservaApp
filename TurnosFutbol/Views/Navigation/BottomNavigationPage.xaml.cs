using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Navigation
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomNavigationPage : TabbedPage
    {
        public BottomNavigationPage()
        {
            InitializeComponent();
            if (Preferences.Get("registered", false))
            {
                //ResetNavigationStack();
                Preferences.Set("registered", true);
            }
        }
        public void ResetNavigationStack()
        {
            Page currentPage = Navigation.NavigationStack[Navigation.NavigationStack.Count - 1];
            System.Collections.Generic.List<Page> pageList = Navigation.NavigationStack.Where(y => y != currentPage).ToList();

            foreach (Page page in pageList)
            {
                Navigation.RemovePage(page);
            }
        }
    }
}