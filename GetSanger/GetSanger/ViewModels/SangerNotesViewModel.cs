using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(CurrentJobJson), "job")]
    public class SangerNotesViewModel : BaseViewModel
    {
        #region Fields
        private string m_Notes;
        private JobOffer m_JobOffer;
        #endregion

        #region Properties
        public string Notes
        {
            get => m_Notes;
            set => SetClassProperty(ref m_Notes, value);
        }
        public JobOffer CurrentJob
        {
            get => m_JobOffer;
            set => SetClassProperty(ref m_JobOffer, value);
        }
        public string CurrentJobJson
        {
            set
            {
                CurrentJob = ObjectJsonSerializer.DeserializeForPage<JobOffer>(value);
            }
        }
        #endregion 
        #region Commands
        public ICommand SendNotesCommand { get; set; }
        #endregion
        #region Constructor
        public SangerNotesViewModel()
        {
            setCommands();
        }
        #endregion
        #region Methods
        public override void Appearing()
        {
        }

        private void setCommands()
        {
            SendNotesCommand = new Command(sendNotes);
        }

        private async void sendNotes()
        {
            CurrentJob.SangerNotes = Notes;
            Activity activity = new Activity
            {
                JobDetails = CurrentJob,
                SangerID = AuthHelper.GetLoggedInUserId(),
                ClientID = CurrentJob.ClientID,
                Status = ActivityStatus.Pending,
                LocationActivatedBySanger = false
            };

            AppManager.Instance.ConnectedUser.Activities.Append<ObservableCollection<Activity>, Activity>(new ObservableCollection<Activity>(await RunTaskWhileLoading(FireStoreHelper.AddActivity(activity))));
            await r_PageService.DisplayAlert("Note", "Your request has been sent!", "Thanks");
            await GoBack();
        }
        #endregion
    }
}
