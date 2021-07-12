namespace GetSanger.Interfaces
{
    public interface ITrip
    {
        void StartTripThread(System.Timers.ElapsedEventHandler i_Elpased = null, int i_Interval = 300000);

        void LeaveTripThread(System.Timers.ElapsedEventHandler i_Elpased = null);
    }
}