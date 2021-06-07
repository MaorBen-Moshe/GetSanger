﻿using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class CategoriesTableView : TableView
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
                                  BindingMode.TwoWay,
                                  propertyChanged: IsGenericNotificationsOnChanged
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
                new TableSection("Categories")
            };

            setGenericView();
            setCategoriesView();
        }

        #endregion

        #region Methods
        private void setGenericView()
        {
            SwitchCell genericCell = new SwitchCell
            {
                BindingContext = this,
                Text = "Notifications",
            };

            genericCell.SetBinding(SwitchCell.OnProperty, new Binding("IsGenericNotifications"));
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
                SwitchCell sc = new SwitchCell { BindingContext = categoryCell };
                sc.SetBinding(SwitchCell.TextProperty, new Binding("Category"));
                sc.SetBinding(SwitchCell.OnProperty, new Binding("Checked"));
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

            SwitchCell current = sender as SwitchCell;
            CategoryCell newCell = new CategoryCell
            {
                Category = (eCategory)Enum.Parse(typeof(eCategory), current.Text),
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

        private static void IsGenericNotificationsOnChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var binding = bindable as CategoriesTableView;
            var cell = binding.Root[0][0] as SwitchCell;
            if(oldValue == null || newValue == null || binding.ToggledCommand == null)
            {
                return;
            }

            bool oldVal = (bool)oldValue;
            bool newVal = (bool)newValue;

            if(oldVal != newVal)
            {
                if (binding.ToggledCommand.CanExecute(cell))
                {
                    binding.ToggledCommand.Execute(cell);
                }
            }
        }

        #endregion
    }
}
