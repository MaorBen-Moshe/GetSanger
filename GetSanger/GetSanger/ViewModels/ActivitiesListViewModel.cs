using GetSanger.Constants;
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
        private List<string> m_StatusFilterList;
        private int m_SelectedStatusFilterIndex;
        private const string k_All = "All";
        #endregion

        #region Properties
        public List<string> StatusFilterList
        {
            get => m_StatusFilterList;
            set => SetClassProperty(ref m_StatusFilterList, value);
        }

        public int SelectedStatusFilterIndex
        {
            get => m_SelectedStatusFilterIndex;
            set => SetStructProperty(ref m_SelectedStatusFilterIndex, value);
        }
        #endregion

        #region Commands
        public ICommand ConfirmActivityCommand { get; set; }
        public ICommand RejectActivityCommand { get; set; }
        public ICommand SelectedActivityCommand { get; set; }
        #endregion

        #region Constructor
        public ActivitiesListViewModel()
        {
            SetCommands();
            StatusFilterList = typeof(eActivityStatus).GetListOfEnumNames().ToList();
            StatusFilterList.Insert(0, k_All);
        }
        #endregion

        #region Methods

        public async override void Appearing()
        {
            try
            {
                sr_CrashlyticsService.LogPageEntrance(nameof(ActivitiesListViewModel));
                setActivities();
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:Appearing", "Error", e.Message);
            }
        }

        public override void Disappearing()
        {
            setFilterIndices();
        }

        protected override void SetCommands()
        {
            base.SetCommands();
            ConfirmActivityCommand = new Command(confirmActivity);
            RejectActivityCommand = new Command(rejectActivity);
            SelectedActivityCommand = new Command(selectedActivity);
        }

        protected override void setFilterIndices()
        {
            base.setFilterIndices();
            SelectedStatusFilterIndex = 0;
        }

        protected async override void filterSelected(object i_Param)
        {
            try
            {
                eCategory category = (eCategory)Enum.Parse(typeof(eCategory), CategoriesFilterList[SelectedCategoryFilterIndex]);
                if(SelectedStatusFilterIndex == 0) // all status
                {
                    filterByCategory(activity => activity.JobDetails.Category.Equals(category));
                }
                else
                {
                    eActivityStatus status = (eActivityStatus)Enum.Parse(typeof(eActivityStatus), StatusFilterList[SelectedStatusFilterIndex]);
                    Func<Activity, bool> predicate;
                    if (category.Equals(eCategory.All))
                    {
                        predicate = activity => activity.Status.Equals(status);
                    }
                    else
                    {
                        predicate = activity => activity.JobDetails.Category.Equals(category) && activity.Status.Equals(status);
                    }

                    FilteredCollection = new ObservableCollection<Activity>(
                            from activity in AllCollection
                            where predicate.Invoke(activity)
                            select activity
                            );
                }

                filterByTIme(activity => activity.JobDetails.Date);
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:filterSelected", "Error", e.Message);
            }
        }

        private async void confirmActivity(object i_Param)
        {
            try
            {
                Activity activity = i_Param as Activity;
                if (AppManager.Instance.CurrentMode.Equals(eAppMode.Client) && activity.Status.Equals(eActivityStatus.Pending))
                {
                    await sr_PageService.DisplayAlert("Warning", "Are you sure?", "Yes", "No",
                        async (answer) =>
                        {
                            if (answer)
                            {
                                activity.Status = eActivityStatus.Active;
                                await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(activity));
                                await RunTaskWhileLoading(sr_PushService.SendToDevice(activity.SangerID, activity, typeof(Activity).Name, "Activity Confirmed", $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} accepted your job offer :)"));
                                //  need to check that the list(ActivitiesSource) is updated
                                foreach (Activity current in AppManager.Instance.ConnectedUser.Activities)
                                {
                                    if (current.JobDetails.JobId.Equals(activity.JobDetails.JobId))
                                    {
                                        current.Status = eActivityStatus.Rejected;
                                        await RunTaskWhileLoading(sr_PushService.SendToDevice(current.SangerID, activity, typeof(Activity).Name, "Activity Rejected", $"{AppManager.Instance.ConnectedUser.PersonalDetails.NickName} rejected your job offer :)"));
                                    }
                                }

                                string json = ObjectJsonSerializer.SerializeForPage(activity);
                                await sr_NavigationService.NavigateTo($"{ShellRoutes.Activity}?activity={json}");
                            }
                        });
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:confirmActivity", "Error", e.Message);
            }
            finally
            {
                SearchCollection = new ObservableCollection<Activity>(FilteredCollection);
            }
        }

        private async void rejectActivity(object i_Param)
        {
            try
            {
                Activity activity = i_Param as Activity;
                if (activity.Status.Equals(eActivityStatus.Pending) || activity.Status.Equals(eActivityStatus.Active))
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

                    IsVisibleViewList = AllCollection.Count > 0;
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:rejectActivity", "Error", e.Message);
            }
        }

        private async void doReject(Activity i_Activity,string i_SendToUserId, string i_Message)
        {
            await sr_PageService.DisplayAlert("Warning", "Are you sure?", "Yes", "No",
                async (answer) =>
                {
                    if (answer)
                    {
                        i_Activity.Status = eActivityStatus.Rejected;
                        await RunTaskWhileLoading(FireStoreHelper.UpdateActivity(i_Activity));
                        await RunTaskWhileLoading(sr_PushService.SendToDevice(i_SendToUserId, i_Activity, typeof(Activity).Name, "Activity Rejected", i_Message));
                        //  need to check that the list(ActivitiesSource) is updated
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
                    await sr_NavigationService.NavigateTo($"{ShellRoutes.Activity}?activity={json}");
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
                activities = activities.Where(activity => activity.ClientID.Equals(AuthHelper.GetLoggedInUserId())).ToList();
            }
            else // sanger mode
            {
                activities = activities.Where(activity => activity.SangerID.Equals(AuthHelper.GetLoggedInUserId())).ToList();
            }

            AllCollection = new ObservableCollection<Activity>(activities.OrderByDescending(activity => activity.JobDetails.Date));
            FilteredCollection = new ObservableCollection<Activity>(AllCollection);
            SearchCollection = new ObservableCollection<Activity>(AllCollection);
            IsVisibleViewList = AllCollection.Count > 0;
            setFilterIndices();
        }
        #endregion
    }
}