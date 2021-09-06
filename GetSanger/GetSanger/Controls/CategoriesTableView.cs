using GetSanger.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class CategoriesTableView : ColoredTableView
    {
        #region BindableProperties
        public static BindableProperty SourceProperty
           = BindableProperty.Create(nameof(Source),
                                     typeof(ObservableCollection<CategoryCell>),
                                     typeof(CategoriesTableView),
                                     default,
                                     propertyChanged: SourceOnCollectionChanged
                                     );

        public static BindableProperty IsGenericNotificationsProperty
        = BindableProperty.Create(nameof(IsGenericNotifications),
                                  typeof(bool),
                                  typeof(CategoriesTableView),
                                  false,
                                  BindingMode.TwoWay
                                  );
        public static BindableProperty ToggledCommandProperty
        = BindableProperty.Create(nameof(ToggledCommand),
                                  typeof(ICommand),
                                  typeof(CategoriesTableView),
                                  default
                                  );
        #endregion

        #region Properties
        public ObservableCollection<CategoryCell> Source
        {
            get => (ObservableCollection<CategoryCell>)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public bool IsGenericNotifications
        {
            get => (bool)GetValue(IsGenericNotificationsProperty);
            set => SetValue(IsGenericNotificationsProperty, value);
        }

        #endregion

        #region Commands
        public ICommand ToggledCommand
        {
            get => (ICommand)GetValue(ToggledCommandProperty);
            set => SetValue(ToggledCommandProperty, value);
        }

        #endregion

        #region Constructor
        public CategoriesTableView()
        {
            Intent = TableIntent.Settings;
            Root = new TableRoot()
            {
                new TableSection("Generic Notifications"),
                new TableSection("My Sanger Settings")
            };

            setGenericView();
            setCategoriesView();
        }

        #endregion

        #region Methods
        private void setGenericView()
        {
            CustomSwitchCell genericCell = new CustomSwitchCell
            {
                BindingContext = this,
                Text = "Notifications",
                ImageString = "bell.png"
            };

            genericCell.SetBinding(CustomSwitchCell.OnProperty, new Binding("IsGenericNotifications"));
            Root[0].Clear();
            Root[0].Add(genericCell);
        }

        private void setCategoriesView()
        {
            // Categories Section
            if(Source == null)
            {
                return;
            }

            var categoriesSection = Root[1];
            categoriesSection.Clear();
            foreach (var categoryCell in Source)
            {
                CustomSwitchCell sc = new CustomSwitchCell {
                    BindingContext = categoryCell,
                    ImageString = categoryCell.ImageUri
                };

                sc.SetBinding(CustomSwitchCell.TextProperty, new Binding("CategoryString"));
                sc.SetBinding(CustomSwitchCell.OnProperty, new Binding("Checked"));
                sc.OnChanged += CategoriesSwitchCell_OnChanged;
                categoriesSection.Add(sc);
            }
        }

        private void CategoriesSwitchCell_OnChanged(object sender, ToggledEventArgs e)
        {
            if(ToggledCommand == null)
            {
                return;
            }

            CustomSwitchCell current = sender as CustomSwitchCell;
            CategoryCell newCell = new CategoryCell
            {
                Category = (eCategory)Enum.Parse(typeof(eCategory), current.Text.Trim().Replace(" ", "_")),
                Checked = current.On
            };

            if (ToggledCommand.CanExecute(newCell))
            {
                ToggledCommand.Execute(newCell);
            }
        }

        private static void SourceOnCollectionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as CategoriesTableView)?.setCategoriesView();
        }

        #endregion
    }
}