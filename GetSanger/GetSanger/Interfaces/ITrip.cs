namespace GetSanger.Interfaces
{
    public interface ITrip
    {
        void StartTripThread(System.Timers.ElapsedEventHandler i_Elpased = null, int i_Interval = 30000);

        void LeaveTripThread(System.Timers.ElapsedEventHandler i_Elpased = null);
    }
}