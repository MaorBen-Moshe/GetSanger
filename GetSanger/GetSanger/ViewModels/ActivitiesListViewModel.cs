using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
            if (activity.Status.Equals(ActivityStatus.Pending))
            {

            }
        }

        private void rejectActivity(object i_Param)
        {
            Activity activity = i_Param as Activity;
            if (activity.Status.Equals(ActivityStatus.Pending))
            {

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
