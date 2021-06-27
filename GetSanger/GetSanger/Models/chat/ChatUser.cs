using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Models.chat
{
    public class ChatUser : PropertySetter
    {
        #region Fields
        private User m_User;
        private DateTime m_LastMessage;
        #endregion

        #region Properties
        public User User
        {
            get => m_User;
            set => SetClassProperty(ref m_User, value);
        }

        public DateTime LastMessage
        {
            get => m_LastMessage;
            set => SetStructProperty(ref m_LastMessage, value);
        }

        #endregion
    }
}
