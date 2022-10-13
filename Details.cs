using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivasiv.Oleh.RobotClallange
{
    public static class Details
    {
        // variant
        public const int MaxStationEnergy = 5000;
        public const int MaxEnergyGrowth = 100;
        public const int MinEnergyGrowth = 50;
        public const int CollectingDistance = 3;
        public const int MaxEnergyCanCollect = 200;
        public const int EnergyLossToCreateNewRobot = 50;
        public const int AttackEnergyLoss = 10;
        public const double StoleRateEnergyAtAttack = 0.0;


        // general
        public const double NewRobotDefaultEnergy = 100;
        public const int MaxRobotCount = 100;
        public const int NumOfRounds = 50;
        public const int BoardSize = 100;


        //personal
        public const int StopCreatingChildsAfter = 40;
        public const int FamilySizeLimit = 100;
        public const int StopChangingStations = 45;
        public const int MaxStepsForChild = 4;
        public const int LeaveForParent = 30; 
        public const int LeaveForChildJustInCase = 10;



        // Round counting
        public static int CurrentRound { get; private set; }
        static Details()
        {
            Logger.OnLogRound += Logger_OnLogRound;
        }
        private static void Logger_OnLogRound(object sender, LogRoundEventArgs e)
        {
            CurrentRound = e.Number;
        }
    }
}
