using GetSanger.Models;

namespace GetSanger.Services
{
    public class ShellPassComplexDataService<T> : Service where T : class
    {
        public static T ComplexObject { get; set; }

        public override void SetDependencies()
        {
        }
    }
}
