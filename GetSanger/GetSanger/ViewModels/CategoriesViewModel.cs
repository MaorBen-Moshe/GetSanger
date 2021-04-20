using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class CategoriesViewModel : BaseViewModel
    {
        #region Fields
        private IList<CategoryCell> m_CategoriesItems;
        #endregion

        #region Properties

        public IList<CategoryCell> CategoriesItems
        {
            get => m_CategoriesItems;
            set => SetClassProperty(ref m_CategoriesItems, value);
        }
        #endregion

        #region Commands
        public ICommand CategorySelectedCommand { get; set; }

        #endregion

        #region Constructor
        public CategoriesViewModel()
        {
            CategorySelectedCommand = new Command(categorySelected);
        }
        #endregion

        #region Methods
        private void categorySelected(object i_Param)
        {
            CategoryCell current = i_Param as CategoryCell;

            Shell.Current.GoToAsync($"jobOffer?category={current.Category}");
        }

        protected override void appearing(object i_Param)
        {
            CategoriesItems = AppManager.Instance.GetListOfEnumNames(typeof(Category)).Select(name => new CategoryCell { Category = (Category)Enum.Parse(typeof(Category), name) }).ToList();
        }

        protected override void disappearing(object i_Param)
        {
        }
        #endregion
    }
}
