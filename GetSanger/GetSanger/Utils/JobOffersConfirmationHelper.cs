using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Views.popups;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;

namespace GetSanger.Utils
{
    public static class JobOffersConfirmationHelper
    {
        public static async Task ConfirmJobOffer(JobOffer i_Job)
        {
            if (AppManager.Instance.CurrentMode.Equals(eAppMode.Sanger))
            {
                await PopupNavigation.Instance.PushAsync(new SangerNotesView(i_Job));
            }
        }

        public static void DeleteMyJobOfferCommand(Action action)
        {
            if (AppManager.Instance.CurrentMode.Equals(eAppMode.Client))
            {
                action?.Invoke();
            }
        }
    }
}