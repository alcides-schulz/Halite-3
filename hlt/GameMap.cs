using System;
using System.Collections.Generic;
using System.Linq;

namespace Halite3.hlt
{
    /// <summary>
    /// Gameplay takes place on a wrapping rectangular grid 32x32, 40x40, 48x48, 56x56, or 64x64 in dimension.
    /// <para>
    /// This class is responsible for two key functions:
    /// keeping track of what's on the map at any given cell, and helping ships navigate.
    /// </para>
    /// </summary>
    /// <para><see cref="https://halite.io/learn-programming-challenge/api-docs#map"/></para>
    public class GameMap
    {
        public readonly int width;
        public readonly int height;
        public readonly MapCell[][] cells;
        public Dictionary<Position, List<Position>> CombatArea = new Dictionary<Position, List<Position>>();

        private List<MapCell> cell_list = new List<MapCell>();
 
        /// <summary>
        /// Creates a new instance of a GameMap
        /// </summary>
        /// <para><seealso cref="_generate"/></para>
        /// <param name="width">The width, as an int, of the map</param>
        /// <param name="height">The height, as an int, of the map</param>
        public GameMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            cells = new MapCell[height][];
            for (int y = 0; y < height; ++y)
            {
                cells[y] = new MapCell[width];
            }

        }

        private List<MapCell> MapCellsByValue()
        {
            return cell_list.OrderByDescending(mc => mc.halite).ToList();
        }

        public void InitCombatAreas()
        {
            foreach (var map_cell in cell_list) {
                InitCombatArea(map_cell.position);
            }
        }

        public void InitCombatArea(Position start)
        {
            var visited = new List<Position>();
            var queue = new Queue<Position>();

            visited.Add(start);
            queue.Enqueue(start);

            var position_list = new List<Position>();
            CombatArea.Add(start, position_list);

            while (queue.Count != 0) {
                var current = queue.Dequeue();

                int distance = CalculateDistance(start, current) + 1;
                if (distance > Constants.INSPIRATION_RADIUS) continue;

                position_list.Add(current);

                foreach (var direction in DirectionExtensions.ALL_CARDINALS) {
                    var new_position = GetTargetPosition(current, direction);
                    if (!visited.Contains(new_position)) {
                        queue.Enqueue(new_position);
                        visited.Add(new_position);
                    }
                }
            }
        }

        public bool IsInpiredPosition(Player player, Position position)
        {
            var positions = CombatArea[position];
            var enemies = positions.Select(p => At(p).ship != null && At(p).ship.owner.id != player.id.id).ToList();
            return enemies.Count >= 2;
        }

        public int GetTotalHalite()
        {
            return cell_list.Sum(mc => mc.halite);
        }

        public int TotalHaliteAround(Position position)
        {
            int total = At(position).halite;
            foreach (Direction direction in DirectionExtensions.ALL_CARDINALS) {
                total += At(GetTargetPosition(position, direction)).halite;
            }
            return total;
        }

        public List<Position> GetHighValuePositions(Shipyard shipyard, int distance_shipyard, int max_positions)
        {
            var high_value_cells = MapCellsByValue();
            var count = 0;
            var list = new List<Position>();
            
            foreach (var mc in high_value_cells) {
                if (CalculateDistance(mc.position, shipyard.position) > distance_shipyard) continue;
                list.Add(mc.position);
                if (++count >= max_positions) break;
            }

            return list;
        }

        public int CalculateHaliteForDropoff(Position start, int max_distance)
        {
            var visited = new List<Position>();
            var queue = new Queue<Position>();
            var total = 0;

            visited.Add(start);
            queue.Enqueue(start);

            while (queue.Count() != 0) {
                var p = queue.Dequeue();
                if (At(p).dropoff_coverage == false) total += At(p).halite;

                int distance = CalculateDistance(start, p) + 1;
                if (distance > max_distance) continue;

                foreach (var d in DirectionExtensions.ALL_CARDINALS) {
                    if (d == Direction.STILL) continue;
                    var new_position = GetTargetPosition(p, d);
                    if (!visited.Contains(new_position)) {
                        queue.Enqueue(new_position);
                        visited.Add(new_position);
                    }
                }
            }

            return total;
        }

        public void UpdateDropoffCoverage(Position start, int max_distance)
        {
            var visited = new List<Position>();
            var queue = new Queue<Position>();

            visited.Add(start);
            queue.Enqueue(start);

            while (queue.Count() != 0) {
                var p = queue.Dequeue();

                At(p).dropoff_coverage = true;

                int distance = CalculateDistance(start, p) + 1;
                if (distance > max_distance) continue;

                foreach (var d in DirectionExtensions.ALL_CARDINALS) {
                    var new_position = GetTargetPosition(p, d);
                    if (!visited.Contains(new_position)) {
                        queue.Enqueue(new_position);
                        visited.Add(new_position);
                    }
                }
            }
        }

