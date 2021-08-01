﻿using System.Threading.Tasks;

namespace GetSanger.Interfaces
{
    public interface ITrip
    {
        void StartTripThread(System.Timers.ElapsedEventHandler i_Elpased, int i_Interval = 15000);

        void LeaveTripThread(System.Timers.ElapsedEventHandler i_Elpased);
    }
}