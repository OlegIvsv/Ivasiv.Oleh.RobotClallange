﻿using Robot.Common;
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
        public const int MaxEnergyGrowth = 40;
        public const int MinEnergyGrowth = 20;
        public const int EnergyStationForAttendant = 5;
        public const int CollectingDistance = 1;
        public const int MaxEnergyCanCollect = 200;
        public const int EnergyLossToCreateNewRobot = 100;
        public const int AttackEnergyLoss = 10;
        public const double StoleRateEnergyAtAttack = 0.0;



        // general
        public const double NewRobotDefaultEnergy = 100;
        public const int MaxRobotCount = 100;
        public const int NumOfRounds = 50;


        //personal
        public const int StopCreatingChildsAfter = 40;


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
