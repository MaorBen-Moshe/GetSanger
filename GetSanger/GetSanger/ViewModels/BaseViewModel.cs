using GetSanger.Interfaces;
using GetSanger.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    public abstract class BaseViewModel : PropertySetter
    {
        #region Fields
        private bool m_IsLoading;
        private bool m_IsNotLoading;
        private bool m_IsEnabledSendBtn;
        protected string m_DefaultBackUri = "..";
        protected readonly IPageService r_PageService;
        protected readonly IDialService r_DialService;
        protected readonly PushServices r_PushService;
        protected readonly NavigationService r_NavigationService;
        protected readonly StorageHelper r_StorageHelper;
        protected readonly LoginServices r_LoginServices;
        #endregion

        #region Properties
        protected LocationService LocationServices { get; private set; }

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
            LocationServices = AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
            r_PushService = AppManager.Instance.Services.GetService(typeof(PushServices)) as PushServices;
            r_NavigationService = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
            r_StorageHelper = AppManager.Instance.Services.GetService(typeof(StorageHelper)) as StorageHelper;
            r_LoginServices = AppManager.Instance.Services.GetService(typeof(LoginServices)) as LoginServices;

            IsEnabledsendBtn = false;
        }
        #endregion

        #region Methods
        protected virtual async Task GoBack()
        {
            await Shell.Current.GoToAsync(m_DefaultBackUri);
        }

        public async Task RunTaskWhileLoading(Task i_InnerTask, ContentPage i_OptionalLoading = null)
        {
            try
            {
                DependencyService.Get<ILoadingService>().InitLoadingPage(i_OptionalLoading);
                DependencyService.Get<ILoadingService>().ShowLoadingPage();
                await i_InnerTask;
                DependencyService.Get<ILoadingService>().HideLoadingPage();
            }
            catch (Exception ex)
            {
                DependencyService.Get<ILoadingService>().HideLoadingPage();
                throw ex;
            }
        }

        public async Task<T> RunTaskWhileLoading<T>(Task<T> i_InnerTask, ContentPage i_OptionalLoading = null)
        {
            try
            {
                DependencyService.Get<ILoadingService>().InitLoadingPage(i_OptionalLoading);
                DependencyService.Get<ILoadingService>().ShowLoadingPage();
                T result = await i_InnerTask;
                DependencyService.Get<ILoadingService>().HideLoadingPage();
                return result;
            }
            catch (Exception ex)
            {
                DependencyService.Get<ILoadingService>().HideLoadingPage();
                throw ex;
            }
        }

        public abstract void Appearing();
        #endregion
    }
}
