using System.ComponentModel;
using GetSanger.Controls;
using GetSanger.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ColoredTableView), typeof(ColoredTableViewRenderer))]
namespace GetSanger.iOS.Renderers
{
    public class ColoredTableViewRenderer : TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
                return;

            var tableView = Control as UITableView;
            var coloredTableView = Element as ColoredTableView;
            tableView.SeparatorColor = coloredTableView.SeparatorColor.ToUIColor();
            tableView.WeakDelegate = new CustomHeaderTableModelRenderer(coloredTableView);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == ColoredTableView.SeparatorColorProperty.PropertyName)
            {
                var tableView = Control as UITableView;
                var coloredTableView = Element as ColoredTableView;
                tableView.SeparatorColor = coloredTableView.SeparatorColor.ToUIColor();
            }
        }

        private class CustomHeaderTableModelRenderer : UnEvenTableViewModelRenderer
        {
            private readonly ColoredTableView _coloredTableView;
            public CustomHeaderTableModelRenderer(TableView model) : base(model)
            {
                _coloredTableView = model as ColoredTableView;
            }
            public override UIView GetViewForHeader(UITableView tableView, System.nint section)
            {
                return new UILabel()
                {
                    Text = TitleForHeader(tableView, section),
                    TextColor = _coloredTableView.GroupHeaderColor.ToUIColor(),
                    TextAlignment = UITextAlignment.Center
                };
            }
        }
    }
}