using GetSanger.Interfaces;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class LinkProviderViewModel : BaseViewModel
    {
        #region Fields
        private List<SocialProvider> m_Providers;
        private SocialProvider m_CurrentProvider;
        #endregion

        #region Properties
        public List<SocialProvider> Providers
        {
            get => m_Providers;
            set => SetClassProperty(ref m_Providers, value);
        }

        public SocialProvider CurrentProvider
        {
            get => m_CurrentProvider;
            set => SetStructProperty(ref m_CurrentProvider, value);
        }
        #endregion

        #region Commands

        public ICommand ProviderSelectedCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        #region Constructor
        public LinkProviderViewModel()
        {
            setCommands();
        }

        #endregion

        #region Methods

        public override void Appearing()
        {
            Providers = AppManager.Instance.GetListOfEnumNames(typeof(SocialProvider)).Select(item => (SocialProvider)Enum.Parse(typeof(SocialProvider), item)).ToList();
        }

        private void setCommands()
        {
            ProviderSelectedCommand = new Command(providerSelected);
            CancelCommand = new Command(cancel);
        }

        private async void cancel()
        {
            bool answer = await r_PageService.DisplayAlert("Note", "Are you sure?", "Yes", "No");
            if (answer)
            {
                r_PopupService.HidePopup(typeof(LinkProviderPage));
            }
        }

        private async void providerSelected(object i_Param)
        {
            try
            {
                Dictionary<string, object> details = await AuthHelper.LinkWithSocialProvider(CurrentProvider);
                tryGetPicture(details["photoUrl"] as string);
                await r_PageService.DisplayAlert("Note", $"Your account linked with: {CurrentProvider}", "Thanks");
            }
            catch(Exception e)
            {
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
            finally
            {
                r_PopupService.HidePopup(typeof(LinkProviderPage));
            }
        }

        private async void tryGetPicture(string i_Uri)
        {
            User current = AppManager.Instance.ConnectedUser;
            if (current.ProfilePictureUri == null && !string.IsNullOrWhiteSpace(i_Uri))
            {
                Uri uri = new Uri(i_Uri);
                current.ProfilePictureUri = uri;
                await RunTaskWhileLoading(FireStoreHelper.UpdateUser(current));
            }
        }

        #endregion
    }
}
