using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class RoundedImage : StackLayout
    {
        private Frame m_Frame;
        private Image m_Image;

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
                                                         propertyName: "Command",
                                                         returnType: typeof(ICommand),
                                                         declaringType: typeof(RoundedImage),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: CommandPropertyChanged);

        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
                                                         propertyName: "CommandParameter",
                                                         returnType: typeof(object),
                                                         declaringType: typeof(RoundedImage),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: CommandParameterPropertyChanged);

        // Please use only even numbers!!
        public int Radius
        {
            get => (int)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        public static readonly BindableProperty RadiusProperty = BindableProperty.Create(
                                                         propertyName: "Radius",
                                                         returnType: typeof(int),
                                                         declaringType: typeof(RoundedImage),
                                                         defaultValue: 36,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: RadiusPropertyChanged);

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
                                                         propertyName: "ImageSource",
                                                         returnType: typeof(ImageSource),
                                                         declaringType: typeof(RoundedImage),
                                                         defaultValue: ImageSource.FromFile("profile.png"),
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: imagePropertyChanged);

        public RoundedImage()
        {
            setView();
        }

        private void setView()
        {
            Children.Clear();
            m_Frame = new Frame
            {
                CornerRadius = Radius / 2,
                Padding = 0,
                HeightRequest = Radius,
                WidthRequest = Radius,
                IsClippedToBounds = true,
                HasShadow = false,
                BorderColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                Content = m_Image = new Image
                {
                    WidthRequest = Radius,
                    HeightRequest = Radius,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Aspect = Aspect.AspectFill
                }
            };

            setImage(this, ImageSource);
            m_Image.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = Command,
                CommandParameter = CommandParameter
            });

            Children.Add(m_Frame);
        }

        private static void imagePropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (!(bindable is RoundedImage thisInstance) || !(newValue is ImageSource newImageSource))
            {
                return;
            }

            setImage(thisInstance, newImageSource);
        }

        private static void setImage(RoundedImage bindable, ImageSource i_NewSource)
        {
            (bindable.m_Frame.Content as Image).Source = i_NewSource;
        }

        private static void CommandPropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (!(bindable is RoundedImage thisInstance) || !(newValue is ICommand newCommand))
            {
                return;
            }

            thisInstance.setComanndAndParam(newCommand, thisInstance.CommandParameter);
        }

        private static void CommandParameterPropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (!(bindable is RoundedImage thisInstance) || !(newValue is object newParam))
            {
                return;
            }

            thisInstance.setComanndAndParam(thisInstance.Command, newParam);
        }

        private void setComanndAndParam(ICommand i_Command, object i_Param)
        {
            var image = m_Frame.Content as Image;
            image.GestureRecognizers.Clear();
            image.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = i_Command,
                CommandParameter = i_Param
            });
        }

        private static void RadiusPropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (!(bindable is RoundedImage thisInstance) || !(newValue is int val))
            {
                return;
            }

            thisInstance.setView();
        }
    }
}