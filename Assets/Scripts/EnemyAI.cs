using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IAI
{
    public GridGenerator gridGenerator;
    public Transform playerTransform;
    public float moveSpeed = 3f;

    private bool isMoving = false;
    private List<Vector2Int> currentPath; // list of grid positions
    private int pathIndex = 0;

    private bool playerHasMoved = true; // enemy moves once at start

    void Update()
    {
        if (!isMoving && playerHasMoved) // checks if the enemy is not moving and the player has moved
        {
            TakeTurn();
            playerHasMoved = false; // reset the flag until next notification
        }
    }

    public void TakeTurn()
    {
        Vector2Int enemyGridPos = WorldPosToGrid(transform.position); // grid position of the enemy
        Vector2Int playerGridPos = WorldPosToGrid(playerTransform.position); // grid position of the player

        List<Vector2Int> adjacentTiles = GetAdjacentFreeTiles(playerGridPos); // find adjacent free tiles

        if (adjacentTiles.Count == 0)
        {
            return; // no need to move
        }

        // Find shortest path to any adjacent tile
        currentPath = null;
        int shortestLength = int.MaxValue;

        foreach (var adj in adjacentTiles)
        {
            List<Vector2Int> path = Pathfinding.FindPath(gridGenerator.GetTilesArray(), enemyGridPos, adj);

            if (path != null && path.Count < shortestLength)
            {
                shortestLength = path.Count;
                currentPath = path;
            }
        }

        if (currentPath != null && currentPath.Count > 1)
        {
            pathIndex = 1; // start moving from next step
            StartCoroutine(MoveAlongPath());
        }
        if (currentPath != null)
            Debug.Log($"Found path of length {currentPath.Count}");
        else
            Debug.Log("No valid path found.");
    }

    IEnumerator MoveAlongPath()
    {
        isMoving = true;
        Debug.Log("Enemy started moving along path.");

        while (pathIndex < currentPath.Count)
        {
            Vector3 targetPos = gridGenerator.GetTileAtPosition(currentPath[pathIndex].x, currentPath[pathIndex].y).worldPosition; // get target position
            targetPos.y = transform.position.y; // keep current y position

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime); // move towards target
                yield return null;
            }
            pathIndex++;
        }
        isMoving = false;
        Debug.Log("Enemy finished moving.");
    }

    List<Vector2Int> GetAdjacentFreeTiles(Vector2Int pos) // returns list of adjacent free tiles
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
            Tile tile = gridGenerator.GetTileAtPosition(checkPos.x, checkPos.y); // get tile at position

            if (tile != null && !tile.isBlocked) // if tile exists and is not blocked
            {
                freeTiles.Add(checkPos);
            }
        }
        return freeTiles;
    }

    Vector2Int WorldPosToGrid(Vector3 worldPos) // converts world position to grid position
    {
        int x = Mathf.RoundToInt(worldPos.x / gridGenerator.cellSize);
        int y = Mathf.RoundToInt(worldPos.z / gridGenerator.cellSize);
        return new Vector2Int(x, y);
    }
    public void OnPlayerMoved()
    {
        playerHasMoved = true; // player has moved
    }
}
