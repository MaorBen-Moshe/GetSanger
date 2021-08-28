using Rg.Plugins.Popup.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class PromptViewModel : PopupBaseViewModel
    {
        #region Fields
        private string m_Title;
        private string m_SubTitle;
        private string m_Placeholder;
        private string m_PromptText;
        #endregion

        #region Properties
        public string Title
        {
            get => m_Title;
            set => SetClassProperty(ref m_Title, value);
        }

        public string SubTitle
        {
            get => m_SubTitle;
            set => SetClassProperty(ref m_SubTitle, value);
        }

        public string Placeholder
        {
            get => m_Placeholder;
            set => SetClassProperty(ref m_Placeholder, value);
        }

        public string PromptText
        {
            get => m_PromptText;
            set => SetClassProperty(ref m_PromptText, value);
        }
        #endregion

        #region Commands
        public ICommand SubmitCommand { get; set; }
        #endregion

        #region Constructor
        public PromptViewModel(string i_Title, string i_Subtitle, string i_PlaceHolder, Action<string> i_AfterSubmit)
        {
            Title = i_Title ?? "No title";
            SubTitle = i_Subtitle ?? "No subtitle";
            Placeholder = i_PlaceHolder ?? "Description";
            SubmitCommand = new Command(async () =>
            {
                i_AfterSubmit.Invoke(PromptText);
                await PopupNavigation.Instance.PopAsync();
            });
        }
        #endregion

        #region Methods 

        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(PromptViewModel));
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
        }

        #endregion
    }
}
