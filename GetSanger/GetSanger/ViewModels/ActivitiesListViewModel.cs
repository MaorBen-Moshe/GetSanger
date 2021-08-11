using GetSanger.Constants;
using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using GetSanger.Utils;
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
        private bool m_IsClientMode;
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

        public bool IsClientMode
        {
            get => m_IsClientMode;
            set => SetStructProperty(ref m_IsClientMode, value);
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
                    Predicate<Activity> predicate;
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

                sortByTime(activity => activity.JobDetails.Date);
                IsVisibleViewList = FilteredCollection.Count > 0;
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:filterSelected", "Error", e.Message);
            }
        }

        protected override void sort(object i_Param)
        {
            TimeSortFlag = !TimeSortFlag;
            sortByTime(activity => activity.JobDetails.Date);
        }

        private async void confirmActivity(object i_Param)
        {
            try
            {
                if(i_Param is Activity activity)
                {
                    sr_LoadingService.ShowLoadingPage();
                    ActivitiesConfirmationHelper.ConfirmActivity(activity, async () =>
                    {
                        string json = ObjectJsonSerializer.SerializeForPage(activity);
                        await sr_NavigationService.NavigateTo($"{ShellRoutes.Activity}?activity={json}");
                    });
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:confirmActivity", "Error", e.Message);
            }
            finally
            {
                sr_LoadingService.HideLoadingPage();
                SearchCollection = new ObservableCollection<Activity>(FilteredCollection);
            }
        }

        private async void rejectActivity(object i_Param)
        {
            try
            {
                if(i_Param is Activity activity)
                {
                    sr_LoadingService.ShowLoadingPage();
                    ActivitiesConfirmationHelper.RejectActivity(activity, () =>
                    {
                        IsVisibleViewList = AllCollection.Count > 0;
                    });
                }
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(ActivitiesListViewModel)}:rejectActivity", "Error", e.Message);
            }
            finally
            {
                sr_LoadingService.HideLoadingPage();
            }
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

        private void setActivities()
        {
            setItems(async () =>
            {
                List<Activity> activities = await FireStoreHelper.GetActivities(AuthHelper.GetLoggedInUserId());
                AppManager.Instance.ConnectedUser.Activities = new ObservableCollection<Activity>(activities);
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
                NoItemsText = "No activities available";
                setFilterIndices();
                IsClientMode = AppManager.Instance.CurrentMode.Equals(eAppMode.Client);
            });
        }
        #endregion
    }
}