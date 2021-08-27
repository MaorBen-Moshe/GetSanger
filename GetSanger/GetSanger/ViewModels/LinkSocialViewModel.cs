using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class LinkSocialViewModel : PopupBaseViewModel
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
            set => SetClassProperty(ref m_SelectedItem, null);
        }

        #endregion

        #region Commands

        public ICommand LinkSocialCommand { get; set; }

        #endregion

        #region Constructor
        public LinkSocialViewModel()
        {
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
            sr_CrashlyticsService.LogPageEntrance(nameof(LinkSocialViewModel));
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
            LinkSocialCommand = new Command(linkSocial);
        }

        private async void linkSocial(object i_Param)
        {
            if(i_Param is SocialProviderCell provider)
            {
                try
                {
                    eSocialProvider current = provider.SocialProvider;
                    if (current.Equals(eSocialProvider.Email))
                    {
                        var page = new LinkEmailPage();
                        await PopupNavigation.Instance.PushAsync(page);
                    }
                    else
                    {
                        await RunTaskWhileLoading(sr_SocialService.SocialLink(current));
                        await sr_PageService.DisplayAlert("Note", $"Your account linked with: {current}", "Thanks");
                        await PopupNavigation.Instance.PopAsync();
                    }
                }
                catch (Exception e)
                {
                    sr_LoadingService.HideLoadingPage();
                    await e.LogAndDisplayError($"{nameof(LinkSocialViewModel)}:providerSelected", "Error", e.Message);
                }
            }
        }

        #endregion
    }
}