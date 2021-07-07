using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public abstract class ListBaseViewModel<T> : BaseViewModel where T : class
    {
        #region Fields
        private bool m_IsListRefreshing;
        private bool m_IsVisibleEmptyListLabel;
        private ObservableCollection<T> m_Collection;
        private ObservableCollection<T> m_SearchCollection;
        private T m_SelectedItem;
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

        public ObservableCollection<T> SearchCollection
        {
            get => m_SearchCollection;
            set => SetClassProperty(ref m_SearchCollection, value);
        }

        public T SelectedItem
        {
            get => m_SelectedItem;
            set => SetClassProperty(ref m_SelectedItem, value);
        }
        #endregion

        #region Commands
        public ICommand RefreshingCommand { get; set; }
        #endregion

        #region Constructor
        public ListBaseViewModel()
        {
            RefreshingCommand = new Command(refreshList);
            SelectedItem = null;
        }
        #endregion

        #region Methods
        protected abstract void refreshList();
        #endregion
    }
}