        public Position GetClosestDropoff(Player player, Position from)
        {
            var closest_distance = CalculateDistance(from, player.shipyard.position);
            var closest_dropoff = player.shipyard.position;

            foreach (var dropoff in player.dropoffs.Values) {
                var distance = CalculateDistance(from, dropoff.position);
                if (distance < closest_distance) {
                    closest_distance = distance;
                    closest_dropoff = dropoff.position;
                }
            }
            return closest_dropoff;
        }

        public Direction HighHaliteDirection(Position from, Game game)
        {
            double current = 0;
            Direction direction = Direction.STILL;

            foreach (var d in DirectionExtensions.ALL_CARDINALS) {
                var new_position = GetTargetPosition(from, d);
                if (At(new_position).IsOccupied(game.me.id.id)) continue;
                if (DangerousPosition(from, new_position)) {
                    Log.LogMessage("Dangerous: " + from + " -> " + new_position);
                    continue;
                }
                var total = AreaTotalHalite(new_position, d, game);
                if (total > current) {
                    current = total;
                    direction = d;
                }
            }

            return direction;
        }

        public bool DangerousPosition(Position from, Position to)
        {
            var my_ship = At(from).ship;
            if (my_ship == null) return false;
            if (At(to).ship != null && At(to).ship.owner.id != my_ship.owner.id) {
                if (EnemyCanPossiblyStay(to)) return true;
                if (PossibleHaliteLoss(my_ship, At(to).ship)) return true;
            }
            // verify if enemies can move to the "to" position.
            foreach (var d in DirectionExtensions.ALL_CARDINALS) {
                var enemy_from_position = GetTargetPosition(to, d);
                var enemy_ship = At(enemy_from_position).ship;
                if (enemy_ship == null) continue;
                if (my_ship.owner == enemy_ship.owner) continue;
                if (EnemyCanPossiblyStay(enemy_from_position)) continue;
                if (At(to).halite - At(enemy_from_position).halite > 500) return true;
                if (PossibleHaliteLoss(my_ship, enemy_ship)) return true;
            }
            return false;
        }

        public bool EnemyCanPossiblyStay(Position position)
        {
            var enemy_ship = At(position).ship;
            if (At(position).halite / Constants.MOVE_COST_RATIO > enemy_ship.halite) {
                Log.LogMessage("DangerPosition: -> " + position + " enemy cannot move");
                return true;
            }
            if (At(position).halite > 50 && At(position).halite / Constants.EXTRACT_RATIO <= Constants.MAX_HALITE - enemy_ship.halite) {
                Log.LogMessage("DangerPosition: -> " + position + " enemy may collect");
                return true;
            }
            return false;
        }

