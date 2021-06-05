using GetSanger.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Models
{
    public class ProviderCell : PropertySetter
    {
        private SocialProvider m_SocialProvider;
        private string m_ImageText;

        public SocialProvider SocialProvider
        {
            get => m_SocialProvider;
            set => SetStructProperty(ref m_SocialProvider, value);
        }

        public string ImageText
        {
            get => m_ImageText;
            set => SetClassProperty(ref m_ImageText, value);
        }
    }
}
