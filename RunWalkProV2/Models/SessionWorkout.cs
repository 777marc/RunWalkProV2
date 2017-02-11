using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunWalkProV2.Models
{
    public sealed class  SessionWorkout
    {
        // properties
        public static DateTime StartTime { get; set; }
        public static double BaseSeconds { get; set; }
        public static DateTime PausedTime { get; set; }
        public static double SecondsPaused { get; set; }
        public static DateTime IntervalStartTime { get; set; }
        public static double IntervalElapsedTime { get; set; }
        public static double CurentIntervalDuration { get; set; }
        public static double Distance { get; set; }
        public static bool IsRunning { get; set; }
        
        // instance
        private static SessionWorkout instance = null;
        
        public SessionWorkout()
        {
            PausedTime = DateTime.MinValue;
            SecondsPaused = 0;
            BaseSeconds = 0;
        }

        public static SessionWorkout Instance
        {
            get
            {
                if (Instance == null)
                {
                    instance = new SessionWorkout();
                }
                return instance;
            }
        }


    }
}
