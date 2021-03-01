using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GetSanger.ViewModels
{
    public class PropertySetter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetClassProperty<T>(ref T i_Member, T i_Value, [CallerMemberName] string i_PropertyName = null)
            where T : class

        {
            if (i_Member == null || i_Member.Equals(i_Value) == false)
            {
                i_Member = i_Value;
                OnPropertyChanged(i_PropertyName);
            }
        }

        public void SetStructProperty<T>(ref T i_Member, T i_Value, [CallerMemberName] string i_PropertyName = null)
            where T : struct
        {
            if (i_Member.Equals(i_Value) == false)
            {
                i_Member = i_Value;
                OnPropertyChanged(i_PropertyName);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string i_PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(i_PropertyName));
        }

        protected T GetProperty<T>(ref T i_Member, [CallerMemberName] string i_PropertyName = null)
        {
            try
            {
                return i_Member;
            }
            catch (NullReferenceException)
            {
                i_Member = default;
                return i_Member;
            }
            catch
            {
                throw;
            }
        }
    }
}
