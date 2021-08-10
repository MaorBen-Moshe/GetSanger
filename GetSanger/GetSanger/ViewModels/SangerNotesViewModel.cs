using GetSanger.Extensions;
using GetSanger.Models;
using GetSanger.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace GetSanger.ViewModels
{
    public class SangerNotesViewModel : BaseViewModel
    {
        #region Fields
        string m_Notes;
        #endregion

        #region Properties
        public string Notes
        {
            get => m_Notes;
            set => SetClassProperty(ref m_Notes, value);
        }

        public JobOffer CurrentConfirmedJobOffer { get; set; }
        #endregion

        #region Commands
        public ICommand SendNotesCommand { get; set; }
        #endregion

        #region Constructor
        public SangerNotesViewModel(JobOffer job)
        {
            CurrentConfirmedJobOffer = job;
        }
        #endregion

        #region Methods
        #endregion
        public override void Appearing()
        {
        }

        public override void Disappearing()
        { 
        }

        protected override void SetCommands()
        {
            SendNotesCommand = new Command(sendNotes);
        }

        private async void sendNotes()
        {
            try
            {
                Activity activity = new Activity
                {
                    JobDetails = CurrentConfirmedJobOffer,
                    SangerNotes = Notes,
                    SangerID = AuthHelper.GetLoggedInUserId(),
                    SangerName = AppManager.Instance.ConnectedUser.PersonalDetails.NickName,
                    ClientID = CurrentConfirmedJobOffer.ClientID,
                    Status = eActivityStatus.Pending,
                    LocationActivatedBySanger = false
                };

                AppManager.Instance.ConnectedUser.Activities.Append<ObservableCollection<Activity>, Activity>(new ObservableCollection<Activity>(await RunTaskWhileLoading(FireStoreHelper.AddActivity(activity))));
                await sr_PageService.DisplayAlert("Note", "Your request has been sent!");
                MessagingCenter.Instance.Send(this, Constants.Constants.SangerNotesSent);
                await PopupNavigation.Instance.PopAsync();
            }
            catch (Exception e)
            {
                await e.LogAndDisplayError($"{nameof(SangerNotesViewModel)}:sendNotes", "Error", e.Message);
            }
        }
    }
}