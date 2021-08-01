using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public abstract class BaseSearchHandler<T> : SearchHandler
    {
        public ObservableCollection<T> Source
        {
            get => (ObservableCollection<T>)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
                                                         propertyName: "Source",
                                                         returnType: typeof(ObservableCollection<T>),
                                                         declaringType: typeof(BaseSearchHandler<T>),
                                                         defaultValue: null,
                                                         validateValue: null,
                                                         propertyChanged: SourcePropertyChanged);

        private static void SourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is BaseSearchHandler<T> thisInstance) || 
                !(newValue is ObservableCollection<T> newSource))
            {
                return;
            }

            UpdateSource(thisInstance, newSource);
        }

        private static void UpdateSource(BaseSearchHandler<T> searchHandler, IEnumerable<T> newSource)
        {
            if(searchHandler.Source.Equals(newSource) == true)
            {
                return;
            }

            searchHandler.Source.Clear();
            foreach (var current in newSource)
            {
                searchHandler.Source.Add(current);
            }
        }
    }
}