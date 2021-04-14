﻿using GetSanger.Interfaces;
using GetSanger.Services;
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
        #region Fields
        private bool m_IsLoading;
        private bool m_IsNotLoading;
        protected string m_DefaultBackUri = "..";
        protected readonly IPageService r_PageService;
        protected readonly IDialService r_DialService;
        protected readonly PushServices r_PushService;
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
            r_PageService = new PageServices();
            r_DialService = new DialServices();
            LocationServices = new LocationService();
            r_PushService = new PushServices();
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
