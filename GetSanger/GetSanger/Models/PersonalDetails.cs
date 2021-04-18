using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GetSanger.Models
{
    public enum GenderType { Male, Female, Other };
    public class PersonalDetails
    {
        public string Nickname { get; set; }
        public GenderType Gender { get; set; }
        public ContactPhone Phone { get; set; }
        public DateTime Birthday { get; set; }

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
                   Nickname == other.Nickname;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
