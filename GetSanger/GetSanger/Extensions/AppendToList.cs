using System;
using System.Collections.Generic;
using System.Text;

namespace GetSanger.Extensions
{
    public static class AppendToList
    {
        public static void Append<T, K>(this T i_FirstCollection, T i_SecondCollection) where T : IList<K>
        {
            foreach (K item in i_SecondCollection)
            {
                i_FirstCollection.Add(item);
            }
        }
    }
}
