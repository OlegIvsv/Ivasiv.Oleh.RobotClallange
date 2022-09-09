using Robot.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ivasiv.Oleh.RobotClallange.Helpers
{
    public static class DirectionHelper
    {
        public static Position NextPosition(Robot.Common.Robot robot, Position destination)
        {
            /* Some required values */
            int xPureDistance = Math.Abs(robot.Position.X - destination.X);
            int xLinearDistance = GetXDistance(robot.Position, destination);
            int yPureDistance = Math.Abs(robot.Position.Y - destination.Y);
            int yLinearDistance = GetYDistance(robot.Position, destination);
            int stepNumber = FindStepNumber(robot.Position, destination, robot.Energy);

            Position newPosition = new Position();

            /* X calculating */
            if (xPureDistance <= xLinearDistance)
                newPosition.X = robot.Position.X
                    + (int)Math.Ceiling((double)(destination.X - robot.Position.X) / stepNumber);
            else 
            {
                newPosition.X = Math.Sign(robot.Position.X - destination.X) * xLinearDistance;
                newPosition.X = robot.Position.X + Math.Sign(newPosition.X) 
                    * (int)Math.Ceiling((double)Math.Abs(newPosition.X) / stepNumber); 
            }

            /* Y calculating */
            if (yPureDistance <= yLinearDistance)
                newPosition.Y = robot.Position.Y
                     + (int)Math.Ceiling((double)(destination.Y - robot.Position.Y) / stepNumber);
            else 
            {
                newPosition.Y = Math.Sign(robot.Position.Y - destination.Y) * yLinearDistance;
                newPosition.Y = robot.Position.Y + Math.Sign(newPosition.Y)
                    * (int)Math.Ceiling((double)Math.Abs(newPosition.Y) / stepNumber);
            }

            /* Correcting */
            newPosition = GetPositionCorrected(newPosition);

            return newPosition; 
        }
        public static int FindStepNumber(Position p1, Position p2, int energy)
        {
            if(energy <= 0)
                return int.MaxValue;
            int steps = 1;
            int xLinearDistance = GetXDistance(p1, p2);
            int yLinearDistance = GetYDistance(p1, p2);

            while (steps < energy)
            {
                Position start = new Position(0, 0);
                Position stepSpan = new Position(
                    (int)Math.Ceiling((double)xLinearDistance / steps),
                    (int)Math.Ceiling((double)yLinearDistance / steps)
                    );
                if (EnergyHelper.EnergyToGetTo(start, stepSpan) * steps <= energy)
                    return steps;
                ++steps;
            }
            return energy;
        }
       
        public static int GetXDistance(Position p1, Position p2)
        {
            return GetLinearDistance(p1.X, p2.X, Details.BoardSize);
        }
        public static int GetYDistance(Position p1, Position p2)
        {
            return GetLinearDistance(p1.Y, p2.Y, Details.BoardSize);
        }
        private static int GetLinearDistance(int x1, int x2, int max)
        {
            return Math.Min((x2 - x1 + max) % max, (x1 - x2 + max) % max);
        }
        public static int GetLinearLoss(Position p1, Position p2)
        {
            return (int)(Math.Pow(GetYDistance(p1, p2), 2) + Math.Pow(GetYDistance(p1, p2), 2));
        }


        public static Position GetPositionCorrected(Position p)
        {
            return GetPositionCorrected(p.X, p.Y);
        }
        public static Position GetPositionCorrected(int x, int y)
        {
            return new Position() { X = (x + Details.BoardSize) % Details.BoardSize, Y = (y + Details.BoardSize) % Details.BoardSize };
        }
    }
}
