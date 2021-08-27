using Rg.Plugins.Popup.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public abstract class PopupBaseViewModel : BaseViewModel
    {
        #region fields
        #endregion

        #region properties
        #endregion

        #region commands
        public ICommand ExitCommand { get; set; }
        #endregion

        #region constructor
        protected PopupBaseViewModel()
        {
            ExitCommand = new Command(onExit);
        }
        #endregion

        #region methods

        protected async void onExit(object i_Param)
        {
            await PopupNavigation.Instance.PopAsync();
        }
        #endregion
    }
}