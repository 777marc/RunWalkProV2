using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunWalkProV2.Models
{
    public sealed class UserSettings
    {
        public static int CaloriesPerUnit { get; set; }
        public static double WalkInterval { get; set; }
        public static double RunInterval { get; set; }
        public static string UnitOfMeasure { get; set; }
        public static string StartWith { get; set; }
        public static string AlertType { get; set; }
        public static bool LocationConsent { get; set; }
        public static string RunWalkPROUserName { get; set; }
        public static string RunWalkPROToken { get; set; }
        public static double TimeGoal { get; set; }
        public static double DistanceGoal { get; set; }
        public static double PaceGoal { get; set; }

        private static UserSettings instance = null;

        public UserSettings()
        {

        }

        public static UserSettings Instance
        {
            get
            {
                if (Instance == null)
                {
                    instance = new UserSettings();
                }
                return instance;
            }
        }

    }
}
