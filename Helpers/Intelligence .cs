using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivasiv.Oleh.RobotClallange.Helpers
{
    public static class Intelligence
    {
        public static int FindDistance(Position a, Position b)
            => (int)(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));


        public static bool IsFreeStation(EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
            =>  IsFreeCell(station.Position, movingRobot, robots);
        public static bool IsFreeCell(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            foreach (var robot in robots)
                if (robot != movingRobot)
                    if (robot.Position == cell)
                        return false;
            return true;
        }

        public static Position FindNearestFreeStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            EnergyStation nearest = null;
            int minDistance = int.MaxValue;
            foreach (var station in map.Stations)
                if (IsFreeStation(station, movingRobot, robots))
                {
                    int d = FindDistance(station.Position, movingRobot.Position);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        nearest = station;
                    }
                }
            return nearest == null ? null : nearest.Position;
        }
        public static EnergyStation TheRobotOnAStation(Map map, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            return map.Stations.FirstOrDefault(s => s.Position == myRobot.Position);
        }


        public static List<Robot.Common.Robot> Enemies(List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            return robots.Where(r => r.OwnerName != myRobot.OwnerName)
                .ToList();
        }
        public static List<Robot.Common.Robot> Family(List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            return robots.Where(r => r.OwnerName == myRobot.OwnerName)
               .ToList();
        }
        public static bool IsFamily(Robot.Common.Robot robot, Robot.Common.Robot myRobot)
        {
            return robot.OwnerName == myRobot.OwnerName;
        }
        public static bool IsEnemy(Robot.Common.Robot robot, Robot.Common.Robot myRobot)
        {
            return !IsFamily(robot, myRobot);
        }


        public static List<EnergyStation> FreeStations(Map map, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            return map.Stations.Where(s => IsFreeStation(s, myRobot, robots)).ToList();
        }
        public static List<EnergyStation> OccupiedStations(Map map, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            return map.Stations.Where(s => !IsFreeStation(s, myRobot, robots)).ToList();
        }
        public static List<EnergyStation> StationsCanBeOccupied(Map map, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            var occupiedByFamily = OccupiedByFamilyStations(map, robots, myRobot);
            return map.Stations.Except(occupiedByFamily).ToList();
        }
        public static List<EnergyStation> OccupiedByFamilyStations(Map map, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            var occupied = OccupiedStations(map, robots, myRobot);

            var familyPossitions = Family(robots, myRobot).Select(r => r.Position);

            return occupied.Where(s => familyPossitions.Contains(s.Position))
                .ToList(); 
        }
        public static List<EnergyStation> OccupiedByEnemiesStations(Map map, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            var occupied = OccupiedStations(map, robots, myRobot);

            var enemiesPossitions = Enemies(robots, myRobot).Select(r => r.Position);

            return occupied.Where(s => enemiesPossitions.Contains(s.Position))
                .ToList();
        }
    }
}
