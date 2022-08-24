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

        // personal
        public const double CreateOneMoreIfCoef = 2.0;
        public const int EnergyLeaveForParent = 100;

        // noticed
        public const double NewRobotDefaultEnergy = 100;
        public const int MaxRobotCount = 100;
    }
}
