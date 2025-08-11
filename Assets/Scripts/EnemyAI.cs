using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IAI
{
    public GridGenerator gridGenerator;
    public Transform playerTransform;
    public float moveSpeed = 3f;

    private bool isMoving = false;
    private List<Vector2Int> currentPath;
    private int pathIndex = 0;

    private TurnManager turnManager;

    public void SetTurnManager(TurnManager tm)
    {
        turnManager = tm;
    }

    public void TakeTurn()
    {
        if (isMoving) return;

        Vector2Int enemyGridPos = WorldPosToGrid(transform.position);
        Vector2Int playerGridPos = WorldPosToGrid(playerTransform.position);

        Tile[,] tiles = gridGenerator.GetTilesArray();

        // Always mark both player & enemy positions as blocked before pathfinding
        tiles[playerGridPos.x, playerGridPos.y].isBlocked = true;
        tiles[enemyGridPos.x, enemyGridPos.y].isBlocked = true;

        // Find all tiles around the player that are free
        List<Vector2Int> adjacentTiles = GetAdjacentFreeTiles(playerGridPos);
        if (adjacentTiles.Count == 0)
        {
            EndTurn();
            return;
        }

        // Pick shortest path to an adjacent tile
        currentPath = null;
        int shortestLength = int.MaxValue;

        foreach (var adj in adjacentTiles)
        {
            List<Vector2Int> path = Pathfinding.FindPath(tiles, enemyGridPos, adj); // finds the path from enemy to adjacent tile
            if (path != null && path.Count < shortestLength)
            {
                shortestLength = path.Count;
                currentPath = path;
            }
        }

        if (currentPath != null && currentPath.Count > 1)
        {
            pathIndex = 1; // index 0 is enemy's current tile
            StartCoroutine(MoveAlongPath());
        }
        else
        {
            EndTurn();
        }
    }

    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        while (pathIndex < currentPath.Count)
        {
            Vector3 targetPos = gridGenerator
                .GetTileAtPosition(currentPath[pathIndex].x, currentPath[pathIndex].y) // gets the world position of the target tile
                .worldPosition;
            targetPos.y = transform.position.y;

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime); // move to the target position
                yield return null;
            }

            pathIndex++;
        }

        isMoving = false;
        EndTurn();
    }

    private void EndTurn()
    {
        turnManager.EndEnemyTurn();
    }

    private List<Vector2Int> GetAdjacentFreeTiles(Vector2Int pos)
    {
        List<Vector2Int> freeTiles = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
        };

        foreach (var dir in directions)
        {
            Vector2Int checkPos = pos + dir;

            Tile tile = gridGenerator.GetTileAtPosition(checkPos.x, checkPos.y); // tiles[x, y] 

            if (tile != null && !tile.isBlocked)
            {
                var path = Pathfinding.FindPath(gridGenerator.GetTilesArray(), WorldPosToGrid(transform.position), checkPos); // // Test pathfinding here to ensure it's reachable
                if (path != null && path.Count > 0)
                {
                    freeTiles.Add(checkPos);
                }
            }
        }

        return freeTiles;
    }


    private Vector2Int WorldPosToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / gridGenerator.cellSize);
        int y = Mathf.RoundToInt(worldPos.z / gridGenerator.cellSize);
        return new Vector2Int(x, y);
    }
}
