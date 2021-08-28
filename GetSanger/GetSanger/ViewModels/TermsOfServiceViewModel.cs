namespace GetSanger.ViewModels
{
    public class TermsOfServiceViewModel : PopupBaseViewModel
    {
        #region Fields
        private string m_Text;
        #endregion

        #region Properties
        public string Text
        {
            get => m_Text;
            set => SetClassProperty(ref m_Text, value);
        }
        #endregion

        #region Commands
        #endregion

        #region Constructor
        public TermsOfServiceViewModel()
        {
            Text = "Empty!";
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(TermsOfServiceViewModel));
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