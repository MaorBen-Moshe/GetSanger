using GetSanger.Extensions;
using GetSanger.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using System;
using System.Threading.Tasks;

namespace GetSanger.ViewModels
{
    public abstract class ListBaseViewModel<T> : BaseViewModel where T : class
    {
        #region Fields
        private bool m_IsListRefreshing;
        private bool m_IsVisibleEmptyListLabel;
        private ObservableCollection<T> m_AllCollection;
        private ObservableCollection<T> m_FilteredCollection;
        private ObservableCollection<T> m_SearchCollection;
        private T m_SelectedItem;
        private bool m_SelectedTimeFilterIndex;
        private List<string> m_CategoriesFilterList;
        private int m_SelectedCategoryFilterIndex;
        private string m_NoItemsText;
        private Color m_NoItemsTextColor;
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

        public ObservableCollection<T> AllCollection
        {
            get => m_AllCollection;
            set => SetClassProperty(ref m_AllCollection, value);
        }

        public ObservableCollection<T> FilteredCollection
        {
            get => m_FilteredCollection;
            set => SetClassProperty(ref m_FilteredCollection, value);
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

        public bool TimeSortFlag
        {
            get => m_SelectedTimeFilterIndex;
            set => SetStructProperty(ref m_SelectedTimeFilterIndex, value);
        }


        public List<string> CategoriesFilterList
        {
            get => m_CategoriesFilterList;
            set => SetClassProperty(ref m_CategoriesFilterList, value);
        }

        public int SelectedCategoryFilterIndex
        {
            get => m_SelectedCategoryFilterIndex;
            set => SetStructProperty(ref m_SelectedCategoryFilterIndex, value);
        }

        public string NoItemsText
        {
            get => m_NoItemsText;
            set => SetClassProperty(ref m_NoItemsText, value);
        }

        public Color NoItemsTextColor
        {
            get => m_NoItemsTextColor;
            set => SetStructProperty(ref m_NoItemsTextColor, value);
        }
        #endregion

        #region Commands
        public ICommand RefreshingCommand { get; set; }
        public ICommand FilterSelectedCommand { get; set; }
        public ICommand SortCommand { get; set; }
        #endregion

        #region Constructor
        public ListBaseViewModel()
        {
            SelectedItem = null;
            CategoriesFilterList = typeof(eCategory).GetListOfEnumNames().ToList();
            setFilterIndices();
        }
        #endregion

        #region Methods
        protected abstract void refreshList();

        protected abstract void filterSelected(object i_Param);

        protected abstract void sort(object i_Param);

        protected override void SetCommands()
        {
            RefreshingCommand = new Command(refreshList);
            FilterSelectedCommand = new Command(filterSelected);
            SortCommand = new Command(sort);
        }

        protected virtual void setFilterIndices()
        {
            SelectedCategoryFilterIndex = 0;
            TimeSortFlag = false;
        }

        protected void sortByTime(Func<T, DateTime> lambda)
        {
            if (TimeSortFlag == false)
            {
                FilteredCollection = new ObservableCollection<T>(FilteredCollection.OrderByDescending(lambda));
            }
            else 
            {
                FilteredCollection = new ObservableCollection<T>(FilteredCollection.OrderBy(lambda));
            }
        }

        protected void filterByCategory(Predicate<T> predicate)
        {
            eCategory category = (eCategory)Enum.Parse(typeof(eCategory), CategoriesFilterList[SelectedCategoryFilterIndex]);
            if (category.Equals(eCategory.All))
            {
                FilteredCollection = new ObservableCollection<T>(AllCollection);
            }
            else
            {
                FilteredCollection = new ObservableCollection<T>(
                    from current in AllCollection
                    where predicate.Invoke(current)
                    select current
                    );
            }
        }

        protected async void setItems(Func<Task> func)
        {
            NoItemsTextColor = Color.Red;
            IsVisibleViewList = false;
            NoItemsText = "Fetching items...";
            await func?.Invoke();
            NoItemsTextColor = Color.DarkGray;
        }
        #endregion
    }
}