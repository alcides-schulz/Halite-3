using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Halite3.hlt
{
    public class Combat
    {
        private Game game;

        private List<Position> CombatPositions = new List<Position>();
        private List<Ship> MyShips = new List<Ship>();
        private List<Ship> EnemyShips = new List<Ship>();
        private int BestScore = 0;

        private static int INFINITY = 1000000;

        public Combat(Game game)
        {
            this.game = game;
        }

        public void EvaluateCombatAreas()
        {
            LocateCombatPositions();
            foreach (var position in CombatPositions) {
                EvaluatePosition(position);
                break;
            }
        }

        private void LocateCombatPositions()
        {
            CombatPositions.Clear();
            foreach(var ship in game.me.ships.Values) {
                foreach(var direction in DirectionExtensions.ALL_CARDINALS) {
                    var position = game.gameMap.GetTargetPosition(ship.position, direction);
                    if (IsCombatPosition(position)) CombatPositions.Add(position);
                }
            }
        }

        private bool IsCombatPosition(Position position)
        {
            if (game.gameMap.At(position).ship != null && game.gameMap.At(position).ship.owner.id != game.me.id.id) return true;
            foreach (var direction in DirectionExtensions.ALL_CARDINALS) {
                var neighbour = game.gameMap.GetTargetPosition(position, direction);
                if (game.gameMap.At(neighbour).ship != null && game.gameMap.At(neighbour).ship.owner.id != game.me.id.id) return true;
            }
            return false;
        }

        private void EvaluatePosition(Position position)
        {
            SelectShips(position);
            if (MyShips.Count == 0) return;
            if (MyShips.Count + EnemyShips.Count > 5) return;
            BestScore = -Combat.INFINITY;
            EvaluateMyMoves(0);
            foreach(var ship in MyShips) {
                ship.make_combat_best_move = true;
            }
            PrintCombatData(position);
        }

        private void PrintCombatData(Position position)
        {
            Log.LogMessage("Combat Position: " + position);
            foreach (var ship in MyShips) {
                Log.LogMessage("     MyShip: " + ship);
            }
            foreach (var ship in EnemyShips) {
                Log.LogMessage("  EnemyShip: " + ship);
            }
        }

        private void SelectShips(Position contact_position)
        {
            MyShips.Clear();
            EnemyShips.Clear();

            var area = game.gameMap.CombatArea[contact_position];
            foreach (var position in area) { 
                if (game.gameMap.At(position).ship != null) {
                    if (game.gameMap.At(position).ship.halite >= Zluhcsh3.SHIP_RETURN_THRESHOLD || game.gameMap.At(position).ship.moved) {
                        MyShips.Clear();
                        EnemyShips.Clear();
                        return;
                    }
                    if (game.gameMap.At(position).ship.owner.id == game.me.id.id)
                        MyShips.Add(game.gameMap.At(position).ship);
                    else
                        EnemyShips.Add(game.gameMap.At(position).ship);
                }
            }
        }

        //List myAnts
        //List enemyAnts
        //bestValue = -Infinity
        //
        //void max(antIndex) 
        //{
        //    if antIndex < myAnts.size 
        //       myAnt = myAnts[antIndex]
        //       for each possible move of myAnt
        //           simulate move
        //           max(antIndex+1)
        //           undo move
        //    else 
        //       value = min(0)
        //       if value > bestValue
        //          bestValue = value
        //          save the current simulated moves of all my ants
        //}
        //
        private void EvaluateMyMoves(int index)
        {
            if (index < MyShips.Count) {
                var moves = GetMoves(MyShips[index]);
                foreach (var move in moves) {
                    MakeMove(MyShips[index], move, EnemyShips.Count >= 2);
                    EvaluateMyMoves(index + 1);
                    UndoMove(MyShips[index], move);
                }
            }
            else {
                int score = EvaluateEnemyMoves(0);
                var eval = "";
                foreach (var ship in MyShips) {
                    eval += ship.position + " " + ship.current_move + " ";
                }
                Log.LogMessage("Evaluation: " + eval + " score: " + score);
                if (score > BestScore) {
                    BestScore = score;
                    foreach (var ship in MyShips) {
                        ship.best_move = ship.current_move;
                    }
                }
            }
        }
        //int min(antIndex) 
        //{
        //    if antIndex < enemyAnts.size
        //        minValue = +Infinity
        //        enemyAnt = enemyAnts[antIndex]
        //        for each possible move of enemyAnt
        //            simulate move
        //            value = min(antIndex+1)
        //            undo move
        //            if value < bestValue
        //               return -Infinity  // cut!
        //            if value < minValue
        //                minValue = value
        //        return minValue
        //    else
        //        return evaluate()
        //}

        private int EvaluateEnemyMoves(int index)
        {
            if (index < EnemyShips.Count) {
                int min_score = Combat.INFINITY;
                var moves = GetMoves(EnemyShips[index]);
                foreach (var move in moves) {
                    MakeMove(EnemyShips[index], move, MyShips.Count >= 2);
                    int score = EvaluateEnemyMoves(index + 1);
                    UndoMove(EnemyShips[index], move);
                    if (score < BestScore) return -Combat.INFINITY;
                    if (score < min_score) min_score = score;
                }
                return min_score;
            }
            else {
                return Evaluate();
            }
        }

        private void MakeMove(Ship ship, Direction direction, bool inspired)
        {
            ship.current_move = direction;
            if (direction == Direction.STILL) {
                ship.combat_target = ship.position;
                ship.combat_collected = (int)(game.gameMap.At(ship).halite / Constants.EXTRACT_RATIO);
                if (inspired) ship.combat_collected *= 3;
                if (ship.halite + ship.combat_collected > 1000) ship.combat_collected = 1000 - ship.halite;
            }
            else {
                ship.combat_target = game.gameMap.GetTargetPosition(ship.position, direction);
                ship.combat_costs = (int)(game.gameMap.At(ship.position).halite / Constants.MOVE_COST_RATIO);
            }
            game.gameMap.At(ship.combat_target).combat_ships.Add(ship);
        }

        private void UndoMove(Ship ship, Direction direction)
        {
            ship.combat_collected = 0;
            ship.combat_costs = 0;
            game.gameMap.At(ship.combat_target).combat_ships.Remove(ship);
        }

        private List<Direction> GetMoves(Ship ship)
        {
            var moves = new List<Direction>();

            moves.Add(Direction.STILL);

            if (game.gameMap.At(ship.position).halite / Constants.MOVE_COST_RATIO > ship.halite) return moves;

            foreach (var direction in DirectionExtensions.ALL_CARDINALS) {
                var target = game.gameMap.GetTargetPosition(ship.position, direction);
                if (!game.gameMap.DangerousPosition(ship.position, target)) moves.Add(direction);
            }

            return moves;
        }

        private int Evaluate()
        {
            var my_score = 0;
            foreach(var my_ship in MyShips) {
                if (game.gameMap.At(my_ship.combat_target).combat_ships.Count == 1)
                    my_score += my_ship.halite + my_ship.combat_collected - my_ship.combat_costs;
                else
                    my_score -= my_ship.halite;
            }
            var enemy_score = 0;
            foreach (var enemy_ship in EnemyShips) {
                if (game.gameMap.At(enemy_ship.combat_target).combat_ships.Count == 1)
                    enemy_score += enemy_ship.halite + enemy_ship.combat_collected - enemy_ship.combat_costs;
                else
                    enemy_score -= enemy_ship.halite;
            }

            return my_score - enemy_score;
        }
    }

}
