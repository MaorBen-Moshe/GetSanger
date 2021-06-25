using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class RoundedImage : StackLayout
    {
        private Frame m_Frame;
        private Image m_Image;
        private ImageButton m_ImageButton;

        public bool IsImageButton { get; set; }

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

        public int ImageWidth
        {
            get => (int)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        public static readonly BindableProperty ImageWidthProperty = BindableProperty.Create(
                                                         propertyName: "ImageWidth",
                                                         returnType: typeof(int),
                                                         declaringType: typeof(RoundedImage),
                                                         defaultValue: 20,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: WidthHeightPropertyChanged);

        public int ImageHeight
        {
            get => (int)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }

        public static readonly BindableProperty ImageHeightProperty = BindableProperty.Create(
                                                         propertyName: "ImageHeight",
                                                         returnType: typeof(int),
                                                         declaringType: typeof(RoundedImage),
                                                         defaultValue: 20,
                                                         defaultBindingMode: BindingMode.OneWay,
                                                         validateValue: null,
                                                         propertyChanged: WidthHeightPropertyChanged);

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
                CornerRadius = 100,
                Padding = 0,
                HeightRequest = ImageHeight,
                WidthRequest = ImageWidth,
                IsClippedToBounds = true,
                HasShadow = false,
                BorderColor = Color.Transparent
            };

            if (IsImageButton)
            {
                m_ImageButton = new ImageButton
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                m_ImageButton.Command = Command;
                m_ImageButton.CommandParameter = CommandParameter;
                m_Frame.Content = m_ImageButton;
            }
            else
            {
                m_Image = new Image
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                m_Image.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = Command,
                    CommandParameter = CommandParameter
                });

                m_Frame.Content = m_Image;
            }

            setImage(this, ImageSource);
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
            if (bindable.IsImageButton)
            {
                (bindable.m_Frame.Content as ImageButton).Source = i_NewSource;
            }
            else
            {
                (bindable.m_Frame.Content as Image).Source = i_NewSource;
            }
        }

        private static void CommandPropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (!(bindable is RoundedImage thisInstance) || !(newValue is ICommand newCommand))
            {
                return;
            }

            setCommand(thisInstance, newCommand);
        }

        private static void setCommand(RoundedImage bindable, ICommand i_Command)
        {
            if (bindable.IsImageButton)
            {
                (bindable.m_Frame.Content as ImageButton).Command = i_Command;
            }
            else
            {
                var image = (bindable.m_Frame.Content as Image);
                if (image.GestureRecognizers.Count > 0)
                {
                    foreach(var current in image.GestureRecognizers)
                    {
                        if(current is TapGestureRecognizer)
                        {
                            (current as TapGestureRecognizer).Command = i_Command;
                            break;
                        }
                    }
                }
            }
        }

        private static void CommandParameterPropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (!(bindable is RoundedImage thisInstance) || !(newValue is object newParam))
            {
                return;
            }

            setCommandParameter(thisInstance, newParam);
        }

        private static void setCommandParameter(RoundedImage bindable, object i_Param)
        {
            if (bindable.IsImageButton)
            {
                (bindable.m_Frame.Content as ImageButton).CommandParameter = i_Param;
            }
            else
            {
                var image = (bindable.m_Frame.Content as Image);
                if (image.GestureRecognizers.Count > 0)
                {
                    foreach (var current in image.GestureRecognizers)
                    {
                        if (current is TapGestureRecognizer)
                        {
                            (current as TapGestureRecognizer).CommandParameter = i_Param;
                            break;
                        }
                    }
                }
            }
        }

        private static void WidthHeightPropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (!(bindable is RoundedImage thisInstance) || !(newValue is int))
            {
                return;
            }

            thisInstance.setView();
        }
    }
}