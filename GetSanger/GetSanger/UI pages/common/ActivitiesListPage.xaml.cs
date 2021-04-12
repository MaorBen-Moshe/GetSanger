using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.UI_pages.common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivitiesListPage : ContentPage
    {
        public ActivitiesListPage()
        {
            InitializeComponent();
        }

        private void ListView_Refreshing(object sender, EventArgs e)
        {
            // server call and get activities

            m_ListView.EndRefresh();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            // var activity = e.SelectedItem as Activity;

            await Navigation.PushAsync(new ActivityDetailPage()); // send the activity in the constructor and set it to be binding context
            m_ListView.SelectedItem = null;
        }
    }
}