        public bool PossibleHaliteLoss(Ship ship, Ship enemy)
        {
            if (ship.halite - enemy.halite > 100) {
                Log.LogMessage("DangerPosition: " + ship.position + "->" + enemy.position + " possible halite loss");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Normalizes the given Position and then returns the corresponding MapCell.
        /// </summary>
        public MapCell At(Position position)
        {
            Position normalized = Normalize(position);
            return cells[normalized.y][normalized.x];
        }

        public int AreaTotalHalite(Position position, Direction direction, Game game)
        {
            int total = 0;
            int lenght = 3 + game.turnNumber / 50;

            for (int i = 0; i < lenght; i++) {

                total += At(position).halite;

                var position_right = GetTargetPosition(position, DirectionExtensions.TurnRight(direction));
                var position_left = GetTargetPosition(position, DirectionExtensions.TurnLeft(direction));

                total += At(position_right).halite;
                total += At(position_left).halite;

                position = GetTargetPosition(position, direction);
            }

            return total;
        }

        /// <summary>
        /// Normalizes the position of an Entity and returns the corresponding MapCell.
        /// </summary>
        public MapCell At(Entity entity)
        {
            return At(entity.position);
        }

        /// <summary>
        /// A method that computes the Manhattan distance between two locations, and accounts for the toroidal wraparound.
        /// </summary>
        public int CalculateDistance(Position source, Position target)
        {
            Position normalizedSource = Normalize(source);
            Position normalizedTarget = Normalize(target);

            int dx = Math.Abs(normalizedSource.x - normalizedTarget.x);
            int dy = Math.Abs(normalizedSource.y - normalizedTarget.y);

            int toroidal_dx = Math.Min(dx, width - dx);
            int toroidal_dy = Math.Min(dy, height - dy);

            return toroidal_dx + toroidal_dy;
        }

        /// <summary>
        /// A method that normalizes a position within the bounds of the toroidal map.
        /// </summary>
        /// <remarks>
        /// Useful for handling the wraparound modulus arithmetic on x and y.
        /// For example, if a ship at (x = 31, y = 4) moves to the east on a 32x32 map,
        /// the normalized position would be (x = 0, y = 4), rather than the off-the-map position of (x = 32, y = 4).
        /// </remarks>
        public Position Normalize(Position position)
        {
            int x = ((position.x % width) + width) % width;
            int y = ((position.y % height) + height) % height;
            return new Position(x, y);
        }

        public Direction GetDirection(Position from, Position to)
        {
            foreach (var direction in DirectionExtensions.ALL_CARDINALS) {
                if (GetTargetPosition(from, direction).Equals(to)) return direction;
            }
            return Direction.STILL;
        }

        public Position GetTargetPosition(Position position, Direction direction)
        {
            return Normalize(position.DirectionalOffset(direction));
        }

        /// <summary>
        /// A method that returns a list of direction(s) to move closer to a target disregarding collision possibilities.
        /// Returns an empty list if the source and destination are the same.
        /// </summary>
        public List<Direction> GetUnsafeMoves(Position source, Position destination)
        {
            List<Direction> possibleMoves = new List<Direction>();

            Position normalizedSource = Normalize(source);
            Position normalizedDestination = Normalize(destination);

            int dx = Math.Abs(normalizedSource.x - normalizedDestination.x);
            int dy = Math.Abs(normalizedSource.y - normalizedDestination.y);
            int wrapped_dx = width - dx;
            int wrapped_dy = height - dy;

            if (normalizedSource.x < normalizedDestination.x)
            {
                possibleMoves.Add(dx > wrapped_dx ? Direction.WEST : Direction.EAST);
            }
            else if (normalizedSource.x > normalizedDestination.x)
            {
                possibleMoves.Add(dx < wrapped_dx ? Direction.WEST : Direction.EAST);
            }

            if (normalizedSource.y < normalizedDestination.y)
            {
                possibleMoves.Add(dy > wrapped_dy ? Direction.NORTH : Direction.SOUTH);
            }
            else if (normalizedSource.y > normalizedDestination.y)
            {
                possibleMoves.Add(dy < wrapped_dy ? Direction.NORTH : Direction.SOUTH);
            }

            return possibleMoves;
        }

        public bool HasUnmovableShip(int playerid, Position position)
        {
            if (At(position).ship == null) return false;
            if (At(position).ship.halite < At(position).halite / Constants.MOVE_COST_RATIO) return true;
            return false;
        }

        public bool ShipWillCollect(int playerid, Position position)
        {
            if (At(position).ship == null) return false;
            if (At(position).halite > Zluhcsh3.MIN_HALITE_MOVE) return true;
            return false;
        }

        public Direction GetClosestDirection(Ship ship, Position destination)
        {
            var closest_distance = CalculateDistance(ship.position, destination);
            var closest_direction = Direction.STILL;

            foreach (var direction in DirectionExtensions.ALL_CARDINALS) {
                var target = GetTargetPosition(ship.position, direction);
                if (At(target).ship != null && At(target).ship.owner.id != ship.owner.id) continue;
                var distance = CalculateDistance(target, destination);
                if (distance < closest_distance) {
                    closest_distance = distance;
                    closest_direction = direction;
                }
            }

            return closest_direction;
        }


        /// <summary>
        /// A method that returns a direction to move closer to a target without colliding with other entities.
        /// Returns a direction of “still” if no such move exists.
        /// </summary>
        public Direction NaiveNavigate(Ship ship, Position destination)
        {
            // getUnsafeMoves normalizes for us
            foreach (Direction direction in GetUnsafeMoves(ship.position, destination))
            {
                Position targetPos = ship.position.DirectionalOffset(direction);
                if (!At(targetPos).IsOccupied(ship.owner.id))
                {
                    return direction;
                }
            }

            return Direction.STILL;
        }

        /// <summary>
        /// Clears all the ships in preparation for player._update() and updates the halite on each cell.
        /// </summary>
        public void _update()
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    cells[y][x].ship = null;
                }
            }

            int updateCount = Input.ReadInput().GetInt();

            for (int i = 0; i < updateCount; ++i)
            {
                Input input = Input.ReadInput();
                int x = input.GetInt();
                int y = input.GetInt();

                cells[y][x].halite = input.GetInt();
            }
        }

        public Boolean CanMove(Ship ship)
        {
            if (ship.halite >= At(ship.position).halite / Constants.MOVE_COST_RATIO) return true;
            return false;
        }

        /// <summary>
        /// Reads the starting map for the game from the Halite engine.
        /// </summary>
        /// <returns></returns>
        public static GameMap _generate()
        {
            Input mapInput = Input.ReadInput();
            int width = mapInput.GetInt();
            int height = mapInput.GetInt();

            GameMap map = new GameMap(width, height);

            map.cell_list.Clear();

            for (int y = 0; y < height; ++y)
            {
                Input rowInput = Input.ReadInput();

                for (int x = 0; x < width; ++x)
                {
                    int halite = rowInput.GetInt();
                    map.cells[y][x] = new MapCell(new Position(x, y), halite);
                    map.cell_list.Add(map.cells[y][x]);
                }
            }

            return map;
        }
    }
}
