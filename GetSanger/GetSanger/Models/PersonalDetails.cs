﻿using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public enum GenderType { Male, Female, Other };
    public class PersonalDetails : PropertySetter
    {
        #region Fields
        private string m_NickName;
        private GenderType m_Gender;
        private ContactPhone m_Phone;
        private DateTime m_Birthday;
        #endregion

        public string NickName
        {
            get => m_NickName;
            set => SetClassProperty(ref m_NickName, value);
        }
        public GenderType Gender
        {
            get => m_Gender;
            set => SetStructProperty(ref m_Gender, value);
        }
        public ContactPhone Phone
        {
            get => m_Phone;
            set => SetClassProperty(ref m_Phone, value);
        }
        public DateTime Birthday
        {
            get => m_Birthday;
            set => SetStructProperty(ref m_Birthday, value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PersonalDetails))
            {
                throw new ArgumentException("Must provide a valid PersonalDetails object");
            }

            PersonalDetails other = obj as PersonalDetails;
            return Birthday.Equals(other.Birthday) &&
                   Phone.PhoneNumber == other.Phone.PhoneNumber &&
                   Gender.Equals(other.Gender) &&
                   NickName == other.NickName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
