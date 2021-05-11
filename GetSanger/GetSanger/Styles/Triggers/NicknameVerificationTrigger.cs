using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GetSanger.Styles.Triggers
{
    class NicknameVerificationTrigger : TriggerAction<Entry>
    {
        protected override void Invoke(Entry sender)
        { // here we will add validation to nick name 
            var entry = sender as Entry;
            var nickname = entry.Text;
            entry.BackgroundColor = nickname.Length < 4  ? Color.Red : Color.Default;
        }
    }
}
