using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomSwitchCell : ViewCell
    {
        public event Action<object, ToggledEventArgs> OnChanged;


        public static readonly BindableProperty OnProperty = BindableProperty.Create(nameof(On),
                                                                                     typeof(bool),
                                                                                     typeof(CustomSwitchCell),
                                                                                     false,
                                                                                     defaultBindingMode: BindingMode.TwoWay,
                                                                                     propertyChanged: onChanged);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text),
                                                                                       typeof(string),
                                                                                       typeof(CustomSwitchCell),
                                                                                       default,
                                                                                       defaultBindingMode: BindingMode.TwoWay,
                                                                                       propertyChanged: textChanged);


        public bool On
        {
            get => (bool)GetValue(OnProperty);
            set => SetValue(OnProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        
        public CustomSwitchCell()
        {
            InitializeComponent();

            BindingContext = this;
            toggle.BindingContext = this;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            label.Text = Text;
            toggle.IsToggled = On;
        }

        private static void onChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomSwitchCell control)
            {
                if (newValue is bool val)
                {
                    control.toggle.IsToggled = val;
                    control.OnChanged?.Invoke(control, new ToggledEventArgs(val));
                }
            }
        }

        private static void textChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomSwitchCell control)
            {
                if (newValue is string val)
                {
                    control.label.Text = val;
                }
            }
        }
    }
}