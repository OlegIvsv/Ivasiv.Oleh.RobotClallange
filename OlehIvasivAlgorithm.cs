using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Ivasiv.Oleh.RobotClallange.Helpers;
using Robot.Common;
using Robot.Tournament;

namespace Ivasiv.Oleh.RobotClallange
{
    public class OlehIvasivAlgorithm : IRobotAlgorithm
    {
        public string Author => "Oleh Ivasiv";

        public OlehIvasivAlgorithm()
        {
            
        }


        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var myRobot = robots[robotToMoveIndex];

            // create a new robot if it's possible
            try
            {
                var createRobotCommand = IfCreateNewRobot(map, myRobot, robots.ToList());

                if (createRobotCommand != null)
                    return createRobotCommand;
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Cannot check if create new robot:" + ex.Message);
            }

            // find station if it's needed
            try
            {
                var moveToStationCommand = IfLookForEnergy(map, myRobot, robots.ToList());

                if (moveToStationCommand != null)
                    return moveToStationCommand;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Cannot check if look for energy:" + ex.Message);
            }

            throw new Exception();
        }

        //TODO: check if the cell is occupied not to attack
        //TODO: check if the station have no energy
        //TODO: fix not enough energy to collect resource
        protected CreateNewRobotCommand IfCreateNewRobot(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            if (Details.CurrentRound > Details.StopCreatingChildsAfter)
                return null;
            if (Intelligence.TheRobotOnAStation(map, robots,myRobot) == null)
                return null;
            if (Intelligence.Family(robots, myRobot).Count() >= Details.FamilySizeLimit)
                return null;
            if (myRobot.Energy < Details.EnergyLossToCreateNewRobot)
                return null;

            return IfChildCanBeCreated(map, myRobot, robots);
        }

        protected CreateNewRobotCommand IfChildCanBeCreated(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            try
            {
                bool childCanSurvive = EnergyHelper.ChildCanSurvive(map, myRobot, robots);
                int childNeed = myRobot.Energy - Details.EnergyLossToCreateNewRobot - 1;
                return CreateCommandForChildCreting(childCanSurvive, childNeed);
            }
            catch (Exception ex)
            {
                throw new ArithmeticException("Cannot canlculate required robot energy:" + ex.Message);
            }
        }

        protected CreateNewRobotCommand CreateCommandForChildCreting(bool iShouldCreate, int energyForChild)
        {
            if (iShouldCreate)
                return new CreateNewRobotCommand()
                {
                    NewRobotEnergy = energyForChild
                };
            return null;
        }


        //TODO: take into account station radius
        protected RobotCommand IfLookForEnergy(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            try
            {
                var mostBeneficialStation = EnergyHelper.MostBenneficialStation(map, myRobot, robots);
                var currentStation = Intelligence.TheRobotOnAStation(map, robots, myRobot);
                bool shouldChange = ShouldChange(currentStation, mostBeneficialStation);
                return CreateCommandForEnergyCollecting(shouldChange, mostBeneficialStation?.Position, myRobot);
            }
            catch (Exception ex)
            {
                throw new ArithmeticException("Cannot choose the station to move:" + ex.Message);
            }
        }
        //TODO: rewrite this
        protected bool ShouldChange(EnergyStation currentStation, EnergyStation newStation)
        {
            bool shouldChange = false;

            if(newStation == null)
                return false;
            if(currentStation == null)
                return true;
            if(Details.CurrentRound >= Details.StopChangingStations)
                return false;

            shouldChange = currentStation.Energy <= Details.MinEnergyGrowth
                && newStation.Energy >= 0.5 * Details.MaxStationEnergy;

            return shouldChange;
        }

        protected RobotCommand CreateCommandForEnergyCollecting(bool shouldChange, Position stationPos, Robot.Common.Robot robot)
        {
            if (shouldChange)
                return new MoveCommand()
                {
                    NewPosition = DirectionHelper.NextPosition(robot, stationPos)
                };
            return new CollectEnergyCommand();
        }
    }
}
