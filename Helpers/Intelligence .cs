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
        public static bool IsFreeStation(EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
            =>  IsFreeCell(station.Position, movingRobot, robots);
        public static bool IsFreeCell(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            foreach (var robot in robots)
                if (robot.Position == cell)
                    return false;
            return true;
        }
        public static EnergyStation TheRobotOnAStation(Map map, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            return map.Stations.FirstOrDefault(s => s.Position == myRobot.Position);
        }
        public static bool OccupiedByEnemie(List<Robot.Common.Robot> robots, Position pos)
        {
            return robots.Where(r => r.Position == pos)
                .Count() > 0;
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


        public static List<EnergyStation> OccupiedStations(Map map, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            return map.Stations.Where(s => !IsFreeStation(s, myRobot, robots))
                .ToList();
        }
        public static List<EnergyStation> StationsCanBeOccupied(Map map, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            var occupiedByFamily = OccupiedByFamilyStations(map, robots, myRobot);
            return map.Stations.Except(occupiedByFamily)
                .ToList();
        }
        public static List<EnergyStation> OccupiedByFamilyStations(Map map, List<Robot.Common.Robot> robots, Robot.Common.Robot myRobot)
        {
            var occupied = OccupiedStations(map, robots, myRobot);

            var familyPossitions = Family(robots, myRobot).Select(r => r.Position);

            return occupied.Where(s => familyPossitions.Contains(s.Position))
                .ToList(); 
        }
    }
}
