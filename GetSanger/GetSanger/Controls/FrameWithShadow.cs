using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class FrameWithShadow : Frame
    {
        public static BindableProperty ElevationProperty =
                                        BindableProperty.Create(nameof(Elevation), typeof(float), typeof(FrameWithShadow), 30f);

        public static BindableProperty ZProperty =
                                        BindableProperty.Create(nameof(Z), typeof(float), typeof(FrameWithShadow), 30f);

        public float Elevation
        {
            get => (float)GetValue(ElevationProperty);
            set => SetValue(ElevationProperty, value);
        }

        public float Z
        {
            get => (float)GetValue(ZProperty);
            set => SetValue(ZProperty, value);
        }
    }
}