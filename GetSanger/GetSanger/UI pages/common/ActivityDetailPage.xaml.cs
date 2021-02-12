using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.UI_pages.common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityDetailPage : ContentPage
    {
        public ActivityDetailPage()
        {
            InitializeComponent();

            m_jobEditor.Keyboard = Keyboard.Create(KeyboardFlags.Suggestions | KeyboardFlags.Spellcheck | KeyboardFlags.CapitalizeSentence);
        }
    }
}