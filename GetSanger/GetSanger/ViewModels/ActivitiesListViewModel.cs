using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GetSanger.ViewModels
{
    public class ActivitiesListViewModel : BaseViewModel
    {
        #region Fields
        private ObservableCollection<Activity> m_ActivitiesSource;
        #endregion
        #region Properties
        public ObservableCollection<Activity> ActivitiesSource
        {
            get => m_ActivitiesSource;
            set => SetClassProperty(ref m_ActivitiesSource, value);
        }
        #endregion

        #region Constructor
        public ActivitiesListViewModel(IEnumerable<Activity> i_Source)
        {

        }
        #endregion
    }
}
