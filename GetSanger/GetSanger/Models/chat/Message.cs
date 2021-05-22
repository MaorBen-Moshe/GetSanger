﻿using GetSanger.Services;
using System;
using System.Windows.Input;

namespace GetSanger.Models.chat
{
    public class Message : PropertySetter
    {
        private string m_Text;
        private string m_FromId; // who sent the message
        private string m_ToId; // who receive the message
        private DateTime m_TimeSent;
        private bool m_MessageSent;

        #region ViewCellCommand
        // the only way that worked to bind the view cell(in a separate file) to the command
        public ICommand DeleteMessageCommand { get; set; }
        #endregion

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

        public bool MessageSent
        {
            get => m_MessageSent;
            set => SetStructProperty(ref m_MessageSent, value);
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Message))
            {
                throw new ArgumentException("obj must be of type Message!");
            }

            Message other = obj as Message;
            return Text.Equals(other.Text) && 
                   FromId.Equals(other.FromId) &&
                   ToId.Equals(other.ToId) &&
                   TimeSent.Equals(other.TimeSent);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}