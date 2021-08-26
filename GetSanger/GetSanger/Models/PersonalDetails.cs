using GetSanger.Services;
using System;

namespace GetSanger.Models
{
    public enum GenderType { Male, Female, Other };
    public class PersonalDetails : PropertySetter
    {
        #region Fields
        private string m_NickName;
        private GenderType m_Gender;
        private string m_Phone;
        private string m_About;
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
        public string Phone
        {
            get => m_Phone;
            set => SetClassProperty(ref m_Phone, value);
        }
        public DateTime Birthday
        {
            get => m_Birthday;
            set => SetStructProperty(ref m_Birthday, value);
        }

        public string About
        {
            get => m_About;
            set => SetClassProperty(ref m_About, value);
        }


        public static bool IsValidName(string name)
        {
            return string.IsNullOrWhiteSpace(name) == false && name.Length >= 4 && name.Length <= 20;
        }

        public override bool Equals(object obj)
        {
            return obj is PersonalDetails details &&
                   NickName == details.NickName &&
                   Gender == details.Gender &&
                   Phone == details.Phone &&
                   Birthday == details.Birthday &&
                   About == details.About;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NickName, Gender, Phone, Birthday, About);
        }
    }
}