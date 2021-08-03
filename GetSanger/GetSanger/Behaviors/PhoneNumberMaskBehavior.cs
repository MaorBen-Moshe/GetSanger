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
                if (newValue.Length == 3)
                { 
                    ((Entry)sender).Text = newValue.Insert(3, "-");
                    return;
                }

                if(newValue.Length == 4)
                {
                    if(newValue.IndexOf("-") != 3)
                    {
                        ((Entry)sender).Text = newValue.Insert(3, "-");
                        return;
                    }
                }

                string newWithoutChar = newValue.Replace("-", "");
                if((newValue.Length - newWithoutChar.Length) > 1)
                {
                    newValue = newValue.Replace("-", "");
                    ((Entry)sender).Text = newValue.Insert(3, "-");
                    return;
                }

                ((Entry)sender).Text = args.NewTextValue;
            }
        }
    }
}