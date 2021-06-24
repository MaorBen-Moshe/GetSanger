using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public abstract class ListBaseViewModel<T> : BaseViewModel
    {
        #region Fields
        private bool m_IsListRefreshing;
        private bool m_IsVisibleEmptyListLabel;
        private ObservableCollection<T> m_Collection;
        #endregion

        #region Properties
        public bool IsListRefreshing
        {
            get => m_IsListRefreshing;
            set => SetStructProperty(ref m_IsListRefreshing, value);
        }

        public bool IsVisibleViewList
        {
            get => m_IsVisibleEmptyListLabel;
            set => SetStructProperty(ref m_IsVisibleEmptyListLabel, value);
        }

        public ObservableCollection<T> Collection
        {
            get => m_Collection;
            set => SetClassProperty(ref m_Collection, value);
        }
        #endregion

        #region Commands
        public ICommand RefreshingCommand { get; set; }
        #endregion

        #region Constructor
        public ListBaseViewModel()
        {
            RefreshingCommand = new Command(refreshList);
        }
        #endregion

        #region Methods
        protected abstract void refreshList();
        #endregion
    }
}
