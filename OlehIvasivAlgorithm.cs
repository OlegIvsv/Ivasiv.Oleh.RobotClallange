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



        private CreateNewRobotCommand IfCreateNewRobot(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            int energyINeedIfICreate = 0, remainingEnergy = 0; 

            try
            {
                energyINeedIfICreate = EnergyToSuriveMyself(map, myRobot, robots);
                remainingEnergy = myRobot.Energy - CanCreateChildWithEnergy(map, myRobot, robots);
            }
            catch(Exception ex)
            {
                throw new ArithmeticException("Cannot canlculate required robot energy:" + ex.Message);
            }

            bool iShouldCreate = Intelligence.TheRobotOnAStation(map, robots, myRobot) != null
                ? remainingEnergy > 0
                : remainingEnergy >= energyINeedIfICreate;

            //throw new Exception($"energyINeedIfICreate= {energyINeedIfICreate}|" +
                //$"remainingEnergy={remainingEnergy}|iShouldCreate={iShouldCreate}");

            if (iShouldCreate)
                return new CreateNewRobotCommand()
                {
                    NewRobotEnergy = myRobot.Energy - remainingEnergy
                };
            return null;
        }

        private int CanCreateChildWithEnergy(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            return Details.EnergyLossToCreateNewRobot + EnergyHelper.MinEnergyFromHereToStation(map, myRobot, robots);
        }

        private int EnergyToSuriveMyself(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            return EnergyHelper.ThenMinEnergyFromHereToStation(map, myRobot, robots);
        }



        private RobotCommand IfLookForEnergy(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            EnergyStation mostBeneficialStation, currentStation;

            try
            {
                mostBeneficialStation = EnergyHelper.MostBenneficialStation(map, myRobot, robots);
                currentStation = Intelligence.TheRobotOnAStation(map, robots, myRobot);
            }
            catch (Exception ex)
            {
                throw new ArithmeticException("Cannot choose the station to move:" + ex.Message);
            }

            bool shouldChange = false;
            if (currentStation == null)
                shouldChange = true;
            else
                shouldChange = currentStation.Energy <= Details.MaxEnergyGrowth 
                    && mostBeneficialStation.Energy > Details.MaxEnergyGrowth;

            if (shouldChange)
                return new MoveCommand()
                {
                    NewPosition = mostBeneficialStation.Position
                };
            return new CollectEnergyCommand(); 
        }
    }
}
