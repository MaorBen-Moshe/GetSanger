﻿using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class ActivitiesListViewModel : ListBaseViewModel<Activity>
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Commands
        public ICommand ConfirmActivityCommand { get; set; }
        public ICommand RejectActivityCommand { get; set; }
        public ICommand SelectedActivityCommand { get; set; }
        #endregion

        #region Constructor
        public ActivitiesListViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods

        public async override void Appearing()
        {
            try
            {
                r_CrashlyticsService.LogPageEntrance(nameof(ActivitiesListViewModel));
                setActivities();
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:Appearing", "Error", e.Message);
            }
        }

        private void setCommands()
        {
            ConfirmActivityCommand = new Command(confirmActivity);
            RejectActivityCommand = new Command(rejectActivity);
            SelectedActivityCommand = new Command(selectedActivity);
        }

        private async void confirmActivity(object i_Param)
        {
            try
            {
                Activity activity = i_Param as Activity;
                if (AppManager.Instance.CurrentMode.Equals(eAppMode.Client) && activity.Status.Equals(ActivityStatus.Pending))
                {
                    await r_PageService.DisplayAlert("Warning", "Are you sure?", "Yes", "No",
                        async (answer) =>
                        {
                            if (answer)
                            {
                                AppManager.Instance.ConnectedUser.Activities.Remove(activity);
                                activity.Status = ActivityStatus.Active;
                                await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(activity));
                                await RunTaskWhileLoading(r_PushService.SendToDevice(activity.SangerID, activity, activity.GetType(), "Activity Confirmed", $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} accepted your job offer :)"));
                                //  need to check that the list(ActivitiesSource) is updated
                                AppManager.Instance.ConnectedUser.Activities.Add(activity);
                                foreach (Activity current in AppManager.Instance.ConnectedUser.Activities)
                                {
                                    if (current.JobDetails.JobId.Equals(activity.JobDetails.JobId))
                                    {
                                        current.Status = ActivityStatus.Rejected;
                                        await RunTaskWhileLoading(r_PushService.SendToDevice(current.SangerID, activity, activity.GetType(), "Activity Rejected", $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} rejected your job offer :)"));
                                    }
                                }

                                await r_NavigationService.NavigateTo(ShellRoutes.Activity + $"?activity={activity}");
                            }
                        });
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:confirmActivity", "Error", e.Message);
            }
        }

        private async void rejectActivity(object i_Param)
        {
            try
            {
                Activity activity = i_Param as Activity;
                if (activity.Status.Equals(ActivityStatus.Pending) || activity.Status.Equals(ActivityStatus.Active))
                {
                    switch (AppManager.Instance.CurrentMode)
                    {
                        case eAppMode.Client:
                            doReject(activity, activity.SangerID, $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} rejected your job offer :(");
                            break;
                        case eAppMode.Sanger:
                            doReject(activity, activity.ClientID, $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} decided to cancel the job offer he already accepted. for more information please contact him :(");
                            break;
                    }

                    IsVisibleViewList = Collection.Count > 0;
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:rejectActivity", "Error", e.Message);
            }
        }

        private async void doReject(Activity i_Activity,string i_SendToUserId, string i_Message)
        {
            await r_PageService.DisplayAlert("Warning", "Are you sure?", "Yes", "No",
                async (answer) =>
                {
                    if (answer)
                    {
                        AppManager.Instance.ConnectedUser.Activities.Remove(i_Activity);
                        i_Activity.Status = ActivityStatus.Rejected;
                        await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(i_Activity));
                        await RunTaskWhileLoading(r_PushService.SendToDevice(i_SendToUserId, i_Activity, i_Activity.GetType(), "Activity Rejected", i_Message));
                        //  need to check that the list(ActivitiesSource) is updated
                        AppManager.Instance.ConnectedUser.Activities.Add(i_Activity);
                    }
                });
        }

        protected async override void refreshList()
        {
            try
            {
                setActivities();
                IsListRefreshing = false;
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:refreshList", "Error", e.Message);
            }
        }

        private async void selectedActivity(object i_Param)
        {
            try
            {
                if(i_Param is Activity activity)
                {
                    string json = ObjectJsonSerializer.SerializeForPage(activity);
                    await r_NavigationService.NavigateTo($"{ShellRoutes.Activity}?activity={json}");
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:selectedActivity", "Error", e.Message);
            }
            finally
            {
                SelectedItem = null;
            }
        }

        private async void setActivities()
        {
            List<Activity> activities = await RunTaskWhileLoading(FireStoreHelper.GetActivities(AuthHelper.GetLoggedInUserId()));
            if (AppManager.Instance.CurrentMode.Equals(eAppMode.Client))
            {
                // client should not see pending activities because it is like job offers
                activities = activities.Where(activity => activity.Status.Equals(ActivityStatus.Pending) == false).ToList();
            }

            Collection = new ObservableCollection<Activity>(activities);
            SearchCollection = new ObservableCollection<Activity>(Collection);
            IsVisibleViewList = Collection.Count > 0;
        }
        #endregion
    }
}
