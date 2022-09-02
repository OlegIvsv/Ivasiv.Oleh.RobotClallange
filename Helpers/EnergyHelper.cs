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
            int energyLoss = EnergyToGetTo(newPosition, myRobot.Position);
            if (!Intelligence.IsFreeCell(newPosition, myRobot, robots))
                energyLoss += Details.AttackEnergyLoss;
            return energyLoss;
        }


        public static int EnergyOnSquare(Position upperLeftPoint, Position lowerRightPoint, Map map)
        {
            Predicate<EnergyStation> isIn = s => s.Position.X >= upperLeftPoint.X
                && s.Position.Y >= upperLeftPoint.Y
                && s.Position.X <= lowerRightPoint.X
                && s.Position.Y <= lowerRightPoint.Y;
            return map.Stations.Where(s => isIn(s))
                .Select(s => s.Energy)
                .Sum();
        }


        public static EnergyStation MostBenneficialStation(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            var canBeOccupied = Intelligence.StationsCanBeOccupied(map, robots, myRobot);
            canBeOccupied = canBeOccupied
                .Except(canBeOccupied.Where(s => CanGetToWith(s.Position, robots, myRobot) > myRobot.Energy)
                            .ToArray())
                .OrderBy(s => CanGetToWith(s.Position, robots, myRobot))
                .ToList();
                           

            return canBeOccupied.First();
        }


        public static int MinEnergyFromHereToStation(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            try
            {
                int recomendedStationPathCosts = Intelligence.StationsCanBeOccupied(map, robots, myRobot)
                    .GroupBy(s => CanGetToWith(s.Position, robots, myRobot))
                    .OrderBy(el => el.Key)
                    .FirstOrDefault().Key;

                return recomendedStationPathCosts;
            }
            catch(Exception ex)
            {
                throw new Exception("Can't execute MinEnergyFromHereToStation.");
            }
        }

        public static int ThenMinEnergyFromHereToStation(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            try
            {
                var recomendedStations = Intelligence.StationsCanBeOccupied(map, robots, myRobot)
                    .GroupBy(s => CanGetToWith(s.Position, robots, myRobot))
                    .OrderBy(el => el.Key)
                    .ToArray();

                if (recomendedStations[0].Count() > 1)
                    return recomendedStations[0].Key;
                return recomendedStations[1].Key;
            }
            catch(Exception ex)
            {
                throw new Exception("Can't execute ThenMinEnergyFromHereToStation.");
            }
        }
    }
}
