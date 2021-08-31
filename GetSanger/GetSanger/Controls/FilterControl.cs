using GetSanger.Behaviors;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class FilterControl : StackLayout
    {
        #region Fields
        private const string k_Filter = "Filter by:";
        private const string k_PickerEventName = "SelectedIndexChanged";
        private Label m_FilterLabel;
        private StackLayout m_PickersStack;
        private ImageButton m_SortButton;
        private PickerWithBorder m_CategoryPicker;
        private PickerWithBorder m_StatusPicker;
        #endregion

        #region TimeBindings
        public bool TimeSortFlag // false means descending;
        {
            get => (bool)GetValue(TimeSortFlagProperty);
            set => SetValue(TimeSortFlagProperty, value);
        }

        public static readonly BindableProperty TimeSortFlagProperty = BindableProperty.Create(
                                                         propertyName: "TimeSortFlag",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: (bindable, oldValue, newValue) => {
                                                             if (bindable is FilterControl filter)
                                                             {
                                                                 int angle = filter.TimeSortFlag ? 180 : 0;
                                                                 filter.m_SortButton.RotateTo(angle, 500, Easing.Linear);
                                                             }
                                                         });

        public ICommand TimeSortCommand
        {
            get => (ICommand)GetValue(TimeSortCommandProperty);
            set => SetValue(TimeSortCommandProperty, value);
        }

        public static readonly BindableProperty TimeSortCommandProperty = BindableProperty.Create(
                                                         propertyName: "TimeSortCommand",
                                                         returnType: typeof(ICommand),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: (bindable, oldValue, newValue) => {
                                                             if (bindable is FilterControl filter)
                                                             {
                                                                 filter.m_SortButton.Command = filter.TimeSortCommand;
                                                             }
                                                         });

        #endregion

        #region CategoryBindings

        public List<string> CategoryFilterSource
        {
            get => (List<string>)GetValue(CategoryFilterSourceProperty);
            set => SetValue(CategoryFilterSourceProperty, value);
        }

        public static readonly BindableProperty CategoryFilterSourceProperty = BindableProperty.Create(
                                                         propertyName: "CategoryFilterSource",
                                                         returnType: typeof(List<string>),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         validateValue: null);

        public int CategorySelectedIndex
        {
            get => (int)GetValue(CategorySelectedIndexProperty);
            set => SetValue(CategorySelectedIndexProperty, value);
        }

        public static readonly BindableProperty CategorySelectedIndexProperty = BindableProperty.Create(
                                                         propertyName: "CategorySelectedIndex",
                                                         returnType: typeof(int),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: -1,
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         validateValue: null);

        public ICommand CategoryFilterCommand
        {
            get => (ICommand)GetValue(CategoryFilterCommandProperty);
            set => SetValue(CategoryFilterCommandProperty, value);
        }

        public static readonly BindableProperty CategoryFilterCommandProperty = BindableProperty.Create(
                                                         propertyName: "CategoryFilterCommand",
                                                         returnType: typeof(ICommand),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: (bindable, oldValue, newValue) => {
                                                             if (bindable is FilterControl filter)
                                                             {
                                                                 filter.setCommand(filter.m_CategoryPicker, filter.CategoryFilterCommand);
                                                             }
                                                         });

        #endregion

        #region StatusBindings 

        public List<string> StatusFilterSource
        {
            get => (List<string>)GetValue(StatusFilterSourceProperty);
            set => SetValue(StatusFilterSourceProperty, value);
        }

        public static readonly BindableProperty StatusFilterSourceProperty = BindableProperty.Create(
                                                         propertyName: "StatusFilterSource",
                                                         returnType: typeof(List<string>),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null);

        public int StatusSelectedIndex
        {
            get => (int)GetValue(StatusSelectedIndexProperty);
            set => SetValue(StatusSelectedIndexProperty, value);
        }

        public static readonly BindableProperty StatusSelectedIndexProperty = BindableProperty.Create(
                                                         propertyName: "StatusSelectedIndex",
                                                         returnType: typeof(int),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: -1,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null);

        public ICommand StatusFilterCommand
        {
            get => (ICommand)GetValue(StatusFilterCommandProperty);
            set => SetValue(StatusFilterCommandProperty, value);
        }

        public static readonly BindableProperty StatusFilterCommandProperty = BindableProperty.Create(
                                                         propertyName: "StatusFilterCommand",
                                                         returnType: typeof(ICommand),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: (bindable, oldValue, newValue) => {
                                                             if (bindable is FilterControl filter)
                                                             {
                                                                 filter.setCommand(filter.m_StatusPicker, filter.StatusFilterCommand);
                                                             }
                                                         });

        #endregion

        #region GenericBindings 

        public bool IsCategoryFilterEnabled
        {
            get => (bool)GetValue(IsCategoryFilterEnabledProperty);
            set => SetValue(IsCategoryFilterEnabledProperty, value);
        }

        public static readonly BindableProperty IsCategoryFilterEnabledProperty = BindableProperty.Create(
                                                         propertyName: "IsCategoryFilterEnabled",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: GenericPropertyChanged);

        public bool IsStatusFilterEnabled
        {
            get => (bool)GetValue(IsStatusFilterEnabledProperty);
            set => SetValue(IsStatusFilterEnabledProperty, value);
        }

        public static readonly BindableProperty IsStatusFilterEnabledProperty = BindableProperty.Create(
                                                         propertyName: "IsStatusFilterEnabled",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: GenericPropertyChanged);

        private static void GenericPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is FilterControl filterControl)
            {
                filterControl.setLayout();
            }
        }

        #endregion

        #region Constructor
        public FilterControl()
        {
            setView(); 
        }
        #endregion

        #region Methods
        private void setView()
        {
            Orientation = StackOrientation.Horizontal;
            Spacing = 2;

            m_PickersStack = new StackLayout
            {
                Spacing = 5,
                Orientation = StackOrientation.Vertical
            };

            m_FilterLabel = new Label
            {
                Text = k_Filter,
                VerticalOptions = LayoutOptions.Center
            };

            m_SortButton = new ImageButton
            {
                Margin = new Thickness(10, 5),
                BackgroundColor = Color.Transparent,
                Command = TimeSortCommand,
                WidthRequest = 32,
                HeightRequest = 32
            };

            setImageSource();
            Application.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
            m_CategoryPicker = createPicker("Category");
            m_StatusPicker = createPicker("Status");

            setPickerBindings(m_CategoryPicker, nameof(CategoryFilterSource), nameof(CategorySelectedIndex), CategoryFilterCommand);
            setPickerBindings(m_StatusPicker, nameof(StatusFilterSource), nameof(StatusSelectedIndex), StatusFilterCommand);

            setLayout();
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            setImageSource();
        }

        private void setImageSource()
        {
            string source = Application.Current.RequestedTheme switch
            {
                OSAppTheme.Dark => "ascendingSortDarkMode.png",
                _ => "ascendingSort.png",
            };

            m_SortButton.Source = ImageSource.FromFile(source);
        }

        private PickerWithBorder createPicker(string i_Title)
        {
            return new PickerWithBorder
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                Title = i_Title,
            };
        }

        private void setPickerBindings(PickerWithBorder i_Picker, string i_Source, string i_Index, ICommand i_Command)
        {
            i_Picker.SetBinding(Picker.ItemsSourceProperty, new Binding(i_Source, source:this));
            i_Picker.SetBinding(Picker.SelectedIndexProperty, new Binding(i_Index, source: this));
            setCommand(i_Picker, i_Command);
        }

        private void setCommand(PickerWithBorder i_Picker, ICommand i_Command)
        {
            i_Picker.Behaviors.Clear();
            i_Picker.Behaviors.Add(new EventToCommandBehavior
            {
                Command = i_Command,
                EventName = k_PickerEventName
            });
        }

        private void setLayout()
        {
            Children.Clear();
            Children.Add(m_SortButton);

            if(IsCategoryFilterEnabled || IsStatusFilterEnabled)
            {
                m_PickersStack.Children.Clear();
                Children.Add(m_FilterLabel);
                if (IsCategoryFilterEnabled)
                {
                    m_PickersStack.Children.Add(m_CategoryPicker);
                }

                if (IsStatusFilterEnabled)
                {
                    m_PickersStack.Children.Add(m_StatusPicker);
                }


                Children.Add(m_PickersStack);
            }
        }

        #endregion
    }
}