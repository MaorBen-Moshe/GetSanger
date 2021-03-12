using GetSanger.Models;
using System.Windows.Input;

namespace GetSanger.ViewModels
{
    public class ActivityViewModel : BaseViewModel
    {
        public Activity ConnectedActivity { get; private set; }

        public string CurrLocation { get => ConnectedActivity.JobOffer.Location; }
        public string WorkLocation { get => ; }
        public string Phone { get => ; }
        public string Category { get =>; }
        public string Date { get =>; }
        public string Description { get => ; }


        public ICommand ProfileCommand { get; private set; }
        public ICommand LocationCommand { get; private set; }


    }
}
