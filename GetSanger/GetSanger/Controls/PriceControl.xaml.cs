using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PriceControl : ContentView
    {
        public static BindableProperty MinPriceProperty = BindableProperty.Create(nameof(MinPrice),
                                                                                      typeof(int),
                                                                                      typeof(PriceControl),
                                                                                      defaultBindingMode:BindingMode.TwoWay,
                                                                                      defaultValue:0,
                                                                                      propertyChanged: (bindable, old, newVal) =>
                                                                                      {
                                                                                          if (bindable is PriceControl control)
                                                                                          {
                                                                                              if (newVal is int val)
                                                                                              {
                                                                                                  control.minPrice.Text = val.ToString();
                                                                                              }
                                                                                          }
                                                                                      });

        public static BindableProperty MaxPriceProperty = BindableProperty.Create(nameof(MaxPrice),
                                                                                  typeof(int),
                                                                                  typeof(PriceControl),
                                                                                  defaultBindingMode: BindingMode.TwoWay,
                                                                                  defaultValue: 0,
                                                                                  propertyChanged:(bindable, old, newVal) =>
                                                                                  {
                                                                                      if(bindable is PriceControl control)
                                                                                      {
                                                                                          if(newVal is int val)
                                                                                          {
                                                                                              control.maxPrice.Text = val.ToString();
                                                                                          }
                                                                                      }
                                                                                  });

        public static BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly),
                                                                                typeof(bool),
                                                                                typeof(PriceControl), 
                                                                                defaultValue:false,
                                                                                propertyChanged: (bindable, old, newVal) =>
                                                                                {
                                                                                    if(bindable is PriceControl control)
                                                                                    {
                                                                                        if(newVal is bool val)
                                                                                        {
                                                                                            control.minPrice.IsReadOnly = val;
                                                                                            control.maxPrice.IsReadOnly = val;
                                                                                        }
                                                                                    }
                                                                                });


        public int MinPrice
        {
            get => (int)GetValue(MinPriceProperty);
            set => SetValue(MinPriceProperty, value);
        }

        public int MaxPrice
        {
            get => (int)GetValue(MaxPriceProperty);
            set => SetValue(MaxPriceProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public PriceControl()
        {
            InitializeComponent();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            minPrice.Text = MinPrice.ToString();
            maxPrice.Text = MaxPrice.ToString();
            minPrice.IsReadOnly = IsReadOnly;
            maxPrice.IsReadOnly = IsReadOnly;
        }

        private void minPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool succeeded = int.TryParse(e.NewTextValue, out int parsed);
            if (succeeded)
            {
                MinPrice = parsed;
            }
        }

        private void maxPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool succeeded = int.TryParse(e.NewTextValue, out int parsed);
            if (succeeded)
            {
                MaxPrice = parsed;
            }
        }
    }
}