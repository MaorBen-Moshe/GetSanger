﻿using GetSanger.Exceptions;
using GetSanger.Interfaces;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GetSanger.Services
{
    public class DialServices : Service, IDialService
    {
        private string m_Phone;

        public string PhoneNumber
        {
            get
            {
                if(m_Phone == null)
                {
                    throw new ArgumentNullException("No phone number was given");
                }

                return m_Phone;
            }
            set
            {
                if(!IsValidPhone(value))
                {
                    throw new ArgumentException("Phone number should contain only numbers");
                }

                m_Phone = value;
            }
        }

        public string Message { get; set; } = string.Empty;

        public void Call()
        {
            PhoneDialer.Open(PhoneNumber);
        }

        public Task SendDefAppMsg()
        {
            return Sms.ComposeAsync(new SmsMessage(Message, PhoneNumber));
        }

        public async Task<bool> SendWhatsapp()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string uri = $"https://wa.me/{"972" + PhoneNumber}?text={Message}";
                bool canOpen = await Launcher.CanOpenAsync(uri);
                if (canOpen) 
                {
                    await Launcher.OpenAsync(new Uri(uri)); 
                }

                return canOpen;
            }
            else
            {
                throw new NoInternetException("No Internet");
            }
            
        }

        public bool IsValidPhone(string i_Phone)
        {
            string validateString = i_Phone.Replace("-", "");
            Regex pattern = new Regex(@"(?<!\d)\d{10}(?!\d)");
            bool match = pattern.IsMatch(validateString) && validateString.Length == 10;
            return match;
        }

        public override void SetDependencies()
        {
        }
    }
}