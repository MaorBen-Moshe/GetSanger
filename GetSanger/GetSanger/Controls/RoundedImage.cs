using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using System;
using System.Windows.Input;
using GetSanger.Extensions;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class RoundedImage : StackLayout
    {
        #region Fields
        private Frame m_Frame;
        private CachedImage m_CachedImage;
        //private Image m_Image;
        #endregion

        #region Properties
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
            get => GetValue(CommandParameterProperty);
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
            get => (Xamarin.Forms.ImageSource)GetValue(ImageSourceProperty);
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

        public bool IsCacheEnable
        {
            get => (bool)GetValue(IsCacheEnableProperty);
            set => SetValue(IsCacheEnableProperty, value);
        }

        public static readonly BindableProperty IsCacheEnableProperty = BindableProperty.Create(
                                                                propertyName:"IsCacheEnable",
                                                                returnType: typeof(bool),
                                                                declaringType: typeof(RoundedImage),
                                                                defaultValue: true,
                                                                defaultBindingMode: BindingMode.OneWay,
                                                                validateValue: null,
                                                                propertyChanged: cachePropertyChanged);

        #endregion

        #region Constructor

        public RoundedImage()
        {
            setView();
        }

        #endregion

        #region Methods

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
                Content = m_CachedImage = new CachedImage
                {
                    WidthRequest = Radius,
                    HeightRequest = Radius,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Aspect = Aspect.AspectFill,
                    FadeAnimationEnabled = true,
                    BitmapOptimizations = true,
                    LoadingPlaceholder = ImageSource.FromFile("rolling.gif"),
                    ErrorPlaceholder = ImageSource.FromFile("profile.jpg")
                }
            };


            setCache();
            setImage(this, ImageSource);
            m_CachedImage.GestureRecognizers.Add(new TapGestureRecognizer
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

        private async static void setImage(RoundedImage bindable, ImageSource i_NewSource)
        {
            if (!bindable.IsCacheEnable)
            {
                await ImageService.Instance.InvalidateCacheAsync(CacheType.All);
            }

            bindable.m_CachedImage.Source = i_NewSource;
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
            m_CachedImage.GestureRecognizers.Clear();
            m_CachedImage.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = i_Command,
                CommandParameter = i_Param
            });
        }

        private static void RadiusPropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (!(bindable is RoundedImage thisInstance) || !(newValue is int))
            {
                return;
            }

            thisInstance.setView();
        }

        private static void cachePropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (!(bindable is RoundedImage thisInstance) || !(newValue is bool val))
            {
                return;
            }

            thisInstance.setView();
        }

        private void setCache()
        {
            if (IsCacheEnable)
            {
                m_CachedImage.CacheType = CacheType.All;
                m_CachedImage.CacheDuration = new TimeSpan(1, 0, 0, 0);
            }
            else
            {
                m_CachedImage.CacheType = CacheType.Memory;
                m_CachedImage.CacheDuration = new TimeSpan(0);
            }
        }

        #endregion
    }
}