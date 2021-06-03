using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class NoItemsListLabel : Label
    {
        public NoItemsListLabel()
        {
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            VerticalOptions = LayoutOptions.CenterAndExpand;
            FontSize = 20;
            BackgroundColor = Color.Transparent;
        }
    }
}
