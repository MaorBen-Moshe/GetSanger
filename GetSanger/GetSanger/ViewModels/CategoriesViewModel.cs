using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using GetSanger.Extensions;

namespace GetSanger.ViewModels
{
    public class CategoriesViewModel : BaseViewModel
    {
        #region Fields
        private IList<CategoryCell> m_CategoriesItems;
        private CategoryCell m_SelectedItem;
        #endregion

        #region Properties

        public IList<CategoryCell> CategoriesItems
        {
            get => m_CategoriesItems;
            set => SetClassProperty(ref m_CategoriesItems, value);
        }

        public CategoryCell SelectedItem
        {
            get => m_SelectedItem;
            set { SetClassProperty(ref m_SelectedItem, value); categorySelected(); }
        }

        #endregion

        #region Commands
        public ICommand CategorySelectedCommand { get; set; }

        #endregion

        #region Constructor
        public CategoriesViewModel()
        {
            SetCommands();
            CategoriesItems = typeof(eCategory).GetListOfEnumNames().Select(name => new CategoryCell { Category = (eCategory)Enum.Parse(typeof(eCategory), name) }).ToList();
            CategoriesItems = CategoriesItems.Where(categoryCell => categoryCell.Category.Equals(eCategory.All) == false).ToList();
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            r_CrashlyticsService.LogPageEntrance(nameof(CategoriesViewModel));
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
            CategorySelectedCommand = new Command(categorySelected);
        }

        private async void categorySelected()
        {
            try
            {
                if (SelectedItem != null)
                {
                    var category = SelectedItem.Category;
                    await r_NavigationService.NavigateTo($"{ShellRoutes.EditJobOffer }?category={category}&isCreate={true}");
                    SelectedItem = null;
                }
            }
            catch(Exception e)
            {
                await e.LogAndDisplayError($"{nameof(CategoriesViewModel)}:categorySelected", "Error", e.Message);
            }
        }
        #endregion
    }
}