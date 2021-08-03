using GetSanger.Services;
using SQLite;
using System;

namespace GetSanger.Models.chat
{
    public class ChatUser : PropertySetter
    {
        #region Fields
        private string m_UserId;
        private string m_UserCreatedById;
        private DateTime m_LastMessage;
        private User m_User;
        private int m_Id = 0;
        #endregion

        #region Properties

        [PrimaryKey, AutoIncrement]
        public int DbId
        {
            get => m_Id;
            set => SetStructProperty(ref m_Id, value);
        }

        public string UserId
        {
            get => m_UserId;
            set => SetClassProperty(ref m_UserId, value);
        }

        public string UserCreatedById
        {
            get => m_UserCreatedById;
            set => SetClassProperty(ref m_UserCreatedById, value);
        }

        [Ignore]
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

        public override bool Equals(object obj)
        {
            return obj is ChatUser user &&
                   UserId == user.UserId &&
                   UserCreatedById == user.UserCreatedById &&
                   LastMessage == user.LastMessage;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, UserCreatedById, LastMessage);
        }

        #endregion
    }
}