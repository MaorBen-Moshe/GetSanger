using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Android.Factories;
using AndroidBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;
using AndroidBitmapDescriptorFactory = Android.Gms.Maps.Model.BitmapDescriptorFactory;

namespace GetSanger.Droid.Services
{
    public class BitmapConfig : IBitmapDescriptorFactory
    {
        public AndroidBitmapDescriptor ToNative(BitmapDescriptor descriptor)
        {
            int iconId = 0;
            switch (descriptor.Id)
            {
                case "getSangerIconHD":
                    iconId = Resource.Drawable.getSangerIconHD;
                    break;
                case "getSangerIconTransparent":
                    iconId = Resource.Drawable.getSangerIconTransparent;
                    break;
            }

            return AndroidBitmapDescriptorFactory.FromResource(iconId);
        }
    }
}