using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class NoItemsInListTemplate : StackLayout
    {
        private Label m_EmptyLabel;
        private Image m_LogoImage;
        private Label m_RefreshLabel;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
                                                                                propertyName: "PasswordText",
                                                                                returnType: typeof(string),
                                                                                declaringType: typeof(NoItemsInListTemplate),
                                                                                defaultValue: null,
                                                                                defaultBindingMode:BindingMode.TwoWay,
                                                                                validateValue: null,
                                                                                propertyChanged: LabelTextPropertyChanged);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
                                                                                propertyName: "TextColor",
                                                                                returnType: typeof(Color),
                                                                                declaringType: typeof(NoItemsInListTemplate),
                                                                                defaultValue: Color.DarkGray,
                                                                                defaultBindingMode: BindingMode.OneWay,
                                                                                validateValue: null,
                                                                                propertyChanged: (bindable, oldVal, newVal) =>
                                                                                {
                                                                                    if (!oldVal.Equals(newVal))
                                                                                    {
                                                                                        (bindable as NoItemsInListTemplate).updateLabelColor();
                                                                                    }
                                                                                });

        public ICommand RefreshListCommand
        {
            get => (ICommand)GetValue(RefreshListCommandProperty);
            set => SetValue(RefreshListCommandProperty, value);
        }
        
        public static readonly BindableProperty RefreshListCommandProperty = BindableProperty.Create(
                                                                                                    propertyName: "RefreshListCommand",
                                                                                                    returnType: typeof(ICommand),
                                                                                                    declaringType: typeof(NoItemsInListTemplate),
                                                                                                    defaultValue: null,
                                                                                                    defaultBindingMode: BindingMode.OneWay,
                                                                                                    validateValue: null,
                                                                                                    propertyChanged: RefreshCommandPropertyChanged);

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
            /*PasswordText Label Part*/
            m_EmptyLabel = new Label
            {
                FontSize = 20,
                BackgroundColor = Color.Transparent,
            };

            updateLabelText();
            updateLabelColor();
            /*Refresh List Label Part*/
            m_RefreshLabel = new Label
            {
                Text = "Refresh list",
                TextColor = Color.Red,
                BackgroundColor = Color.Transparent,
                FontSize = 14,
                TextTransform = TextTransform.None,
                HorizontalTextAlignment= TextAlignment.Center
            };



            Children.Add(m_LogoImage);
            Children.Add(m_EmptyLabel);
            Children.Add(m_RefreshLabel);
        }

        private static void LabelTextPropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            ((NoItemsInListTemplate)bindable).updateLabelText();
        }

        private void updateLabelText()
        {
            m_EmptyLabel.Text = Text ?? "No Items";
        }

        private void updateLabelColor()
        {
            m_EmptyLabel.TextColor = TextColor;
        }

        private void setImage()
        {
            m_LogoImage = new Image
            {
                BackgroundColor = Color.Transparent,
                Source = ImageSource.FromFile("getSangerIconTransparent.png"),
                HeightRequest = 96,
                WidthRequest = 96
            };
        }

        private static void RefreshCommandPropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            ((NoItemsInListTemplate)bindable).updateRefreshCommand();
        }

        private void updateRefreshCommand()
        {
            m_RefreshLabel.GestureRecognizers.Clear();
            m_RefreshLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = RefreshListCommand
            });
        }
    }
}