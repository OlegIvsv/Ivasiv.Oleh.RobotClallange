using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ivasiv.Oleh.RobotClallange.Helpers
{
    public static class EnergyHelper
    {
        public static int EnergyToGetTo(Position newPosition, Position currentPosition)
        {
            int Min2D(int x1, int x2)
            {
                return (new int[3]
                            {
                    (int) Math.Pow( x1 - x2, 2.0),
                    (int) Math.Pow( x1 - x2 + 100, 2.0),
                    (int) Math.Pow( x1 - x2 - 100, 2.0)
                            }).Min();
            }

            return Min2D(currentPosition.X, newPosition.X) + Min2D(currentPosition.Y, newPosition.Y);
        }

        public static int CanGetToWith(Position newPosition, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            int energyLoss = EnergyToGetTo(myRobot.Position, newPosition);
            if (Intelligence.OccupiedByEnemie(robots, newPosition))
                energyLoss += Details.AttackEnergyLoss;

            return energyLoss;
        }


        public static EnergyStation MostBenneficialStation(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
           var recomendedStationPathCosts = Intelligence.StationsCanBeOccupied(map, robots, myRobot)
                    .GroupBy(s => CanGetToWith(s.Position, robots, myRobot))
                    .OrderBy(el => el.Key)
                    .ToArray();

            if (recomendedStationPathCosts.Length == 0)
                return null;

            EnergyStation res = recomendedStationPathCosts
                    .First()
                    .First();

            return res;
        }

        public static bool ChildCanSurvive(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            var myRobotCloneLikeChild = new Robot.Common.Robot
            {
                Position = map.FindFreeCell(myRobot.Position, robots),
            };
            var recomendedStationPathCosts = Intelligence.StationsCanBeOccupied(map, robots, myRobot)
                .Where(
                    s => DirectionHelper.FindStepNumber(
                            myRobotCloneLikeChild.Position, 
                            s.Position, 
                            myRobot.Energy - Details.EnergyLossToCreateNewRobot
                    ) <= Details.MaxStepsForChild
                ) 
                .GroupBy(s => CanGetToWith(myRobotCloneLikeChild.Position, robots, myRobotCloneLikeChild))
                .OrderBy(el => el.Key)
                .ToArray();

            return recomendedStationPathCosts.
                Any(g => g.Count() > 0);
        }
    }
}
