using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using GetSanger.Controls;
using GetSanger.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ColoredTableView), typeof(ColoredTableViewRenderer))]
namespace GetSanger.Droid.Renderers
{
    public class ColoredTableViewRenderer : TableViewRenderer
    {
        public ColoredTableViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
                return;

            var listView = Control as Android.Widget.ListView;
            var coloredTableView = (ColoredTableView)Element;
            listView.Divider = new ColorDrawable(coloredTableView.SeparatorColor.ToAndroid());
            listView.DividerHeight = 3;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == ColoredTableView.SeparatorColorProperty.PropertyName)
            {
                helper(Control, Element);
            }
        }

        protected override TableViewModelRenderer GetModelRenderer(Android.Widget.ListView listView, TableView view)
        {
            helper(listView, view);
            return new CustomHeaderTableViewModelRenderer(Context, listView, view);
        }

        private void helper(Android.Widget.ListView listView, TableView view)
        {
            var coloredTableView = (ColoredTableView)view;
            listView.Divider = new ColorDrawable(coloredTableView.SeparatorColor.ToAndroid());
        }

        private class CustomHeaderTableViewModelRenderer : TableViewModelRenderer
        {
            private readonly ColoredTableView _coloredTableView;

            public CustomHeaderTableViewModelRenderer(Context context, Android.Widget.ListView listView, TableView view) : base(context, listView, view)
            {
                _coloredTableView = view as ColoredTableView;
            }

            public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
            {
                var view = base.GetView(position, convertView, parent);

                var element = GetCellForPosition(position);

                // section header will be a TextCell
                if (element.GetType() == typeof(TextCell))
                {
                    try
                    {
                        // Get the textView of the actual layout
                        var textView = ((((view as LinearLayout).GetChildAt(0) as LinearLayout).GetChildAt(1) as LinearLayout).GetChildAt(0) as TextView);

                        // get the divider below the header
                        var divider = (view as LinearLayout).GetChildAt(1);

                        // Set the color
                        textView.SetTextColor(_coloredTableView.GroupHeaderColor.ToAndroid());
                        textView.TextAlignment = Android.Views.TextAlignment.ViewStart;
                        textView.Gravity = GravityFlags.Start;
                        divider.SetBackgroundColor(_coloredTableView.GroupHeaderColor.ToAndroid());
                    }
                    catch (Exception) { }
                }

                return view;
            }
        }
    }
}