﻿using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class RoundedImage : StackLayout
    {
        #region Fields
        private Frame m_Frame;
        private Image m_Image;
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
                Padding = 0,
                IsClippedToBounds = true,
                HasShadow = false,
                BorderColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                Content = m_Image = new Image
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Aspect = Aspect.AspectFill
                }
            };

            setRadius();
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
            bindable.m_Image.Source = ImageSource.FromFile("profile.jpg");
            bindable.m_Image.Source = i_NewSource;
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
            m_Image.GestureRecognizers.Clear();
            m_Image.GestureRecognizers.Add(new TapGestureRecognizer
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

            thisInstance.setRadius();
        }

        private void setRadius()
        {
            if(Radius % 2 != 0)
            {
                throw new ArgumentException("Rounded Image expect only even values for the radius property!");
            }

            m_Frame.CornerRadius = Radius / 2;
            m_Frame.HeightRequest = Radius;
            m_Frame.WidthRequest = Radius;

            m_Image.WidthRequest = Radius;
            m_Image.HeightRequest = Radius;
        }

        #endregion
    }
}