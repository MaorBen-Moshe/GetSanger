using GetSanger.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    [Preserve(AllMembers = true)]
    [DataContract]
    public abstract class BaseViewModel : PropertySetter
    {
        private bool m_IsLoading;
        private bool m_IsNotLoading;
        protected string m_DefaultBackUri = "..";
        public bool IsLoading
        {
            set
            {
                SetStructProperty(ref m_IsLoading, value);
                IsNotLoading = !value;
            }
            get => m_IsLoading;
        }
        public bool IsNotLoading
        {
            set => SetStructProperty(ref m_IsNotLoading, value);
            get => m_IsNotLoading;
        }

        protected virtual async Task GoBack()
        {
            await Shell.Current.GoToAsync(m_DefaultBackUri);
        }

        public async Task RunTaskWhileLoading(Task i_InnerTask)
        {
            try
            {
                DependencyService.Get<ILoadingService>().InitLoadingPage();
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
    }
}
