using System;
using System.Collections.Generic;
using System.Linq;

namespace GetSanger.Extensions
{
    public static class EnumTypeExtension
    {
        public static IList<string> GetListOfEnumNames(this Type i_EnumType)
        {
            return i_EnumType.GetEnumNames().ToList();
        }
    }
}