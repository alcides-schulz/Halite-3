using System;
using System.Collections.Generic;
using System.Text;

namespace Halite3.hlt
{
    public class Mission
    {
        public Game Game;
        public int ShipId;
        public Position Target;
        public bool Returning;
        public int Distance;
        public List<Position> Path;

        public Mission(Game game, Ship ship, Position target, bool returning)
        {
            ShipId = ship.id.id;
            Target = target;
            Returning = returning;
            Game = game;
            Distance = Game.gameMap.CalculateDistance(ship.position, target);
        }

        public bool IsReturning()
        {
            return Returning;
        }

        public bool IsCollecting()
        {
            return !Returning;
        }

        public Position GetNextMove(Ship ship)
        {
            if (Path == null) return null;
            var index = Path.IndexOf(ship.position);
            if (index == -1) return null;
            if (index >= Path.Count - 1) return null;
            return Path[index + 1];
        }

        public override string ToString()
        {
            var str = "";

            str += "Ship: " + ShipId + " ";
            str += "Target: " + Target + " ";
            str += "Return: " + Returning + " ";
            str += "Distance: " + Distance;

            return str;
        }
    }
}
