using GetSanger.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class LoadingPageViewModel : BaseViewModel
    {
        #region Fields
        private string m_Text;
        private Color m_TextColor;
        private bool m_NoInternetPage;
        #endregion

        #region Properties
        public string Text
        {
            get => m_Text;
            set => SetClassProperty(ref m_Text, value);
        }

        public Color TextColor
        {
            get => m_TextColor;
            set => SetStructProperty(ref m_TextColor, value);
        }
        #endregion

        #region Commands
        public ICommand NoInternetTapCommand { get; set; }
        #endregion

        #region Constructor
        public LoadingPageViewModel(string i_Text = "Loading...", bool i_NoInternetPage = false)
        {
            Text = i_NoInternetPage ? "No Internet\nTap to try again" : i_Text;
            TextColor = i_NoInternetPage ? Color.WhiteSmoke : Color.Black;
            m_NoInternetPage = i_NoInternetPage;
            if (i_NoInternetPage)
            {
                NoInternetTapCommand = new Command(tapped);
            }
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            if (m_NoInternetPage)
            {
                Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            }
        }

        public void Disappearing()
        {
            if (m_NoInternetPage)
            {
                Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            }
        }

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess.Equals(NetworkAccess.Internet))
            {
                r_PopupService.HidePopup(typeof(LoadingPage));
                Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            }
        }

        private void tapped(object i_Param)
        {
            if (Connectivity.NetworkAccess.Equals(NetworkAccess.Internet))
            {
                r_PopupService.HidePopup(typeof(LoadingPage));
                Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            }
        }
        #endregion
    }
}
