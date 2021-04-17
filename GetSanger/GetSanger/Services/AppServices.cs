using System;
using System.Collections.Generic;

namespace GetSanger.Services
{
    public class AppServices : Dictionary<Type, Service>
    {
        #region Service Dictionary Methods

        public Service GetService(Type type)
        {
            Service service;
            if (TryGetValue(type, out service) == false)
            {
                throw new ArgumentException($"Service of type {type} does not exist in app services.");
            }
            
            return service;
        }

        #endregion

        #region Service Methods

        public void SetDependencies()
        {
            foreach (Service service in Values)
            {
                service.SetDependencies();
            }
        }

        #endregion

    }
}
