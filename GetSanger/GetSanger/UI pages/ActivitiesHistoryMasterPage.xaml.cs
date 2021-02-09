using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.UI_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivitiesHistoryMasterPage : ContentPage
    {
        public ActivitiesHistoryMasterPage()
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
            if(e.SelectedItem == null)
                return;

            // var activity = e.SelectedItem as Activity;

            await Navigation.PushAsync(new ActivitiesHistoryDetailPage()); // send the activity in the constructor and set it to be binding context
            m_ListView.SelectedItem = null;
        }
    }
}