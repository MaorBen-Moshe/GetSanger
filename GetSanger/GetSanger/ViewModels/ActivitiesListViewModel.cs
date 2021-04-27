using GetSanger.Constants;
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

        public override void Appearing()
        {
            setActivities();
        }

        private void searchActivity(object i_Param)
        {
            string text = i_Param as string;
            if (!string.IsNullOrWhiteSpace(text) && text.Length >= 5)
            {
                ActivitiesSource = (ObservableCollection<Activity>)ActivitiesSource.Where(activity =>
                activity.JobDetails.Description.ToLower().Contains(text) ||
                activity.Title.ToLower().Contains(text) ||
                activity.Status.ToString().ToLower().Contains(text) ||
                activity.JobDetails.Category.ToString().ToLower().Contains(text)
                );
            }
        }

        private async void confirmActivity(object i_Param)
        {
            Activity activity = i_Param as Activity;
            if (AppManager.Instance.CurrentMode.Equals(AppMode.Client) && activity.Status.Equals(ActivityStatus.Pending))
            {
                bool answer = await r_PageService.DisplayAlert("Warning", "Are you sure?", "Yes", "No");
                if (answer)
                {
                    AppManager.Instance.ConnectedUser.Activities.Remove(activity);
                    activity.Status = ActivityStatus.Active;
                    await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(activity));
                    r_PushService.SendToDevice(activity.SangerID, activity, $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} accepted your job offer :)");
                    //  need to check that the list(ActivitiesSource) is updated
                    AppManager.Instance.ConnectedUser.Activities.Add(activity);
                    foreach(Activity current in AppManager.Instance.ConnectedUser.Activities)
                    {
                        if (current.JobDetails.JobId.Equals(activity.JobDetails.JobId))
                        {
                            current.Status = ActivityStatus.Rejected;
                            r_PushService.SendToDevice(current.SangerID, activity, $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} rejected your job offer :)");
                        }
                    }

                    await r_NavigationService.NavigateTo(ShellRoutes.Activity + $"?activity={activity}");
                }
            }
        }

        private void rejectActivity(object i_Param)
        {
            Activity activity = i_Param as Activity;
            if (activity.Status.Equals(ActivityStatus.Pending) || activity.Status.Equals(ActivityStatus.Active))
            {
                switch (AppManager.Instance.CurrentMode)
                {
                    case AppMode.Client: doReject(activity, activity.SangerID, $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} rejected your job offer :("); break;
                    case AppMode.Sanger: doReject(activity, activity.ClientID, $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} decided to cancel the job offer he already accepted. for more information please contact him :("); break;
                }
            }
        }

        private async void doReject(Activity i_Activity,string i_SendToUserId, string i_Message)
        {
            bool answer = await r_PageService.DisplayAlert("Warning", "Are you sure?", "Yes", "No");
            if (answer)
            {
                AppManager.Instance.ConnectedUser.Activities.Remove(i_Activity);
                i_Activity.Status = ActivityStatus.Rejected;
                await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(i_Activity));
                r_PushService.SendToDevice(i_SendToUserId, i_Activity, i_Message);
                //  need to check that the list(ActivitiesSource) is updated
                AppManager.Instance.ConnectedUser.Activities.Add(i_Activity);
            }
        }

        private void refreshList()
        {
            setActivities();
            IsListRefreshing = false;
        }

        private void selectedActivity(object i_Param)
        {
            Shell.Current.GoToAsync($"activitydetail?activity={i_Param as Activity}");
        }

        private async void setActivities()
        {
            List<Activity> activities = await FireStoreHelper.GetActivities(AuthHelper.GetLoggedInUserId());
            if (AppManager.Instance.CurrentMode.Equals(AppMode.Client))
            {
                // client should not see pending activities because it is like job offers
                activities = activities.Where(activity => activity.Status.Equals(ActivityStatus.Pending) == false).ToList();
            }

            ActivitiesSource = new ObservableCollection<Activity>(activities);
        }
        #endregion
    }
}
