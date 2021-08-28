namespace GetSanger.ViewModels
{
    public class EditorViewModel : PopupBaseViewModel
    {
        #region Fields
        private string m_Title;
        private string m_Description;
        private string m_Placeholder;
        #endregion

        #region Properties
        public string Title
        {
            get => m_Title;
            set => SetClassProperty(ref m_Title, value);
        }

        public string Placeholder
        {
            get => m_Placeholder;
            set => SetClassProperty(ref m_Placeholder, value);
        }

        public string Description
        {
            get => m_Description;
            set => SetClassProperty(ref m_Description, value);
        }
        #endregion

        #region Commands
        #endregion

        #region constructor
        public EditorViewModel(string i_Title, string i_Description, string i_PlaceHolder)
        {
            Description = i_Description;
            Placeholder = i_PlaceHolder ?? "details here...";
            Title = i_Title ?? "Details:";
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(EditorViewModel));
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