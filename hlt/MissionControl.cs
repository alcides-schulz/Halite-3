using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Halite3.hlt
{
    public class MissionControl
    {
        public Game Game;
        public Dictionary<int, Mission> Missions = new Dictionary<int, Mission>();

        public MissionControl(Game game)
        {
            Game = game;
        }

        public bool ShipIsReturning(Ship ship)
        {
            if (!Missions.ContainsKey(ship.id.id)) return false;
            var mission = Missions[ship.id.id];
            return mission.IsReturning();
        }

        public void AddReturnMission(Ship ship)
        {
            if (Missions.ContainsKey(ship.id.id))
                Log.LogMessage("ERROR: MissionControl.AddReturnMission. ship already have mission: " + ship);
            else
                Missions.Add(ship.id.id, new Mission(Game, ship, Game.gameMap.GetClosestDropoff(Game.me, ship.position), true));
        }

        public void AddCollectMission(Ship ship, Position halite_position)
        {
            if (Missions.ContainsKey(ship.id.id))
                Log.LogMessage("ERROR: MissionControl.AddCollectMission. ship already have mission: " + ship);
            else
                Missions.Add(ship.id.id, new Mission(Game, ship, halite_position, false));
        }

        public void CancelMission(Ship ship)
        {
            if (Missions.ContainsKey(ship.id.id)) Missions.Remove(ship.id.id);
        }

        public bool ShipHasMission(Ship ship)
        {
            return Missions.ContainsKey(ship.id.id);
        }

        public bool ThereIsMissiontToTarget(Position target)
        {
            return Missions.Values.FirstOrDefault(m => m.Target.Equals(target)) != null;
        }
    }
}
