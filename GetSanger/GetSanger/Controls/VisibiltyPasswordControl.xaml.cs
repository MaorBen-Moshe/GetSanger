using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisibiltyPasswordControl : ContentView
    {
        public static readonly BindableProperty PasswordPlaceHolderProperty
        = BindableProperty.Create(nameof(PasswordPlaceHolder), typeof(string), typeof(VisibiltyPasswordControl), defaultValue: "", defaultBindingMode: BindingMode.TwoWay);

        public string PasswordPlaceHolder
        {
            get => (string)GetValue(PasswordPlaceHolderProperty);
            set => SetValue(PasswordPlaceHolderProperty, value);
        }

        public static readonly BindableProperty PasswordTextProperty
        = BindableProperty.Create(nameof(PasswordText), 
                                  typeof(string), typeof(VisibiltyPasswordControl),
                                  defaultValue:null, 
                                  defaultBindingMode: BindingMode.TwoWay,
                                  propertyChanged: (bindable, oldVal, newVal) =>
                                  {
                                      if(bindable is VisibiltyPasswordControl control)
                                      {
                                          if(newVal is string val)
                                          {
                                              control.PasswordEntry.Text = val;
                                          }
                                      }
                                  });

        public string PasswordText
        {
            get => (string)GetValue(PasswordTextProperty);
            set => SetValue(PasswordTextProperty, value);
        }

        public static BindableProperty CornerRadiusProperty =
             BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(VisibiltyPasswordControl), 8);

        public int CornerRadius
        {
            get => (int)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public EntryWithBorder PasswordEntry
        {
            get => m_PasswordEntry;
        }

        public VisibiltyPasswordControl()
        {
            InitializeComponent();
            PasswordEntry.TextChanged += (sender, args) => PasswordText = args.NewTextValue;
            PasswordText = "";
            PasswordPlaceHolder = "";
        }


        protected override void OnParentSet()
        {
            base.OnParentSet();
            PasswordEntry.Text = PasswordText;
            PasswordEntry.CornerRadius = CornerRadius;
            PasswordEntry.Placeholder = PasswordPlaceHolder;
        }

        private void ImageButton_Clicked(object sender, System.EventArgs e)
        {
            if (PasswordEntry.IsPassword)
            {
                visibilityImage.Source = ImageSource.FromFile("visibility.png");
                PasswordEntry.IsPassword = false;
            }
            else
            {
                visibilityImage.Source = ImageSource.FromFile("noVisibility.png");
                PasswordEntry.IsPassword = true;
            }
        }
    }
}