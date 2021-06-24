using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class ActivitiesSearchHandler : SearchHandler
    {
        #region Properties
        public IList<Activity> Activities { get; set; }
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
                ItemsSource = Activities
                    .Where(activity =>
                           activity.JobDetails.Description.ToLower().Contains(newValue) ||
                           activity.Title.ToLower().Contains(newValue) ||
                           activity.Status.ToString().ToLower().Contains(newValue) ||
                           activity.JobDetails.Category.ToString().ToLower().Contains(newValue))
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
            string json = ObjectJsonSerializer.SerializeForPage((Activity)item);
            await Shell.Current.GoToAsync(ShellRoutes.Activity + $"?activity={json}");
        }

        #endregion
    }
}
