using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GetSanger.Controls
{
    public class BindableMap : Map
    {
        public ObservableCollection<Pin> PinsSource
        {
            get => (ObservableCollection<Pin>)GetValue(PinsSourceProperty);
            set => SetValue(PinsSourceProperty, value);
        }

        public static readonly BindableProperty PinsSourceProperty = BindableProperty.Create(
                                                         propertyName: "PinsSource",
                                                         returnType: typeof(ObservableCollection<Pin>),
                                                         declaringType: typeof(BindableMap),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         validateValue: null,
                                                         propertyChanged: PinsSourcePropertyChanged);

        public MapSpan MapSpan
        {
            get => (MapSpan)GetValue(MapSpanProperty); 
            set => SetValue(MapSpanProperty, value); 
        }

        public static readonly BindableProperty MapSpanProperty = BindableProperty.Create(
                                                         propertyName: "MapSpan",
                                                         returnType: typeof(MapSpan),
                                                         declaringType: typeof(BindableMap),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         validateValue: null,
                                                         propertyChanged: MapSpanPropertyChanged);

        public BindableMap() : base()
        {
            PinsSource = new ObservableCollection<Pin>();
            PinsSource.CollectionChanged += PinsSourceOnCollectionChanged;
        }

        private static void MapSpanPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisInstance = bindable as BindableMap;
            var newMapSpan = newValue as Xamarin.Forms.GoogleMaps.MapSpan;
            
            if(newMapSpan == null)
            {
                return;
            }

            thisInstance?.MoveToRegion(newMapSpan);
        }
        private static void PinsSourcePropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var thisInstance = bindable as BindableMap;
            var newPinsSource = newValue as ObservableCollection<Pin>;

            if (thisInstance == null || newPinsSource == null)
            {
                return;
            }

            UpdatePinsSource(thisInstance, newPinsSource);
        }
        private void PinsSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePinsSource(this, sender as IEnumerable<Pin>);
        }

        private static void UpdatePinsSource(Map bindableMap, IEnumerable<Pin> newSource)
        {
            bindableMap.Pins.Clear();
            foreach (var pin in newSource)
                bindableMap.Pins.Add(pin);
        }
    }
}
