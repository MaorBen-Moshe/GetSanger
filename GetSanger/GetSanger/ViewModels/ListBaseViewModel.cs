using GetSanger.Extensions;
using GetSanger.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using System;

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
        private List<string> m_TimeFilterList;
        private int m_SelectedTimeFilterIndex;
        private List<string> m_CategoriesFilterList;
        private int m_SelectedCategoryFilterIndex;

        protected const string k_Newest = "Newest";
        protected const string k_Oldest = "Oldest";
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

        public List<string> TimeFilterList
        {
            get => m_TimeFilterList;
            set => SetClassProperty(ref m_TimeFilterList, value);
        }

        public int SelectedTimeFilterIndex
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
        #endregion

        #region Commands
        public ICommand RefreshingCommand { get; set; }
        public ICommand FilterSelectedCommand { get; set; }
        #endregion

        #region Constructor
        public ListBaseViewModel()
        {
            SetCommands();
            SelectedItem = null;
            CategoriesFilterList = typeof(eCategory).GetListOfEnumNames().ToList();
            TimeFilterList = new List<string>
            {
                k_Newest,
                k_Oldest
            };

            setFilterIndices();
        }
        #endregion

        #region Methods
        protected abstract void refreshList();

        protected abstract void filterSelected(object i_Param);

        protected override void SetCommands()
        {
            RefreshingCommand = new Command(refreshList);
            FilterSelectedCommand = new Command(filterSelected);
        }

        protected virtual void setFilterIndices()
        {
            SelectedCategoryFilterIndex = 0;
            SelectedTimeFilterIndex = 0;
        }

        protected void filterByTIme(Func<T, DateTime> lambda)
        {
            if (TimeFilterList[SelectedTimeFilterIndex].Equals(k_Newest))
            {
                FilteredCollection = new ObservableCollection<T>(FilteredCollection.OrderByDescending(lambda));
            }
            else
            {
                FilteredCollection = new ObservableCollection<T>(FilteredCollection.OrderBy(lambda));
            }
        }

        protected void filterByCategory(Func<T, bool> predicate)
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
        #endregion
    }
}
