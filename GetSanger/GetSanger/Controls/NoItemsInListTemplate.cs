using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class NoItemsInListTemplate : StackLayout
    {
        private Label m_EmptyLabel;
        private Image m_LogoImage;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                                                propertyName: "Text",
                                                                                returnType: typeof(string),
                                                                                declaringType: typeof(NoItemsInListTemplate),
                                                                                defaultValue: null,
                                                                                defaultBindingMode:BindingMode.TwoWay,
                                                                                validateValue: null,
                                                                                propertyChanged: LabelTextPropertyChanged);

        public NoItemsInListTemplate()
        {
            generateDisplay();
        }

        private void generateDisplay()
        {
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            VerticalOptions = LayoutOptions.CenterAndExpand;
            /*Image Part*/
            setImage();
            /*Label Part*/
            m_EmptyLabel = new Label
            {
                FontSize = 20,
                BackgroundColor = Color.Transparent,
                TextColor = Color.DarkGray
            };
            updateLabelText();

            this.Children.Add(m_LogoImage);
            this.Children.Add(m_EmptyLabel);
        }

        private static void LabelTextPropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            ((NoItemsInListTemplate)bindable).updateLabelText();
        }

        private void updateLabelText()
        {
            m_EmptyLabel.Text = Text ?? "No Items";
        }

        private void setImage()
        {
            m_LogoImage = new Image
            {
                BackgroundColor = Color.Transparent,
                Source = ImageSource.FromFile("getSangerIconHD.png"),
                HeightRequest = 96,
                WidthRequest = 96
            };
        }
    }
}
