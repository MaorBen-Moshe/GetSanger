using GetSanger.Extensions;
using GetSanger.Interfaces;
using GetSanger.Services;
using GetSanger.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public abstract class BaseViewModel : PropertySetter
    {
        #region Fields
        private bool m_IsLoading;
        private bool m_IsNotLoading;
        private bool m_IsEnabledSendBtn;
        private string m_DefaultBackUri = "..";
        private readonly IRunTasks r_RunTasks;
        protected readonly IPageService r_PageService;
        protected readonly IDialService r_DialService;
        protected readonly IPhotoDisplay r_PhotoDisplay;
        protected readonly IUiPush r_PushService;
        protected readonly Interfaces.INavigation r_NavigationService;
        protected readonly IStorageHelper r_StorageHelper;
        protected readonly ILogin r_LoginServices;
        protected readonly ILocation r_LocationService;
        protected readonly ISocialAdapter r_SocialService;
        protected readonly ILoadingDisplay r_LoadingService;
        protected readonly ICrashlyticsDisplay r_CrashlyticsService;
        protected readonly ITrip r_TripHelper;
        #endregion

        #region Properties

        protected bool IsLoading
        {
            set
            {
                SetStructProperty(ref m_IsLoading, value);
                IsNotLoading = !value;
            }
            get => m_IsLoading;
        }

        protected bool IsNotLoading
        {
            set => SetStructProperty(ref m_IsNotLoading, value);
            get => m_IsNotLoading;
        }

        public bool IsEnabledsendBtn
        {
            get => m_IsEnabledSendBtn;
            set => SetStructProperty(ref m_IsEnabledSendBtn, value);

        }
        #endregion

        #region Constructor
        protected BaseViewModel()
        {
            r_PageService = AppManager.Instance.Services.GetService(typeof(PageServices)) as PageServices;
            r_DialService = AppManager.Instance.Services.GetService(typeof(DialServices)) as DialServices;
            r_LocationService = AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
            r_TripHelper = AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
            r_PushService = AppManager.Instance.Services.GetService(typeof(PushServices)) as PushServices;
            r_NavigationService = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            r_StorageHelper = AppManager.Instance.Services.GetService(typeof(StorageHelper)) as StorageHelper;
            r_LoginServices = AppManager.Instance.Services.GetService(typeof(LoginServices)) as LoginServices;
            r_PhotoDisplay = AppManager.Instance.Services.GetService(typeof(PhotoDisplayService)) as PhotoDisplayService;
            r_SocialService = AppManager.Instance.Services.GetService(typeof(SocialAdapterService)) as SocialAdapterService;
            r_RunTasks = AppManager.Instance.Services.GetService(typeof(RunTasksService)) as RunTasksService;
            r_LoadingService = AppManager.Instance.Services.GetService(typeof(LoadingService)) as LoadingService;
            r_CrashlyticsService = AppManager.Instance.Services.GetService(typeof(CrashlyticsService)) as CrashlyticsService;
            
            IsEnabledsendBtn = false;
        }
        #endregion

        #region Methods
        protected virtual async Task GoBack()
        {
            try
            {
                await Shell.Current.GoToAsync(m_DefaultBackUri);
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(BaseViewModel)}:GoBack", "Error", e.Message);
            }
        }

        public Task RunTaskWhileLoading(Task i_InnerTask, string i_OptionalLoadingText = "Loading...")
        {
            var loading = new LoadingPage(i_OptionalLoadingText);
            return r_RunTasks.RunTaskWhileLoading(i_InnerTask, loading);
        }

        public Task<T> RunTaskWhileLoading<T>(Task<T> i_InnerTask, string i_OptionalLoadingText = "Loading...")
        {
            var loading = new LoadingPage(i_OptionalLoadingText);
            return r_RunTasks.RunTaskWhileLoading<T>(i_InnerTask, loading);
        }

        public abstract void Appearing();
        public abstract void Disappearing();
        protected abstract void setCommands();
        #endregion
    }
}
