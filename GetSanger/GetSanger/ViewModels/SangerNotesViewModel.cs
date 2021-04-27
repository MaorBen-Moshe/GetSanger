using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(CurrentJob), "job")]
    public class SangerNotesViewModel : BaseViewModel
    {
        #region Fields
        private string m_Notes;
        #endregion

        #region Properties
        public string Notes
        {
            get => m_Notes;
            set => SetClassProperty(ref m_Notes, value);
        }
        public JobOffer CurrentJob { get; set; }
        #endregion 
        #region Commands
        public ICommand SendNotesCommand { get; set; }
        #endregion
        #region Constructor
        public SangerNotesViewModel()
        {
            SendNotesCommand = new Command(sendNotes);
        }
        #endregion
        #region Methods
        public override void Appearing()
        {
        }

        private async void sendNotes()
        {
            if(string.IsNullOrEmpty(Notes) == false)
            {
                CurrentJob.SangerNotes = Notes;
                Activity activity = new Activity
                {
                    JobDetails = CurrentJob,
                    SangerID = AuthHelper.GetLoggedInUserId(),
                    ClientID = CurrentJob.ClientID,
                    Title = "No title", // need to change
                    Status = ActivityStatus.Pending,
                    LocationActivatedBySanger = false
                };

                AppManager.Instance.ConnectedUser.AppendCollections(AppManager.Instance.ConnectedUser.Activities, new ObservableCollection<Activity>(await FireStoreHelper.AddActivity(activity)));
                await r_PageService.DisplayAlert("Note", "Your request has been sent!", "Thanks");
                await GoBack();
            }
        }
        #endregion
    }
}
