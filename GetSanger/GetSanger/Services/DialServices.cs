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
                if(!Regex.Match(value, @"^(\+[0-9])$").Success)
                {
                    throw new ArgumentException("Phone number should contain only numbers");
                }

                m_Phone = value;
            }
        }

        public string Message { get; set; } = string.Empty;

        public void Call()
        {
            if(PhoneNumber == null)
            {
                throw new ArgumentNullException("No phone number was given!");
            }

            PhoneDialer.Open(PhoneNumber);
        }

        public async void SendDefMsg()
        {
            await Xamarin.Essentials.Sms.ComposeAsync(new SmsMessage(Message, PhoneNumber));
        }

        public async Task<bool> SendWhatsapp()
        {
            string uri = $"https://wa.me/{"972" + PhoneNumber}?text={Message}";
            bool canOpen = await Launcher.CanOpenAsync(uri);
            if (canOpen) 
            {
                await Launcher.OpenAsync(new Uri(uri)); 
            }

            return canOpen;
        }

        public override void SetDependencies()
        {
           //
        }
    }
}
