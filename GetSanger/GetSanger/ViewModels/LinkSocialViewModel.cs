using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class LinkSocialViewModel : BaseViewModel
    {
        #region Fields
        private List<SocialProviderCell> m_Socials;
        private SocialProviderCell m_SelectedItem;
        #endregion

        #region Properties
        public List<SocialProviderCell> Socials
        {
            get => m_Socials;
            set => SetClassProperty(ref m_Socials, value);
        }

        public SocialProviderCell SelectedItem
        {
            get => m_SelectedItem;
            set => SetClassProperty(ref m_SelectedItem, value);
        }

        #endregion

        #region Commands
        public ICommand LinkSocialCommand { get; set; }
        #endregion

        #region Constructor
        public LinkSocialViewModel()
        {
            setCommands();
            eSocialProvider[] providers = (eSocialProvider[])Enum.GetValues(typeof(eSocialProvider));
            Socials = providers.Select(provider => new SocialProviderCell
            {
                SocialProvider = provider
            }).ToList();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(LinkSocialViewModel));
        }

        private void setCommands()
        {
            LinkSocialCommand = new Command(linkSocial);
        }

        private void linkSocial(object i_Param)
        {
            if(i_Param is SocialProviderCell provider)
            {
                providerSelected(provider.SocialProvider);
            }
        }

        private async void providerSelected(eSocialProvider i_CurrentProvider)
        {
            try
            {
                if (i_CurrentProvider.Equals(eSocialProvider.Email))
                {
                    var page = new LinkEmailPage();
                    await PopupNavigation.Instance.PushAsync(page);
                }
                else
                {
                    r_LoadingService.ShowPopup();
                    Dictionary<string, object> details = await AuthHelper.LinkWithSocialProvider(i_CurrentProvider);
                    string photoUrl = details.ContainsKey("photoUrl") ? details["photoUrl"] as string : null;
                    await r_PhotoDisplay.TryGetPictureFromUri(photoUrl, AppManager.Instance.ConnectedUser);
                    await r_PageService.DisplayAlert("Note", $"Your account linked with: {i_CurrentProvider}", "Thanks");
                    r_LoadingService.HidePopup();
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            catch (Exception e)
            {
                r_LoadingService.HidePopup();
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
            finally
            {
                SelectedItem = null;
            }
        }

        #endregion
    }
}
