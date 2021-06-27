using GetSanger.Services;
using SQLite;
using System;

namespace GetSanger.Models.chat
{
    public class ChatUser : PropertySetter
    {
        #region Fields
        private string m_UserId;
        private DateTime m_LastMessage;
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

        public DateTime LastMessage
        {
            get => m_LastMessage;
            set => SetStructProperty(ref m_LastMessage, value);
        }

        #endregion
    }
}
