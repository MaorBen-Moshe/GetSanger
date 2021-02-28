using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Xamarin.Forms.Internals;

namespace GetSanger.ViewModels
{
    [Preserve(AllMembers = true)]
    [DataContract]
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Event handler
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Methods

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T i_BackingField, T i_Value, [CallerMemberName] string i_PropertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(i_BackingField, i_Value))
            {
                return;
            }

            i_BackingField = i_Value;
            NotifyPropertyChanged(i_PropertyName);
        }

        #endregion
    }
}
