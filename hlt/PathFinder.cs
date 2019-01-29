using System;
using System.Collections.Generic;
using System.Text;

namespace Halite3.hlt
{
    public class PathFinder
    {
        private Game game;

        private List<Position> closed = new List<Position>();
        private List<Position> open = new List<Position>();
        private Dictionary<Position, Position> came_from = new Dictionary<Position, Position>();
        private Dictionary<Position, int> g_score = new Dictionary<Position, int>();
        private Dictionary<Position, int> f_score = new Dictionary<Position, int>();

        public PathFinder(Game game)
        {
            this.game = game;
            Log.enabled_path_find = true;
        }

        private void InitLists()
        {
            closed.Clear();
            open.Clear();
            came_from.Clear();
            g_score.Clear();
            f_score.Clear();
        }

        public void SearchPath(int playerId, Position start, Position goal, int exclude_blocked_distance)
        {
            Log.LogPathFind("FindLowCostPath: " + start + " -> " + goal);

            InitLists();

            open.Add(start);

            g_score.Add(start, 0);
            f_score.Add(start, CostEstimate(start, goal));

            while (open.Count != 0) {
                var current = GetLowestCost();
                if (current.Equals(goal)) return;

                open.Remove(current);
                closed.Add(current);

                foreach (Direction direction in DirectionExtensions.ALL_CARDINALS) {
                    var neighbor = game.gameMap.GetTargetPosition(current, direction);
                    if (closed.Contains(neighbor)) continue;

                    if (game.gameMap.At(neighbor).ship != null && game.gameMap.CalculateDistance(start, neighbor) < exclude_blocked_distance) continue;

                    if (current == start) {
                        if (game.gameMap.At(neighbor).ship != null && game.gameMap.At(neighbor).ship.owner.id != playerId) continue;
                        if (game.gameMap.DangerousPosition(current, neighbor)) continue;
                    }

                    var tentative_g_score = g_score[current] + game.gameMap.CalculateDistance(current, neighbor);
                    if (!open.Contains(neighbor)) {
                        open.Add(neighbor);
                    }
                    else {
                        if (tentative_g_score >= g_score[neighbor]) continue;
                    }

                    if (!came_from.ContainsKey(neighbor))
                        came_from.Add(neighbor, current);
                    else
                        came_from[neighbor] = current;
                    if (g_score.ContainsKey(neighbor))
                        g_score[neighbor] = tentative_g_score;
                    else
                        g_score.Add(neighbor, tentative_g_score);
                    if (f_score.ContainsKey(neighbor))
                        f_score[neighbor] = g_score[neighbor] + CostEstimate(neighbor, goal);
                    else
                        f_score.Add(neighbor, g_score[neighbor] + CostEstimate(neighbor, goal));
                }
            }

        }

        // The distance from start to a neighbor
        //tentative_gScore := gScore[current] + dist_between(current, neighbor)

        //    if neighbor not in openSet	// Discover a new node
        //        openSet.Add(neighbor)
        //    else if tentative_gScore >= gScore[neighbor]
        //        continue;       

        //    // This path is the best until now. Record it!
        //    cameFrom[neighbor] := current
        //    gScore[neighbor] := tentative_gScore
        //    fScore[neighbor] := gScore[neighbor] + heuristic_cost_estimate(neighbor, goal)

        private Position GetLowestCost()
        {
            Position lowest_position = null;
            int lowest_cost = int.MaxValue;

            foreach (var position in open) {
                if (f_score[position] < lowest_cost) {
                    lowest_cost = f_score[position];
                    lowest_position = position;
                }
            }

            return lowest_position;
        }

        private int CostEstimate(Position from, Position to)
        {
            return game.gameMap.At(from).halite / Constants.MOVE_COST_RATIO;
        }

        public List<Position> GetPath(Position goal)
        {
            var current = goal;
            var total_path = new List<Position>();
            while (came_from.ContainsKey(current)) {
                current = came_from[current];
                total_path.Insert(0, current);
            }
            total_path.Add(goal);
            return total_path;
        }

        public void PrintPath(Position goal)
        {
            if (!Log.enabled_path_find) return;

            var path = GetPath(goal);
            var path_line = "";
            var cost = 0;
            foreach (var position in path) {
                cost += (int)(game.gameMap.At(position).halite / Constants.INSPIRED_MOVE_COST_RATIO);
                path_line += position.ToString() + " (-" + cost + ") ";
            }
            Log.LogPathFind("Path to " + goal.ToString() + ": " + path_line + " Total Cost: " + cost);
        }
    }
    /*
function reconstruct_path(cameFrom, current)
    total_path := {current}
    while current in cameFrom.Keys:
        current := cameFrom[current]
        total_path.append(current)
    return total_path

function A_Star(start, goal)
    // The set of nodes already evaluated
    closedSet := {}

    // The set of currently discovered nodes that are not evaluated yet.
    // Initially, only the start node is known.
    openSet := {start}

    // For each node, which node it can most efficiently be reached from.
    // If a node can be reached from many nodes, cameFrom will eventually contain the
    // most efficient previous step.
    cameFrom := an empty map

    // For each node, the cost of getting from the start node to that node.
    gScore := map with default value of Infinity

    // The cost of going from start to start is zero.
    gScore[start] := 0

    // For each node, the total cost of getting from the start node to the goal
    // by passing by that node. That value is partly known, partly heuristic.
    fScore := map with default value of Infinity

    // For the first node, that value is completely heuristic.
    fScore[start] := heuristic_cost_estimate(start, goal)

    while openSet is not empty
        current := the node in openSet having the lowest fScore[] value
        if current = goal
            return reconstruct_path(cameFrom, current)

        openSet.Remove(current)
        closedSet.Add(current)

        for each neighbor of current
            if neighbor in closedSet
                continue		// Ignore the neighbor which is already evaluated.

            // The distance from start to a neighbor
            tentative_gScore := gScore[current] + dist_between(current, neighbor)

            if neighbor not in openSet	// Discover a new node
                openSet.Add(neighbor)
            else if tentative_gScore >= gScore[neighbor]
                continue;       

            // This path is the best until now. Record it!
            cameFrom[neighbor] := current
            gScore[neighbor] := tentative_gScore
            fScore[neighbor] := gScore[neighbor] + heuristic_cost_estimate(neighbor, goal)      
      
      

    */


}
