using GetSanger.Constants;
using GetSanger.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class RatingsSearchHandler : SearchHandler
    {
        #region Properties
        public IList<Rating> Ratings { get; set; }
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
                ItemsSource = Ratings
                    .Where(rating =>
                    rating.RatingWriterName.ToLower().Contains(newValue) ||
                    rating.Description.ToLower().Contains(newValue)
                    ).ToList();
            }
        }

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);

            // Let the animation complete
            await Task.Delay(1000);

            ShellNavigationState state = (App.Current.MainPage as Shell).CurrentState;
            // The following route works because route names are unique in this application.
            await Shell.Current.GoToAsync($"{ShellRoutes.Profile}?userid={(item as Rating).RatingWriterId}");
        }

        #endregion
    }
}
