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
    }
}
