using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GetSanger.ChatDatabase
{
    public class AsyncLazy<T>
    {
        private readonly Lazy<Task<T>> r_Instance;

        public AsyncLazy(Func<T> i_Factory)
        {
            r_Instance = new Lazy<Task<T>>(() => Task.Run(i_Factory));
        }

        public AsyncLazy(Func<Task<T>> i_Factory)
        {
            r_Instance = new Lazy<Task<T>>(() => Task.Run(i_Factory));
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return r_Instance.Value.GetAwaiter();
        }
    }
}
