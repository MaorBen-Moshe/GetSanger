﻿using GetSanger.Constants;
using GetSanger.Models;
using GetSanger.Services;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using GetSanger.Views.popups;

namespace GetSanger.ViewModels
{
    [QueryProperty(nameof(UserId), "userid")]
    public class ProfileViewModel : BaseViewModel
    {
        #region Fields
        private User m_CurrenUser;
        private Report m_CurrentReport;
        private int m_AverageRating;
        private ImageSource m_UserImage;
        private string m_Location;
        #endregion

        #region Properties
        public string UserId { get; set; }

        public User CurrentUser
        {
            get => m_CurrenUser;
            set => SetClassProperty(ref m_CurrenUser, value);
        }

        public int AverageRating
        {
            get => m_AverageRating;
            set => SetStructProperty(ref m_AverageRating, value);
        }

        public ImageSource UserImage
        {
            get => m_UserImage;
            set => SetClassProperty(ref m_UserImage, value);
        }

        public string UserLocation
        {
            get => m_Location;
            set => SetClassProperty(ref m_Location, value);
        }

        #endregion

        #region Commands

        public ICommand SendMessageCommand { get; set; }

        public ICommand ReportUserCommand { get; set; }

        public ICommand AddRatingCommand { get; set; }

        public ICommand ViewRatingsCommand { get; set; }

        #endregion

        #region Constructor
        public ProfileViewModel()
        {
            setCommands();
        }
        #endregion

        #region Methods

        public override void Appearing()
        {
            setUser();
        }

        public void Disappearing()
        {
            CurrentUser = null;
        }

        private void setCommands()
        { 
            AddRatingCommand = new Command(addRating);
            ReportUserCommand = new Command(reportUser);
            SendMessageCommand = new Command(sendMessageToUser);
            ViewRatingsCommand = new Command(viewRatings);
        }

        private async void setUser()
        {
            try
            {
                r_LoadingService.ShowPopup();
                if (string.IsNullOrEmpty(UserId))
                {
                    throw new ArgumentException("User details aren't available.");
                }
                CurrentUser = await FireStoreHelper.GetUser(UserId);
                if (CurrentUser == null)
                {
                    throw new ArgumentException("User details aren't available.");
                }

                List<Rating> ratings = new List<Rating>(CurrentUser.Ratings);
                UserImage = r_PhotoDisplay.DisplayPicture(CurrentUser.ProfilePictureUri);
                Placemark placemark = await r_LocationServices.PickedLocation(CurrentUser.UserLocation);
                UserLocation = $"{placemark.Locality}, {placemark.CountryName}";
                AverageRating = getAverage(ratings);
            }
            catch(Exception e)
            {
                await r_PageService.DisplayAlert("Error", e.Message, "OK");
            }
            finally
            {
                r_LoadingService.HidePopup();
            }
        }

        private int getAverage(List<Rating> i_Ratings)
        {
            int avg = 0;
            foreach(var rating in i_Ratings)
            {
                avg += rating.Score;
            }

            if(i_Ratings.Count > 0)
            {
                avg /= i_Ratings.Count;
            }
            
            return (avg > 0) ? avg : 1;
        }

        private async void sendMessageToUser(object i_Param)
        {
            string json = ObjectJsonSerializer.SerializeForPage(CurrentUser);
            await r_NavigationService.NavigateTo($"{ShellRoutes.ChatView}?user={json}");
        }

        private async void reportUser(object i_Param)
        {   
            string response = await r_PageService.DisplayActionSheet("Please choose the reason:", "Cancel", null, AppManager.Instance.GetListOfEnumNames(typeof(ReportOption)).ToArray());
            if(Enum.TryParse(response, out ReportOption option))
            {
                try
                {
                    m_CurrentReport = new Report
                    {
                        ReporterId = AppManager.Instance.ConnectedUser.UserId,
                        ReportedId = CurrentUser.UserId,
                        Reason = option
                    };

                    string answer = await r_PageService.DisplayPrompt("Write your details:", "please add info to contact with you", "text here..."); 
                    if(answer != null)
                    {
                        m_CurrentReport.ReportMessage = answer;
                        await RunTaskWhileLoading(FireStoreHelper.AddReport(m_CurrentReport));
                        await r_PageService.DisplayAlert("Note", "Your Report has been sent to admin.", "Thanks");
                    }
                }
                catch (Exception e)
                {
                    await r_PageService.DisplayAlert("Oh No", e.Message, "Sorry");
                }
            }
        }

        private async void addRating(object i_Param)
        {
            var ratingsPopup = new AddRatingPage(CurrentUser.UserId);
            (ratingsPopup.BindingContext as AddRatingViewModel).RatingAddedEvent += async () => AverageRating = getAverage(await FireStoreHelper.GetRatings(CurrentUser.UserId));
            await PopupNavigation.Instance.PushAsync(ratingsPopup);
        }

        private async void viewRatings(object i_Param)
        {
            await r_NavigationService.NavigateTo($"{ShellRoutes.Ratings}?id={CurrentUser.UserId}&isMyRatings={false}");
        }

        #endregion
    }
}
