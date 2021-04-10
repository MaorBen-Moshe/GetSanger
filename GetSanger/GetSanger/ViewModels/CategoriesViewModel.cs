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
            CategoriesItems = AppManager.Instance.GetListOfEnum(typeof(Category)).Select(name => new CategoryCell { Category = (Category)Enum.Parse(typeof(Category), name) }).ToList();
        }
        #endregion

        #region Methods
        private void categorySelected(object i_Param)
        {
           
        }
        #endregion
    }
}
