using GetSanger.Models;

namespace GetSanger.Services
{
    public class ShellPassComplexDataService<T> : Service where T : class
    {
        private static object m_Obj;

        public static T ComplexObject
        {
            get
            {
                T toRet = default;
                bool same = m_Obj != null && m_Obj.GetType().Equals(typeof(T));
                if (same)
                {
                    toRet = (T)((T)m_Obj).CloneObject();
                }

                return toRet;
            }

            set
            {
                if (value != null)
                {
                    m_Obj = value;
                }
            }
        }

        public override void SetDependencies()
        {
        }
    }
}
