using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public static List<Vector2Int> FindPath(Tile[,] grid, Vector2Int start, Vector2Int target) // finds the path from start to target using Breadth-First Search
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        bool[,] visited = new bool[width, height];
        Vector2Int[,] parents = new Vector2Int[width, height];

        Queue<Vector2Int> queue = new Queue<Vector2Int>(); // use Queue for BFS
        queue.Enqueue(start);
        visited[start.x, start.y] = true;
        parents[start.x, start.y] = new Vector2Int(-1, -1);

        Vector2Int[] directions = { // 4 cardinal directions
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        while (queue.Count > 0) // while queue is not empty
        {
            Vector2Int current = queue.Dequeue();

            if (current == target) // if we found the target
            {
                // Reconstruct path
                List<Vector2Int> path = new List<Vector2Int>();
                while (current.x != -1) // while current is not the start
                {
                    path.Add(current);
                    current = parents[current.x, current.y];
                }
                path.Reverse();
                return path;
            }

            foreach (var dir in directions) // for each direction
            {
                Vector2Int neighbor = current + dir;

                if (neighbor.x < 0 || neighbor.x >= width || neighbor.y < 0 || neighbor.y >= height) // if neighbor is out of bounds
                    continue;

                if (visited[neighbor.x, neighbor.y]) // already visited
                    continue;

                if (grid[neighbor.x, neighbor.y].isBlocked) // if neighbor is blocked
                    continue;

                visited[neighbor.x, neighbor.y] = true;
                parents[neighbor.x, neighbor.y] = current;
                queue.Enqueue(neighbor);
            }
        }

        return null; // no path found
    }
}
