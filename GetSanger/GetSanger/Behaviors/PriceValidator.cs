using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace GetSanger.Behaviors
{
    public class PriceValidator : BehaviorBase<Entry>
    {
        private static Regex priceValidator = new Regex("^[0-9]*$"); 
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnEntryTextChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= OnEntryTextChanged;
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if(sender is Entry entry)
            {
                if (entry.Text != null && !priceValidator.IsMatch(entry.Text))
                {
                    entry.Text = entry.Text.Remove(entry.Text.Length - 1);
                }
            }
        }
    }
}