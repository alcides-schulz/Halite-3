using Halite3.hlt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Halite3
{
    public class Zluhcsh3
    {
        public static int MAP_SIZE;
        public static int HIGH_HALITE_AREA_MAX;
        public static int HIGH_HALITE_AREA_DISTANCE;
        public static int SHIP_RETURN_THRESHOLD;
        public static int MAX_SHIPS_PER_MAP;
        public static int MIN_HALITE_MOVE;

        private static Game game = new Game();
        private static List<Command> commandQueue = new List<Command>();

        private static Combat search = new Combat(game);
        private static PathFinder path_finder = new PathFinder(game);
        private static MissionControl mission_control;
        private static Dictionary<int, Direction> detour_next_move = new Dictionary<int, Direction>();
        private static bool IsEndGamePhase = false;

        public class HighAreaDistance
        {
            public Position ShipPosition;
            public Position AreaPosition;
            public double Value;

            public HighAreaDistance(Ship ship, Position area, double value)
            {
                ShipPosition = ship.position;
                AreaPosition = area;
                Value = value;
            }
        }

        private static bool HasBlockingShip(int playerId, Position position)
        {
            if (game.gameMap.At(position).ship == null) return false;
            var ship = game.gameMap.At(position).ship;
            if (ship.owner.id != playerId) return false;
            if (ship.moved) return true;
            if (game.gameMap.HasUnmovableShip(playerId, position)) return true;
            if (game.gameMap.ShipWillCollect(playerId, position)) return true;
            return false;
        }

        private static void ClearEnemies(Position dropoff_position)
        {
            Log.LogMessage("ClearEnemies: Initial Position " + dropoff_position);
            var position_list = new List<Position>();
            position_list.Add(dropoff_position);
            foreach(var direction in DirectionExtensions.ALL_CARDINALS) {
                position_list.Add(game.gameMap.GetTargetPosition(dropoff_position, direction));
            }
            foreach (var position in position_list) {
                if (game.gameMap.At(position).ship == null)continue;
                if (game.gameMap.At(position).ship.owner.id != game.me.id.id) {
                    game.gameMap.At(position).ship = null;
                    Log.LogMessage("ClearEnemies: ClearedEnemy at " + position);
                }
            }
        }

        public static void Main(string[] args)
        {
            MAP_SIZE = Math.Max(game.gameMap.width, game.gameMap.height);
            SHIP_RETURN_THRESHOLD = 950;
            HIGH_HALITE_AREA_MAX = MAP_SIZE / 4;
            HIGH_HALITE_AREA_DISTANCE = 10;
            MIN_HALITE_MOVE = 50;

            game.Ready("zluhcsh3");

            Log.enabled = false;

            mission_control = new MissionControl(game);
            
            var combat = new Combat(game);

            while (true) {
                game.UpdateFrame();
                Log.StartTimeTrack();

                Player me = game.me;
                GameMap gameMap = game.gameMap;

                if (game.turnNumber == 1) gameMap.InitCombatAreas();

                commandQueue.Clear();

                Log.StartSection("Clear enemy ships around shipyard/dropoffs");
                ClearEnemies(me.shipyard.position);
                foreach (var dropoff in me.dropoffs.Values) {
                    ClearEnemies(dropoff.position);
                }
                Log.EndSection();

                Log.StartSection("Update or Delete completed missions");
                var delete = new List<int>();
                foreach (var mission in mission_control.Missions.Values) {
                    if (me.ships.ContainsKey(mission.ShipId)) {
                        var ship = me.ships[mission.ShipId];
                        if (ship.position.Equals(mission.Target)) {
                            delete.Add(mission.ShipId);
                        }
                        mission.Distance = gameMap.CalculateDistance(ship.position, mission.Target);
                    }
                    else {
                        // ship does not exist anymore
                        Log.LogMessage("remove mission: " + mission.ShipId);
                        delete.Add(mission.ShipId);
                    }
                }
                foreach (var shipid in delete) {
                    mission_control.Missions.Remove(shipid);
                }
                Log.EndSection();

                Log.StartSection("Send ships back before game ends.");
                if (game.turnNumber + MAP_SIZE >= Constants.MAX_TURNS) {
                    IsEndGamePhase = true;
                    Log.LogMessage("ENDGAME: turn#: " + game.turnNumber + " MAX_TURNS: " + Constants.MAX_TURNS + " MAP_SIZE: " + MAP_SIZE);
                    foreach (var ship in me.ships.Values) {
                        if (mission_control.ShipIsReturning(ship)) continue;
                        var distance_shipyard = gameMap.CalculateDistance(gameMap.GetClosestDropoff(me, ship.position), ship.position);
                        var remaining_turns = Constants.MAX_TURNS - game.turnNumber;
                        if (distance_shipyard < remaining_turns && distance_shipyard + 10 > remaining_turns) {
                            Log.LogMessage("ENDGAME: return ship: " + ship + " DistYard: " + distance_shipyard + " Remaining Turns: " + remaining_turns);
                            mission_control.CancelMission(ship);
                            mission_control.AddReturnMission(ship);
                        }
                    }
                }
                Log.EndSection();

                Log.StartSection("Combat");
                combat.EvaluateCombatAreas();
                foreach (var ship in me.ships.Values) {
                    if (ship.make_combat_best_move == false) continue;
                    MakeMove("Combat_Move", ship, ship.best_move, true);
                }
                Log.EndSection();

                Log.StartSection("Prepare new missions");
                var dropoff_candidates = new List<int>();
                int dropoff_distance = MAP_SIZE / 4;

                var available_for_collection = new List<Ship>();

                foreach (var ship in me.ships.Values) {
                    if (ship.moved) continue;
                    if (gameMap.CalculateDistance(gameMap.GetClosestDropoff(me, ship.position), ship.position) >= dropoff_distance) dropoff_candidates.Add(ship.id.id);
                    if (mission_control.ShipIsReturning(ship)) continue;
                    if (ship.halite >= SHIP_RETURN_THRESHOLD) {
                        mission_control.CancelMission(ship);
                        mission_control.AddReturnMission(ship);
                    }
                    else {
                        // Ships available for halite collection, always re-assing collection mission.
                        mission_control.CancelMission(ship);
                        available_for_collection.Add(ship);
                    }
                }
                Log.EndSection();

                Log.StartSection("Create Dropoffs");
                // Create Dropoff
                int dropoff_ships = me.ships.Count / 8;
                if (MAP_SIZE >= 56) dropoff_ships = me.ships.Count / 10;
                int dropoff_max = 1;
                if (MAP_SIZE == 48) dropoff_max = 2;
                if (MAP_SIZE == 56) dropoff_max = 4;
                if (MAP_SIZE == 64) dropoff_max = 5;
                if (game.players.Count == 4) {
                    dropoff_max--;
                    if (MAP_SIZE == 64) dropoff_max--;
                }
                if (me.halite >= (Constants.DROPOFF_COST + Constants.SHIP_COST) && me.dropoffs.Count < dropoff_max && dropoff_candidates.Count >= dropoff_ships) {
                    var largest_halite = 0;
                    Ship dropoff_ship = null;
                    foreach (var id in dropoff_candidates) {
                        var ship = me.ships[id];
                        if (gameMap.CalculateDistance(ship.position, gameMap.GetClosestDropoff(me, ship.position)) < dropoff_distance + 2) {
                            continue;
                        }
                        var halite = gameMap.CalculateHaliteForDropoff(ship.position, MAP_SIZE / 4);
                        if (halite > largest_halite) {
                            largest_halite = halite;
                            dropoff_ship = ship;
                        }
                    }
                    var create_dropoff = true;
                    if (MAP_SIZE == 32 && game.players.Count == 2 && largest_halite < 30000) create_dropoff = false;
                    if (MAP_SIZE == 40 && game.players.Count == 2 && largest_halite < 32000) create_dropoff = false;
                    if (MAP_SIZE == 48 && game.players.Count == 2 && largest_halite < 34000) create_dropoff = false;
                    if (dropoff_ship != null && create_dropoff) {
                        commandQueue.Add(dropoff_ship.MakeDropoff());
                        gameMap.UpdateDropoffCoverage(dropoff_ship.position, MAP_SIZE / 4);
                        me.ships.Remove(dropoff_ship.id.id);
                        mission_control.CancelMission(dropoff_ship);
                        if (available_for_collection.Contains(dropoff_ship)) available_for_collection.Remove(dropoff_ship);
                    }
                }
                Log.EndSection();

                Log.StartSection("Assign halite collection to ships");
                var high_value_positions = game.gameMap.GetHighValuePositions(me.shipyard, 10 + game.turnNumber / 10, available_for_collection.Count * 20);

                // Send availble ships to closest halite areas
                var ship_area_distance_list = new List<HighAreaDistance>();

                foreach (var high_area_position in high_value_positions) {

                    foreach (var ship in available_for_collection) {
                        if (ship.moved) continue;
                        if (IsEndGamePhase && gameMap.At(ship.position).HasStructure()) {
                            if (gameMap.At(ship.position).structure.owner.id == me.id.id) continue;
                        }
                        int ship_halite_distance = gameMap.CalculateDistance(ship.position, high_area_position);
                        int halite_dropoff_distance = gameMap.CalculateDistance(high_area_position, gameMap.GetClosestDropoff(me, high_area_position));

                        int halite_to_collect = Constants.MAX_HALITE - ship.halite;
                        int available_halite = gameMap.At(high_area_position).halite;
                        int collected_halite = 0;
                        int turns_to_collect = 0;
                        var is_inspired = gameMap.IsInpiredPosition(me, high_area_position);

                        while (halite_to_collect > 0 && collected_halite < halite_to_collect && available_halite > 0) {
                            int collected_this_turn = (int)(available_halite / Constants.EXTRACT_RATIO);
                            if (collected_this_turn <= 0) break;
                            available_halite -= collected_this_turn;
                            if (is_inspired) collected_this_turn *= 3;
                            collected_halite += collected_this_turn;
                            turns_to_collect += 1;
                        }

                        double value = (double)collected_halite / (double)(turns_to_collect + ship_halite_distance + halite_dropoff_distance);
                        if (game.players.Count == 4 && is_inspired) value *= 3;

                        ship_area_distance_list.Add(new HighAreaDistance(ship, high_area_position, value));
                    }
                }

                var closest_area_list = ship_area_distance_list.OrderByDescending(sad => sad.Value).ToList<HighAreaDistance>();

                var created_collect_mission_count = 0;
                foreach (var closest_distance in closest_area_list) {
                    var high_area_position = closest_distance.AreaPosition;
                    if (mission_control.ThereIsMissiontToTarget(high_area_position)) continue;
                    var closest_ship = gameMap.At(closest_distance.ShipPosition).ship;
                    if (mission_control.Missions.ContainsKey(closest_ship.id.id)) continue;
                    mission_control.AddCollectMission(closest_ship, high_area_position);
                    created_collect_mission_count++;
                }

                Log.EndSection();

                Log.StartSection("Move execution");
                var priority_missions = mission_control.Missions.Values.OrderBy(m => m.Distance);

                foreach (var mission in priority_missions) {
                    if (!me.ships.ContainsKey(mission.ShipId)) {
                        continue;
                    }

                    var ship = me.ships[mission.ShipId];
                    var target = mission.Target;

                    if (ship.moved) continue;

                    if (!gameMap.CanMove(ship)) {
                        commandQueue.Add(ship.StayStill());
                        continue;
                    }

                    if (mission.IsReturning()) {
                        path_finder.SearchPath(ship.owner.id, ship.position, target, 1);
                        var path = path_finder.GetPath(target);
                        if (!IsEndGamePhase) {
                            if (path.Count() < 2 || gameMap.At(path[1]).IsOccupied(me.id.id)) {
                                commandQueue.Add(ship.StayStill());
                                continue;
                            }
                        }
                        if (path.Count > 1) {
                            var return_direction = gameMap.GetDirection(ship.position, path[1]);
                            MakeMove("return_ship", ship, return_direction, false);
                        }
                        else {
                            commandQueue.Add(ship.StayStill());
                        }
                        continue;
                    }
                    else {
                        if (gameMap.At(ship.position).halite > Zluhcsh3.MIN_HALITE_MOVE) {
                            commandQueue.Add(ship.StayStill());
                            continue;
                        }
                        if (detour_next_move.ContainsKey(ship.id.id)) {
                            var continuation_direction = detour_next_move[ship.id.id];
                            MakeMove("cont_detour", ship, continuation_direction, false);
                            detour_next_move.Remove(ship.id.id);
                            continue;
                        }
                        var direction = gameMap.GetClosestDirection(ship, target);
                        if (direction != Direction.STILL) {
                            var position_to_move = gameMap.GetTargetPosition(ship.position, direction);
                            if (HasBlockingShip(ship.owner.id, position_to_move) || BlockReturningShips(ship, direction)) {
                                var detour_direction = SelectDetour(ship, direction, target);
                                MakeMove("detour", ship, detour_direction, false);
                                continue;
                            }
                            MakeMove("collect", ship, direction, false);
                            continue;
                        }
                        commandQueue.Add(ship.StayStill());
                        continue;
                    }
                }
                Log.EndSection();

                Log.StartSection("Resolve pending moves");
                foreach (var ship in me.ships.Values) {
                    if (ship.pending == Direction.STILL || ship.moved) continue;

                    var target = gameMap.GetTargetPosition(ship.position, ship.pending);

                    var other_ship = gameMap.At(target).ship;
                    if (other_ship == null) {
                        commandQueue.Add(ship.Move(ship.pending));
                        gameMap.At(ship.position).ship = null;
                        gameMap.At(target).MarkUnsafe(ship);
                        continue;
                    }

                    if (other_ship.moved || other_ship.pending == Direction.STILL) {
                        commandQueue.Add(ship.StayStill());
                        continue;
                    }

                    var other_target = gameMap.GetTargetPosition(other_ship.position, other_ship.pending);
                    if (other_target.Equals(ship.position)) {
                        commandQueue.Add(ship.Move(ship.pending));
                        gameMap.At(target).MarkUnsafe(ship);
                        commandQueue.Add(other_ship.Move(other_ship.pending));
                        gameMap.At(other_target).MarkUnsafe(other_ship);
                        continue;
                    }

                    if (gameMap.At(other_target).ship == null) {
                        commandQueue.Add(other_ship.Move(other_ship.pending));
                        gameMap.At(other_ship.position).ship = null;
                        gameMap.At(other_target).MarkUnsafe(other_ship);
                        commandQueue.Add(ship.Move(ship.pending));
                        gameMap.At(ship.position).ship = null;
                        gameMap.At(target).MarkUnsafe(ship);
                        continue;
                    }

                }
                Log.EndSection();

                if (Log.enabled) {
                    Log.StartSection("DEV: Verify ships with no mission");
                    foreach (var ship in me.ships.Values) {
                        if (!mission_control.ShipHasMission(ship)) Log.LogMessage("ERROR: ship missing mission: " + ship);
                    }
                    Log.EndSection();
                }

                Log.StartSection("Spawn new ships");

                var completed_turns_percent = (double)game.turnNumber / (double)Constants.MAX_TURNS * 100.0;
                var my_ships_count = Math.Max(1, me.ships.Count);


                // Spawn new ships if necessary. If there's an enemy blocking it will spawn and destroy it.
                if (completed_turns_percent < 60 && gameMap.GetTotalHalite() / game.players.Count / my_ships_count > 1500) {
                    if (me.halite >= Constants.SHIP_COST && (gameMap.At(me.shipyard).ship == null || gameMap.At(me.shipyard).ship.owner.id != me.id.id)) {
                        commandQueue.Add(me.shipyard.Spawn());
                    }
                }
                Log.EndSection();

                // Output commands.
                game.EndTurn(commandQueue);
            }
        }

        private static bool BlockReturningShips(Ship ship, Direction direction)
        {
            var front_position = game.gameMap.GetTargetPosition(ship.position, direction);
            int count = 0;
            for (int i = 0; i < 4; i++) {
                if (game.gameMap.At(front_position).ship != null) {
                    if (mission_control.ShipIsReturning(game.gameMap.At(front_position).ship)) count++;
                }
                front_position = game.gameMap.GetTargetPosition(front_position, direction);
            }
            return count >= 2;
        }

        private static Direction SelectDetour(Ship ship, Direction direction, Position target)
        {
            var right_direction = DirectionExtensions.TurnRight(direction);
            var right_position = game.gameMap.GetTargetPosition(ship.position, right_direction);
            var right_blocked = HasBlockingShip(ship.owner.id, right_position);

            var left_direction = DirectionExtensions.TurnLeft(direction);
            var left_position = game.gameMap.GetTargetPosition(ship.position, left_direction);
            var left_blocked = HasBlockingShip(ship.owner.id, left_position);

            if (right_blocked && left_blocked) return Direction.STILL; // no detour avaialable

            // define detour direction to cell closest to target or highest halite value.
            var detour_direction = Direction.STILL;
            if (right_blocked == false && left_blocked == true) detour_direction = right_direction;
            if (right_blocked == true && left_blocked == false) detour_direction = left_direction;
            if (right_blocked == false && left_blocked == false) {
                var right_distance = game.gameMap.CalculateDistance(right_position, target);
                var left_distance = game.gameMap.CalculateDistance(left_position, target);
                if (right_distance < left_distance) detour_direction = right_direction;
                if (left_distance < right_distance) detour_direction = left_direction;
                if (right_distance == left_distance) {
                    if (game.gameMap.At(right_position).halite >= game.gameMap.At(left_position).halite) {
                        detour_direction = right_direction;
                    }
                    else {
                        detour_direction = left_direction;
                    }
                }
            }

            // give a continuation move to avoid coming back to current square
            if (detour_next_move.ContainsKey(ship.id.id)) detour_next_move.Remove(ship.id.id);
            detour_next_move.Add(ship.id.id, direction);

            return detour_direction;
        }

        private static void MakeMove(string move_source, Ship ship, Direction direction, bool force_move)
        {
            if (direction == Direction.STILL) {
                commandQueue.Add(ship.StayStill());
                return;
            }

            var target_position = game.gameMap.GetTargetPosition(ship.position, direction);
            if (!force_move && game.gameMap.DangerousPosition(ship.position, target_position)) {
                commandQueue.Add(ship.StayStill());
                return;
            }

            if (IsEndGamePhase && game.gameMap.At(target_position).HasStructure()) {
                commandQueue.Add(ship.Move(direction));
                game.gameMap.At(ship.position).ship = null;
                game.gameMap.At(target_position).ship = ship;
                return;
            }

            if (force_move || game.gameMap.At(target_position).ship == null) {
                commandQueue.Add(ship.Move(direction));
                game.gameMap.At(ship.position).ship = null;
                game.gameMap.At(target_position).ship = ship;
                return;
            }

            ship.pending = direction;
        }
    }
}
