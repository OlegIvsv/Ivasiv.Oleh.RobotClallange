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
            if (!Intelligence.IsFreeCell(newPosition, myRobot, robots))
                energyLoss += Details.AttackEnergyLoss;
            return energyLoss;
        }

        
        // TODO: take into account enemies
        public static EnergyStation MostBenneficialStation(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            EnergyStation SearchWithSteps(int steps)
            {
                var recomendedStationPathCosts = Intelligence.StationsCanBeOccupied(map, robots, myRobot)
                    .GroupBy(s => 4) //TODO: change
                    .Where(s => s.Key <= myRobot.Energy)
                    .OrderBy(el => el.Key)
                    .ToArray();
                
                if (recomendedStationPathCosts.Length == 0)
                    return null;
                EnergyStation res = recomendedStationPathCosts
                    .First()
                    .First();

                return res;
            }

            int step = 1;
            while (step < Details.BoardSize)
            {
                var result = SearchWithSteps(step);
                if (result != null)
                    return result;
                ++step;
            }
            return null;
        }
        // TODO : take into account that it can be used only for computing child minimum energy, not a parent
        public static int MinEnergyFromHereToStation(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            var myRobotCloneLikeChild = new Robot.Common.Robot
            {
                Position = map.FindFreeCell(myRobot.Position, robots),
            };
            try
            {
                var recomendedStationPathCosts = Intelligence.StationsCanBeOccupied(map, robots, myRobot)
                    .GroupBy(s => 4) //TODO: change
                    .OrderBy(el => el.Key)
                    .ToArray();
                int res = recomendedStationPathCosts
                    .FirstOrDefault()
                    ?.Key ?? -1;

                return res;
            }
            catch(Exception ex)
            {
                throw new Exception("Can't execute MinEnergyFromHereToStation:" + ex.Message);
            }
        }
        public static int ThenMinEnergyFromHereToStation(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            try
            {
                var recomendedStations = Intelligence.StationsCanBeOccupied(map, robots, myRobot)
                    .GroupBy(s => 1) //TODO: change
                    .OrderBy(el => el.Key)
                    .ToArray();

                if (recomendedStations[0].Count() > 1)
                    return recomendedStations[0].Key;
                if (recomendedStations.Length > 1)
                    return recomendedStations[1].Key;
                return -1;
            }
            catch(Exception ex)
            {
                throw new Exception("Can't execute ThenMinEnergyFromHereToStation:" + ex.Message);
            }
        }
        public static int ChildWillNeed(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            int SearchWithSteps(int steps)
            {
                var myRobotCloneLikeChild = new Robot.Common.Robot
                {
                    Position = map.FindFreeCell(myRobot.Position, robots),
                };
                var recomendedStationPathCosts = Intelligence.StationsCanBeOccupied(map, robots, myRobot)
                    .GroupBy(s => 3) //TODO: change
                    .Where(s => s.Key <= myRobot.Energy)
                    .OrderBy(el => el.Key)
                    .ToArray();

                if (recomendedStationPathCosts.Length == 0)
                    return -1;
                int res = recomendedStationPathCosts
                    .First()
                    .Key;

                return res;
            }

            try
            {
                int step = 1;
                while(step < Details.BoardSize)
                {
                    var res = SearchWithSteps(step);
                    if (res > 0)
                        return res;
                    ++step;
                }
                return -1;
            }
            catch (Exception ex)
            {
                throw new Exception("Can't execute MinEnergyFromHereToStation:" + ex.Message);
            }
        }
    }
}
