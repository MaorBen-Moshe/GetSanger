using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.ViewModels
{
    public class AboutUsViewModel : PopupBaseViewModel
    {
        #region Fields
        private string m_AboutDescription;
        #endregion 

        #region Properties
        public string AboutDescription
        {
            get => m_AboutDescription;
            set => SetClassProperty(ref m_AboutDescription, value);
        }
        #endregion

        #region Commands
        #endregion

        #region Constructor
        public AboutUsViewModel()
        {
            setAboutDescription();
        }
        #endregion

        #region Methods
        public override void Appearing()
        {
            sr_CrashlyticsService.LogPageEntrance(nameof(AboutUsViewModel));
        }

        public override void Disappearing()
        {
        }

        protected override void SetCommands()
        {
        }

        private void setAboutDescription()
        {
            AboutDescription = string.Format(@"What is Get Sanger?
                        
Get Sanger is a mobile application that helps connect between people in need of help with daily tasks and people willing to lend a hand for a few bucks. 
This platform enables the “helpers” to make a side income through helping others without requiring any knowledge or qualification while enabling the “help receiver” the peace of mind that he is getting good quality help. 

How Does It Work?
                        
Need to get a task done? 
Get a Sanger.
Get Sanger offers a large variety of task categories that people may need help with in their daily lives. When you choose a category you publish your task, Sangers who are available for the chosen category will receive this request and contact you in order to coordinate. Now you choose your Sanger! You will be able to see Sanger ranking from previous users and speak to the Sanger in order to make sure he is capable of answering your needs.

Need to make some extra cash?
Become a Sanger.
By becoming a Sanger you can use your naturally gained knowledge and skills to help others get tasks done and get paid for it. All you have to do is sign up to the Sanger mode on the application, choose the categories you are interested to help with and wait for someone in need to reach out. Remember - as a Sanger you are always ranked according to the quality of your work and your interpersonal skills, the user can choose from a variety of Sangers who he feels will do the best work - let the best Sanger win!");

        }
        #endregion
    }
}