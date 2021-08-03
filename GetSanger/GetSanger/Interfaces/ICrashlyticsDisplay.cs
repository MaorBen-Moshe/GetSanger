namespace GetSanger.Interfaces
{
    public interface ICrashlyticsDisplay : ICrashlytics
    {
        void LogPageEntrance(string i_PageName);
    }
}