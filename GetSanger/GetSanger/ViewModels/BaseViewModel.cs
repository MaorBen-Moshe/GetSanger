using GetSanger.Interfaces;
using GetSanger.Services;
using GetSanger.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public abstract class BaseViewModel : PropertySetter
    {
        #region Fields
        private bool m_IsLoading;
        private bool m_IsNotLoading;
        private bool m_IsEnabledSendBtn;
        private bool m_IsVisibleEmptyListLabel;
        private string m_DefaultBackUri = "..";
        private readonly RunTasksService r_RunTasks;
        protected readonly IPageService r_PageService;
        protected readonly IDialService r_DialService;
        protected readonly IPhotoDisplay r_PhotoDisplay;
        protected readonly PushServices r_PushService;
        protected readonly NavigationService r_NavigationService;
        protected readonly StorageHelper r_StorageHelper;
        protected readonly LoginServices r_LoginServices;
        protected readonly LocationService r_LocationServices;
        protected readonly SocialAdapterService r_SocialService;
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

        public bool IsVisibleViewList
        {
            get => m_IsVisibleEmptyListLabel;
            set => SetStructProperty(ref m_IsVisibleEmptyListLabel, value);
        }
        #endregion

        #region Constructor
        protected BaseViewModel()
        {
            r_PageService = AppManager.Instance.Services.GetService(typeof(PageServices)) as PageServices;
            r_DialService = AppManager.Instance.Services.GetService(typeof(DialServices)) as DialServices;
            r_LocationServices = AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
            r_PushService = AppManager.Instance.Services.GetService(typeof(PushServices)) as PushServices;
            r_NavigationService = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            r_StorageHelper = AppManager.Instance.Services.GetService(typeof(StorageHelper)) as StorageHelper;
            r_LoginServices = AppManager.Instance.Services.GetService(typeof(LoginServices)) as LoginServices;
            r_PhotoDisplay = AppManager.Instance.Services.GetService(typeof(PhotoDisplayService)) as PhotoDisplayService;
            r_SocialService = AppManager.Instance.Services.GetService(typeof(SocialAdapterService)) as SocialAdapterService;
            r_RunTasks = AppManager.Instance.Services.GetService(typeof(RunTasksService)) as RunTasksService;

            IsEnabledsendBtn = false;
        }
        #endregion

        #region Methods
        protected virtual async Task GoBack()
        {
            await Shell.Current.GoToAsync(m_DefaultBackUri);
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
        #endregion
    }
}
