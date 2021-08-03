using System;

namespace GetSanger.Exceptions
{
    public class NoInternetException : Exception
    {
        public NoInternetException(string i_Message, Exception i_Inner = null)
            : base(i_Message, i_Inner)
        {
        }
    }
}
