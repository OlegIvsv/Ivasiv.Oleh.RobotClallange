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

        public static Position FindNearestFreeStation(Robot.Common.Robot movingRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            EnergyStation nearest = null;
            int minDistance = int.MaxValue;
            foreach (var station in map.Stations)
                if (IsStationFree(station, movingRobot, robots))
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

        public static bool IsStationFree(EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
            =>  IsCellFree(station.Position, movingRobot, robots);

        public static bool IsCellFree(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            foreach (var robot in robots)
                if (robot != movingRobot)
                    if (robot.Position == cell)
                        return false;
            return true;
        }

        public static bool TheRobotOnTheStation(Robot.Common.Robot robot, EnergyStation station)
        {
            throw new NotImplementedException();
        }

        public static List<Robot.Common.Robot> Enemies(List<Robot.Common.Robot> robots)
        {
            throw new NotImplementedException();
        }

        public static List<Robot.Common.Robot> Family(List<Robot.Common.Robot> robots)
        {
            throw new NotImplementedException();
        }

        public static List<EnergyStation> FreeStations(Map map, List<Robot.Common.Robot> robots)
        {
            throw new NotImplementedException();
        }

        public static List<EnergyStation> OccupiedStations(Map map, List<Robot.Common.Robot> robots)
        {
            throw new NotImplementedException();
        }
    }
}
