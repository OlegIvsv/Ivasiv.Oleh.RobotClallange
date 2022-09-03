using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivasiv.Oleh.RobotClallange.Helpers
{
    public static class DirectionHelper
    {
        public static Position NextPosition(Position start, Position end)
        {
            Position result = new Position();
            result.X = start.X == end.X ? start.X : (start.X < end.X ? start.X + 1 : start.X - 1);
            result.Y = start.Y == end.Y ? start.Y : (start.Y < end.Y ? start.Y + 1 : start.Y - 1);

            return result;
        }
    }
}
