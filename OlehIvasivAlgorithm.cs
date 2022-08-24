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
        protected EnergyStation myStation = null;
        public OlehIvasivAlgorithm()
        {
          
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var myRobot = robots[robotToMoveIndex];
            // Create a new robot if it's possible
            var createRobotCommand = CreateNewRobot(myRobot);
            if (createRobotCommand != null)
                return createRobotCommand;
            //Find station if it's needed
            var moveToStationCommand = LookForEnergy(map, myRobot, robots.ToList());
            if (moveToStationCommand != null)
                return moveToStationCommand;
        }

        private CreateNewRobotCommand CreateNewRobot(Robot.Common.Robot myRobot)
        {
            CreateNewRobotCommand command = null;
            if (Details.EnergyLossToCreateNewRobot < myRobot.Energy - Details.EnergyLeaveForParent)
                command = new CreateNewRobotCommand() 
                { 
                    NewRobotEnergy = myRobot.Energy - Details.EnergyLeaveForParent
                };
            return command;
        }

        private RobotCommand LookForEnergy(Map map, Robot.Common.Robot myRobot, List<Robot.Common.Robot> robots)
        {
            var mostBeneficialStation = EnergyHelper.MostBenneficialStation(map, myRobot, robots);
            int energyToGetThere = EnergyHelper.EnergyToGetTo(myRobot.Position, mostBeneficialStation.Position);

            RobotCommand command = null;
            if (myStation == null || myStation.Energy < Details.MaxEnergyCanCollect)
                command = new MoveCommand()
                {
                    NewPosition = mostBeneficialStation.Position
                };
            else
                command = new CollectEnergyCommand();

            return command;    
        }
    }
}
