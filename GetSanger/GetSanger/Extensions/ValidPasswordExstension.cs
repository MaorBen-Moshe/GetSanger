using System.Linq;

namespace GetSanger.Extensions
{
    public static class ValidPasswordExstension
    {
        public static bool IsValidPassword(this string i_Password)
        {
            return i_Password.Length >= 6 &&
                   i_Password.Length <= 12 &&
                   i_Password.Any(c => IsCapital(c)) &&
                   i_Password.Any(c => IsLower(c)) &&
                   i_Password.Any(c => IsDigit(c)) &&
                   i_Password.Any(c => IsSymbol(c));
        }

        private static bool IsCapital(char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        private static bool IsLower(char c)
        {
            return c >= 'a' && c <= 'z';
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsSymbol(char c)
        {
            // by Ascii table
            return c > 32 && c < 127 && !IsDigit(c) && !IsCapital(c) && !IsLower(c);
        }
    }
}