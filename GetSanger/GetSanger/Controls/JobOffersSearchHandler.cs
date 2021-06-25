using GetSanger.Constants;
using GetSanger.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using GetSanger.Services;

namespace GetSanger.Controls
{
    public class JobOffersSearchHandler : BaseSearchHandler<JobOffer>
    {
        #region Properties
        #endregion

        #region Methods

        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                ItemsSource = Source
                    .Where(job => job.Title.ToLower().Contains(newValue.ToLower()) || job.CategoryName.ToLower().Contains(newValue.ToLower()))
                    .ToList();
            }
        }

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);

            // Let the animation complete
            await Task.Delay(1000);

            ShellNavigationState state = (App.Current.MainPage as Shell).CurrentState;
            // The following route works because route names are unique in this application.
            string json = ObjectJsonSerializer.SerializeForPage((JobOffer)item);
            await Shell.Current.GoToAsync(ShellRoutes.ViewJobOffer + $"?jobOffer={json}");
        }

        #endregion
    }
}
