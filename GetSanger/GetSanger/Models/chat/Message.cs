using GetSanger.Services;
using SQLite;
using System;
namespace GetSanger.Models.chat
{
    [Table("Messages")]
    public class Message : PropertySetter
    {
        private string m_Text;
        private string m_FromId; // who sent the message
        private string m_ToId; // who receive the message
        private DateTime m_TimeSent;

        [PrimaryKey, AutoIncrement]
        public int UniqueId { get; set; }

        [MaxLength(255)]
        public string Text
        {
            get => m_Text;
            set => SetClassProperty(ref m_Text, value);
        }

        public string FromId
        {
            get => m_FromId;
            set => SetClassProperty(ref m_FromId, value);
        }

        public string ToId
        {
            get => m_ToId;
            set => SetClassProperty(ref m_ToId, value);
        }

        public DateTime TimeSent
        {
            get => m_TimeSent;
            set => SetStructProperty(ref m_TimeSent, value);
        }
    }
}
