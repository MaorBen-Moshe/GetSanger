using GetSanger.Behaviors;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class FilterControl : Frame
    {
        #region Fields
        private const string k_Filter = "Filter by:";
        private const string k_PickerEventName = "SelectedIndexChanged";
        private StackLayout m_Layout;
        private Label m_Label;
        private Picker m_TimePicker;
        private Picker m_CategoryPicker;
        private Picker m_StatusPicker;
        #endregion

        #region TimeBindings
        public List<string> TimeFilterSource
        {
            get => (List<string>)GetValue(TimeFilterSourceProperty);
            set => SetValue(TimeFilterSourceProperty, value);
        }

        public static readonly BindableProperty TimeFilterSourceProperty = BindableProperty.Create(
                                                         propertyName: "TimeFilterSource",
                                                         returnType: typeof(List<string>),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         validateValue: null);

        public int TimeSelectedIndex
        {
            get => (int)GetValue(TimeSelectedIndexProperty);
            set => SetValue(TimeSelectedIndexProperty, value);
        }

        public static readonly BindableProperty TimeSelectedIndexProperty = BindableProperty.Create(
                                                         propertyName: "TimeSelectedIndex",
                                                         returnType: typeof(int),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: -1,
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         validateValue: null);

        public ICommand TimeFilterCommand
        {
            get => (ICommand)GetValue(TimeFilterCommandProperty);
            set => SetValue(TimeFilterCommandProperty, value);
        }

        public static readonly BindableProperty TimeFilterCommandProperty = BindableProperty.Create(
                                                         propertyName: "TimeFilterCommand",
                                                         returnType: typeof(ICommand),
                                                         declaringType: typeof(FilterControl),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: (bindable, oldValue, newValue) => {
                                                             if (bindable is FilterControl filter)
                                                             {
                                                                 filter.setCommand(filter.m_TimePicker, filter.TimeFilterCommand);
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
            BorderColor = Color.Red;
            Padding = 10;
            m_Layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 5
            };

            m_Label = new Label
            {
                Text = k_Filter,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            m_TimePicker = createPicker("Time");
            m_CategoryPicker = createPicker("Category");
            m_StatusPicker = createPicker("Status");

            setPickerBindings(m_TimePicker, nameof(TimeFilterSource), nameof(TimeSelectedIndex), TimeFilterCommand);
            setPickerBindings(m_CategoryPicker, nameof(CategoryFilterSource), nameof(CategorySelectedIndex), CategoryFilterCommand);
            setPickerBindings(m_StatusPicker, nameof(StatusFilterSource), nameof(StatusSelectedIndex), StatusFilterCommand);

            setLayout();
        }

        private Picker createPicker(string i_Title)
        {
            return new Picker
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                Title = i_Title,
                Margin = new Thickness(10, 5)
            };
        }

        private void setPickerBindings(Picker i_Picker, string i_Source, string i_Index, ICommand i_Command)
        {
            i_Picker.SetBinding(Picker.ItemsSourceProperty, new Binding(i_Source, source:this));
            i_Picker.SetBinding(Picker.SelectedIndexProperty, new Binding(i_Index, source: this));
            setCommand(i_Picker, i_Command);
        }

        private void setCommand(Picker i_Picker, ICommand i_Command)
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
            m_Layout.Children.Clear();
            m_Layout.Children.Add(m_Label);
            m_Layout.Children.Add(m_TimePicker);

            if (IsCategoryFilterEnabled)
            {
                m_Layout.Children.Add(m_CategoryPicker);
            }

            if (IsStatusFilterEnabled)
            {
                m_Layout.Children.Add(m_StatusPicker);
            }

            Content = m_Layout;
        }

        #endregion
    }
}