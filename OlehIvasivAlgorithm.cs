using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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



        protected CreateNewRobotCommand IfCreateNewRobot(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            try
            {
                int energyINeedIfICreate = EnergyToSuriveMyself(map, myRobot, robots);
                int energyChildNeed = CanCreateChildWithEnergy(map, myRobot, robots);

                bool iShouldCreate = Intelligence.TheRobotOnAStation(map, robots, myRobot) == null
               ? myRobot.Energy > energyINeedIfICreate + energyChildNeed
               : myRobot.Energy >= energyChildNeed;

                return CreateCommandForChildCreting(iShouldCreate, energyChildNeed);
            }
            catch(Exception ex)
            {
                throw new ArithmeticException("Cannot canlculate required robot energy:" + ex.Message);
            }
        }

        protected int CanCreateChildWithEnergy(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            return Details.EnergyLossToCreateNewRobot + EnergyHelper.MinEnergyFromHereToStation(map, myRobot, robots);
        }

        protected int EnergyToSuriveMyself(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            return EnergyHelper.ThenMinEnergyFromHereToStation(map, myRobot, robots);
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



        protected RobotCommand IfLookForEnergy(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            try
            {
                var mostBeneficialStation = EnergyHelper.MostBenneficialStation(map, myRobot, robots);
                var currentStation = Intelligence.TheRobotOnAStation(map, robots, myRobot);
                bool shouldChange = ShouldChange(currentStation, mostBeneficialStation);
                return CreateCommandForEnergyCollecting(shouldChange, mostBeneficialStation, myRobot);
            }
            catch (Exception ex)
            {
                throw new ArithmeticException("Cannot choose the station to move:" + ex.Message);
            }
        }

        protected bool ShouldChange(EnergyStation currentStation, EnergyStation mostBeneficialStation)
        {
            bool shouldChange = false;
            if (currentStation == null)
                shouldChange = true;
            else
                shouldChange = currentStation.Energy <= Details.MaxEnergyGrowth
                    && mostBeneficialStation.Energy > Details.MaxEnergyGrowth;

            return shouldChange;
        }

        protected RobotCommand CreateCommandForEnergyCollecting(bool shouldChange, EnergyStation mostBeneficialStation, Robot.Common.Robot myRobot)
        {
            if (shouldChange)
                return new MoveCommand()
                {
                    NewPosition = DirectionHelper.NextPosition(myRobot.Position, mostBeneficialStation.Position)
                };
            return new CollectEnergyCommand();
        }
    }
}
