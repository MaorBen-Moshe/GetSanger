using System;
using System.Reflection;

namespace GetSanger.Services
{
    public static class Singleton<T> where T : class
    {
        private static T s_Instance;
        private static readonly object s_LockObj;

        static Singleton()
        {
            s_LockObj = new object();
        }

        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (s_LockObj)
                    {
                        if (s_Instance == null)
                        {
                            Type typeT = typeof(T);
                            ConstructorInfo[] constructors = typeT.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                            bool hasCtor = false;

                            try
                            {
                                foreach(ConstructorInfo ctor in constructors)
                                {
                                    if(ctor.IsPrivate && ctor.GetParameters().Length == 0)
                                    {
                                        hasCtor = true;
                                        s_Instance = (T)ctor.Invoke(null);
                                        break;
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                throw new Exception(null, exception);
                            }

                            if (!hasCtor)
                            {
                                throw new Exception("No constructor was found!");
                            }
                        }
                    }
                }

                return s_Instance;
            }
        }
    }
}