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
        private Map CurrentMap { get; set; }
        private List<Robot.Common.Robot> CurrentRobots { get; set; }



        private static int IdCounter = 1;
        public static int Id { get; private set; }
        public static Dictionary<int, Position> Aims { get; private set; }
        public OlehIvasivAlgorithm()
        {
            Id = IdCounter;
            ++IdCounter;
        }
        static OlehIvasivAlgorithm()
        {
            Aims = new Dictionary<int, Position>();
        }



        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var myRobot = robots[robotToMoveIndex];
            CurrentMap = map;
            CurrentRobots = robots.ToList();

            // create a new robot if it's possible
            var createRobotCommand = IfCreateNewRobot(myRobot);

            if (createRobotCommand != null)
                return createRobotCommand;


            // find station if it's needed
            var moveToStationCommand = IfLookForEnergy(myRobot);

            if (moveToStationCommand != null)
                return moveToStationCommand;
        

            throw new Exception();
        }




        protected CreateNewRobotCommand IfCreateNewRobot(Robot.Common.Robot myRobot)
        {
            if (Details.CurrentRound > Details.StopCreatingChildsAfter)
                return null;
            if (Intelligence.TheRobotOnAStation(CurrentMap, CurrentRobots, myRobot) == null)
                return null;
            if (Intelligence.Family(CurrentRobots, myRobot).Count() >= Details.FamilySizeLimit)
                return null;
            if (myRobot.Energy <= Details.EnergyLossToCreateNewRobot + Details.LeaveForParent)
                return null;

            return IfChildCanBeCreated(myRobot);
        }

        protected CreateNewRobotCommand IfChildCanBeCreated(Robot.Common.Robot myRobot)
        {
            bool childCanSurvive = EnergyHelper.ChildCanSurvive(CurrentMap, myRobot, CurrentRobots);
            int childNeed = myRobot.Energy 
                - Details.EnergyLossToCreateNewRobot 
                - Details.LeaveForParent
                - 1;
            return CreateCommandForChildCreting(childCanSurvive, childNeed);
        }

        protected CreateNewRobotCommand CreateCommandForChildCreting(bool iShouldCreate, int energyForChild)
        {
            if (iShouldCreate)
                return new CreateNewRobotCommand() { NewRobotEnergy = energyForChild };
            return null;
        }




        protected RobotCommand IfLookForEnergy(Robot.Common.Robot myRobot)
        {
            var mostBeneficialStation = EnergyHelper.MostBenneficialStation(CurrentMap, myRobot, CurrentRobots);
            var currentStation = Intelligence.TheRobotOnAStation(CurrentMap, CurrentRobots, myRobot);
            bool shouldChange = ShouldChange(currentStation, mostBeneficialStation);
            if (myRobot.Energy <= 1)
                return new CollectEnergyCommand();
            return CreateCommandForEnergyCollecting(shouldChange, mostBeneficialStation?.Position, myRobot);
        }

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
                && newStation.Energy >= 0.2 * Details.MaxStationEnergy;

            return shouldChange;
        }

        protected RobotCommand CreateCommandForEnergyCollecting(bool shouldChange, Position stationPos, Robot.Common.Robot robot)
        {
            if (shouldChange)
            {
                var newPosition = DirectionHelper.NextPosition(robot, stationPos);
                return new MoveCommand()
                {
                    NewPosition = DirectionHelper.NextPosition(robot, stationPos)
                };
            }
            return new CollectEnergyCommand();
        }
    }
}
