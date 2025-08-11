using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IAI
{
    public GridGenerator gridGenerator;
    public Transform playerTransform;
    public float moveSpeed = 3f;

    private bool isMoving = false;
    private List<Vector2Int> currentPath; // path to the player
    private int pathIndex = 0;

    private TurnManager turnManager;

    public void SetTurnManager(TurnManager tm)
    {
        turnManager = tm;
    }
    public void TakeTurn()
    {
        if (isMoving) return; // safety check

        Vector2Int enemyGridPos = WorldPosToGrid(transform.position);
        Vector2Int playerGridPos = WorldPosToGrid(playerTransform.position);

        Tile[,] tiles = gridGenerator.GetTilesArray(); // tiles[x, y]

        tiles[playerGridPos.x, playerGridPos.y].isBlocked = true; // player position is blocked 
        tiles[enemyGridPos.x, enemyGridPos.y].isBlocked = true; // enemy position is blocked

        List<Vector2Int> adjacentTiles = GetAdjacentFreeTiles(playerGridPos); // adjacentTiles[x, y] 

        if (adjacentTiles.Count == 0)
        {
            EndTurn();
            return;
        }

        // Pick the shortest path to an adjacent tile
        currentPath = null;
        int shortestLength = int.MaxValue;

        foreach (var adj in adjacentTiles)
        {
            List<Vector2Int> path = Pathfinding.FindPath(tiles, enemyGridPos, adj);
            if (path != null && path.Count < shortestLength)
            {
                shortestLength = path.Count;
                currentPath = path;
            }
        }

        // If we have a path, start moving
        if (currentPath != null && currentPath.Count > 1)
        {
            pathIndex = 1; // index 0 is the current position
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
            Vector3 targetPos = gridGenerator.GetTileAtPosition(currentPath[pathIndex].x, currentPath[pathIndex].y).worldPosition;
            targetPos.y = transform.position.y;

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
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
            Tile tile = gridGenerator.GetTileAtPosition(checkPos.x, checkPos.y);

            if (tile != null && !tile.isBlocked)
            {
                freeTiles.Add(checkPos);
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
