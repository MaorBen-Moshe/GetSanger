using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetSanger.Services
{
    public class PropertySetter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetClassProperty<T>(ref T i_Member, T i_Value, [CallerMemberName] string i_PropertyName = null) where T : class
        {
            setHelper(ref i_Member, i_Value, i_PropertyName);
        }

        public void SetStructProperty<T>(ref T i_Member, T i_Value, [CallerMemberName] string i_PropertyName = null) where T : struct
        {
            setHelper(ref i_Member, i_Value, i_PropertyName);
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

        private void setHelper<T>(ref T i_Member, T i_Value, string i_PropertyName)
        {
            if (i_Member == null || i_Member.Equals(i_Value) == false)
            {
                i_Member = i_Value;
                OnPropertyChanged(i_PropertyName);
            }
        }
    }
}