using GetSanger.Interfaces;
using GetSanger.Services;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    [Preserve(AllMembers = true)]
    public abstract class BaseViewModel : PropertySetter
    {
        #region Fields
        private bool m_IsLoading;
        private bool m_IsNotLoading;
        protected string m_DefaultBackUri = "..";
        protected readonly IPageService r_PageService;
        protected readonly IDialService r_DialService;
        protected readonly PushServices r_PushService;
        protected readonly NavigationService r_NavigationService;
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
        #endregion

        #region Constructor
        protected BaseViewModel()
        {
            r_PageService = AppManager.Instance.Services.GetService(typeof(PageServices)) as PageServices;
            r_DialService = AppManager.Instance.Services.GetService(typeof(DialServices)) as DialServices;
            LocationServices = AppManager.Instance.Services.GetService(typeof(LocationService)) as LocationService;
            r_PushService = AppManager.Instance.Services.GetService(typeof(PushServices)) as PushServices;
            r_NavigationService = AppManager.Instance.Services.GetService(typeof(NavigationService)) as NavigationService;
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
        #endregion
    }
}
