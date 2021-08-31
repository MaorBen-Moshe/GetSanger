using GetSanger.Constants;
using GetSanger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using GetSanger.Extensions;
using Xamarin.Forms;

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
            set { SetClassProperty(ref m_SelectedItem, null); categorySelected(value); }
        }

        #endregion

        #region Commands
        #endregion

        #region Constructor
        public CategoriesViewModel()
        {
            CategoriesItems = typeof(eCategory).GetListOfEnumNames().Select(name => new CategoryCell { Category = (eCategory)Enum.Parse(typeof(eCategory), name) }).ToList();
            CategoriesItems = CategoriesItems.Where(categoryCell => categoryCell.Category.Equals(eCategory.All) == false).ToList();
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(CategoriesViewModel));
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
        }

        private async void categorySelected(CategoryCell current)
        {
            try
            {
                if (current != null)
                {
                    await sr_NavigationService.NavigateTo($"{ShellRoutes.EditJobOffer}?category={current.Category}&isCreate={true}");
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