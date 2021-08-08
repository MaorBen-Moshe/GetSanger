using GetSanger.AppShell;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class OnBoardingViewModel : BaseViewModel
    {
        #region ModelClass
        public class OnBoardingItem
        {
            public string Text { get; set; }

            public bool IsLast { get; set; }
        }

        #endregion

        #region Fields
        private ObservableCollection<OnBoardingItem> m_Items;
        #endregion

        #region Properties
        public ObservableCollection<OnBoardingItem> Items
        {
            get => m_Items;
            set => SetClassProperty(ref m_Items, value);
        }
        #endregion

        #region Commands
        public ICommand StartCommand { get; set; }
        #endregion

        #region Constructor
        public OnBoardingViewModel()
        {
            SetCommands();
            initialItems();
        }
        #endregion

        #region Methods
        #endregion
        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(OnBoardingViewModel));
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
            StartCommand = new Command(() =>
            {                
                Application.Current.MainPage = new AuthShell();
            });
        }

        private void initialItems()
        {
            Items = new ObservableCollection<OnBoardingItem>
            {
                new OnBoardingItem
                {
                    Text = "Welcome to Get Sanger.",
                    IsLast = false
                },
                new OnBoardingItem
                {
                    Text = "Start your journey to help others and benefit from it.",
                    IsLast = false
                },
                new OnBoardingItem
                {
                    Text = string.Format(@"Decide what type of person you are:
Sanger
Client
Or you can be Both :)"),
                    IsLast = false
                },
                new OnBoardingItem
                {
                    Text = "Sign up and let's go.",
                    IsLast = true
                }
            };
        }
    }
}