using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GetSanger.Behaviors
{
    public class PhoneNumberMaskBehavior : BehaviorBase<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += Bindable_TextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= Bindable_TextChanged;
        }

        private void Bindable_TextChanged(object sender, TextChangedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                if (args.OldTextValue != null && args.NewTextValue.Length < args.OldTextValue.Length)
                {
                    return;
                }

                var newValue = args.NewTextValue;
                var oldValue = args.OldTextValue;
                if (newValue.Length == 3 || (oldValue != null && oldValue.Length == 3 && newValue.Length == 4))
                {
                    ((Entry)sender).Text += "-";
                    return;
                }

                ((Entry)sender).Text = args.NewTextValue;
            }
        }
    }
}