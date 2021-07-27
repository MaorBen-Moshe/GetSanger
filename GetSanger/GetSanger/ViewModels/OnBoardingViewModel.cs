using GetSanger.Views;
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
                if(Application.Current.Properties.ContainsKey(Constants.Constants.StartProperty) == false)
                {
                    Application.Current.Properties.Add(Constants.Constants.StartProperty, "");
                }
                
                Application.Current.MainPage = new SplashPage();
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
                    Text = "Decide what type of person you are:\n" +
                    " Sanger\n" +
                    " Client",
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
