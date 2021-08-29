using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class ColoredTableView : TableView
    {
        public static BindableProperty SeparatorColorProperty = BindableProperty.Create(nameof(SeparatorColor),
                                                                                        typeof(Color),
                                                                                        typeof(ColoredTableView), 
                                                                                        Color.White);
        public Color SeparatorColor
        {
            get => (Color)GetValue(SeparatorColorProperty);
            set => SetValue(SeparatorColorProperty, value);
        }

        public static BindableProperty GroupHeaderColorProperty = BindableProperty.Create(nameof(GroupHeaderColor),
                                                                                          typeof(Color), 
                                                                                          typeof(ColoredTableView),
                                                                                          Color.White);
        public Color GroupHeaderColor
        {
            get => (Color)GetValue(GroupHeaderColorProperty);
            set => SetValue(GroupHeaderColorProperty, value);
        }
    }
}