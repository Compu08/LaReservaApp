using Syncfusion.DataSource;
using Syncfusion.ListView.XForms;
using TurnosFutbol.Models;
using Xamarin.Forms;


namespace TurnosFutbol.Helper
{
    public class Behavior : Behavior<SfListView>
    {
        SfListView ListView;
        protected override void OnAttachedTo(SfListView bindable)
        {
            ListView = bindable;
            ListView.DataSource.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "ContactName",
                Direction = ListSortDirection.Ascending
            });

            ListView.DataSource.GroupDescriptors.Add(new GroupDescriptor()
            {
                PropertyName = "ContactName",
                KeySelector = (object obj1) =>
                {
                    Contacts item = (obj1 as Contacts);
                    return item.ContactName[0].ToString();
                },
            });
            base.OnAttachedTo(bindable);
        }
    }
}
