﻿using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SocialButton : ContentView
    {
        public event EventHandler Clicked;
        public static readonly BindableProperty ButtonText
        = BindableProperty.Create(nameof(Text), typeof(string), typeof(SocialButton));

        public string Text
        {
            get => (string)GetValue(ButtonText);
            set => SetValue(ButtonText, value);
        }

        public static readonly BindableProperty ButtonImage
        = BindableProperty.Create(nameof(Image), typeof(string), typeof(SocialButton));

        public string Image
        {
            get => (string)GetValue(ButtonImage);
            set => SetValue(ButtonImage, value);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command),
        typeof(ICommand), typeof(SocialButton), null);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty CommandPropertyParameter = BindableProperty.Create(nameof(CommandParameter),
        typeof(object), typeof(SocialButton), null);

        public object CommandParameter
        {
            get => GetValue(CommandPropertyParameter);
            set => SetValue(CommandPropertyParameter, value);
        }
        public SocialButton()
        {
            InitializeComponent();

            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => {
                    Clicked?.Invoke(this, EventArgs.Empty);
                    if (Command != null)
                    {
                        if (Command.CanExecute(CommandParameter))
                            Command.Execute(CommandParameter);
                    }
                })

            });
        }
        protected override void OnParentSet()
        {
            base.OnParentSet();
            btnIcon.Source = Image; // ImageSource.FromFile(Image);
            btnText.Text = Text;
        }

    }
}