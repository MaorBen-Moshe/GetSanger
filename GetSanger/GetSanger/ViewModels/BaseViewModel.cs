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
        private bool m_IsEnabledSendBtn;
        private const string k_DefaultBackUri = "..";
        private static readonly IRunTasks sr_RunTasks;
        protected static readonly IPageService sr_PageService;
        protected static readonly IDialService sr_DialService;
        protected static readonly IPhotoDisplay sr_PhotoDisplay;
        protected static readonly IUiPush sr_PushService;
        protected static readonly Interfaces.INavigation sr_NavigationService;
        protected static readonly IStorageHelper sr_StorageHelper;
        protected static readonly ILogin sr_LoginServices;
        protected static readonly ILocation sr_LocationService;
        protected static readonly ISocialAdapter sr_SocialService;
        protected static readonly ILoadingDisplay sr_LoadingService;
        protected static readonly ICrashlyticsDisplay sr_CrashlyticsService;
        protected static readonly ITrip sr_TripHelper;
        #endregion

        #region Properties

        public bool IsEnabledsendBtn
        {
            get => m_IsEnabledSendBtn;
            set => SetStructProperty(ref m_IsEnabledSendBtn, value);

        }
        #endregion

        #region Constructor
        protected BaseViewModel()
        {
            SetCommands();
            IsEnabledsendBtn = false;
        }

        static BaseViewModel()
        {
            sr_PageService = AppManager.Instance.Services.GetService(typeof(PageServices)) as PageServices;
            sr_DialService = AppManager.Instance.Services.GetService(typeof(DialServices)) as DialServices;
            sr_LocationService = AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
            sr_TripHelper = AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
            sr_PushService = AppManager.Instance.Services.GetService(typeof(PushServices)) as PushServices;
            sr_NavigationService = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            sr_StorageHelper = AppManager.Instance.Services.GetService(typeof(StorageHelper)) as StorageHelper;
            sr_LoginServices = AppManager.Instance.Services.GetService(typeof(LoginServices)) as LoginServices;
            sr_PhotoDisplay = AppManager.Instance.Services.GetService(typeof(PhotoDisplayService)) as PhotoDisplayService;
            sr_SocialService = AppManager.Instance.Services.GetService(typeof(SocialAdapterService)) as SocialAdapterService;
            sr_RunTasks = AppManager.Instance.Services.GetService(typeof(RunTasksService)) as RunTasksService;
            sr_LoadingService = AppManager.Instance.Services.GetService(typeof(LoadingService)) as LoadingService;
            sr_CrashlyticsService = AppManager.Instance.Services.GetService(typeof(CrashlyticsService)) as CrashlyticsService;
        }

        #endregion

        #region Methods
        protected virtual async Task GoBack()
        {
            try
            {
                await Shell.Current.GoToAsync(k_DefaultBackUri);
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(BaseViewModel)}:GoBack", "Error", e.Message);
            }
        }

        public Task RunTaskWhileLoading(Task i_InnerTask, string i_OptionalLoadingText = "Loading...")
        {
            var loading = new LoadingPage(i_OptionalLoadingText);
            return sr_RunTasks.RunTaskWhileLoading(i_InnerTask, loading);
        }

        public Task<T> RunTaskWhileLoading<T>(Task<T> i_InnerTask, string i_OptionalLoadingText = "Loading...")
        {
            var loading = new LoadingPage(i_OptionalLoadingText);
            return sr_RunTasks.RunTaskWhileLoading<T>(i_InnerTask, loading);
        }

        public abstract void Appearing();
        public abstract void Disappearing();
        protected abstract void SetCommands();
        #endregion
    }
}