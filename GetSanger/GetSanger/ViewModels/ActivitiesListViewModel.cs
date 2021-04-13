using GetSanger.Models;
using GetSanger.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(ActivitiesSource), "activities")]
    public class ActivitiesListViewModel : BaseViewModel
    {
        #region Fields
        private ObservableCollection<Activity> m_ActivitiesSource;
        private bool m_IsListRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<Activity> ActivitiesSource
        {
            get => m_ActivitiesSource;
            set => SetClassProperty(ref m_ActivitiesSource, value);
        }

        public bool IsListRefreshing
        {
            get => m_IsListRefreshing;
            set => SetStructProperty(ref m_IsListRefreshing, value);
        }
        #endregion

        #region Commands
        public ICommand SearchActivity { get; set; }
        public ICommand ConfirmActivityCommand { get; set; }
        public ICommand RejectActivityCommand { get; set; }
        public ICommand RefreshingCommand { get; set; }
        public ICommand SelectedActivityCommand { get; set; }
        #endregion

        #region Constructor
        public ActivitiesListViewModel()
        {
            SearchActivity = new Command(searchActivity);
            ConfirmActivityCommand = new Command(confirmActivity);
            RejectActivityCommand = new Command(rejectActivity);
            RefreshingCommand = new Command(refreshList);
            SelectedActivityCommand = new Command(selectedActivity);
        }
        #endregion

        #region Methods
        private void searchActivity(object i_Param)
        {
            string text = i_Param as string;
            if (!string.IsNullOrWhiteSpace(text) && text.Length >= 5)
            {
                ActivitiesSource = (ObservableCollection<Activity>)ActivitiesSource.Where(activity =>
                activity.JobOffer.Description.Contains(text) ||
                activity.Title.Contains(text) ||
                activity.Status.ToString().Contains(text) ||
                activity.JobOffer.Category.ToString().Contains(text)
                );
            }
        }

        private void confirmActivity(object i_Param)
        {
            Activity activity = i_Param as Activity;
            if (activity.Status.Equals(ActivityStatus.Pending)) //snager mode
            {
                activity.Status = ActivityStatus.ConfirmedBySanger;
                FireStoreHelper.DeleteActivity(activity);
                r_PushService.SendToDevice(activity.ClientID, activity, $"{AppManager.Instance.ConnectedUser.PersonalDetails.Nickname} confirmed your job.");
            }
            else if (activity.Status.Equals(ActivityStatus.ConfirmedBySanger)) // user mode
            {
                activity.Status = ActivityStatus.Active;
                AppManager.Instance.ConnectedUser.ActivatedMap.Add(activity.ActivityId, false);
                FireStoreHelper.UpdateUser(AppManager.Instance.ConnectedUser);
                FireStoreHelper.UpdateActivity(activity);
                r_PushService.SendToDevice(activity.SangerID, activity, $"{AppManager.Instance.ConnectedUser.PersonalDetails.Nickname} confirmed your job.\n You can see it now on your list.");
                IList<Activity> rejected = (from Rejectactivity in AppManager.Instance.ConnectedUser.Activities
                                            where Rejectactivity.JobOffer.JobId.Equals(activity.JobOffer.JobId) && Rejectactivity.ActivityId != activity.ActivityId
                                            select Rejectactivity).ToList();
                foreach(Activity reject in rejected)
                {
                    reject.Status = ActivityStatus.Rejected;
                    r_PushService.SendToDevice(reject.SangerID, reject, $"{AppManager.Instance.ConnectedUser.PersonalDetails.Nickname} rejected your job offer.");
                    FireStoreHelper.DeleteActivity(reject);
                }
            }
        }

        private void rejectActivity(object i_Param)
        {
            Activity activity = i_Param as Activity;
            if (activity.Status.Equals(ActivityStatus.Pending)) // sanger mode
            {
                FireStoreHelper.DeleteActivity(activity);
            }
            else if (activity.Status.Equals(ActivityStatus.ConfirmedBySanger)) // user mode
            {
                FireStoreHelper.DeleteActivity(activity);
                activity.Status = ActivityStatus.Rejected;
                r_PushService.SendToDevice(activity.SangerID, activity, $"{AppManager.Instance.ConnectedUser.PersonalDetails.Nickname} rejected your job offer.");
            }
        }

        private async void refreshList()
        {
            ActivitiesSource = new ObservableCollection<Activity>(await FireStoreHelper.GetActivities(AppManager.Instance.ConnectedUser.UserID));

            IsListRefreshing = false;
        }

        private void selectedActivity(object i_Param)
        {
            Shell.Current.GoToAsync($"activitydetail?activity={i_Param as Activity}");
        }
        #endregion
    }
}
