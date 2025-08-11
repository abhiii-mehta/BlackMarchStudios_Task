using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GridGenerator gridGenerator;
    public float moveSpeed = 3f; // units per second
    public LayerMask tileLayer; // layer for tiles

    private bool isMoving = false;
    private List<Vector2Int> currentPath = null;
    private int pathIndex = 0;
    public EnemyAI enemyAI;

    public Animator animator; 
    public Transform modelTransform; // player model
    public float rotationSpeed = 10f; // rotation speed of the model
    public TurnManager turnManager;
    public void SetTurnManager(TurnManager tm)
    {
        turnManager = tm;
    }

    void Update()
    {
        if (!turnManager.IsPlayerTurn() || isMoving)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            TryMoveToClickedTile();
        }
    }
    private Vector2Int WorldPosToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / gridGenerator.cellSize);
        int y = Mathf.RoundToInt(worldPos.z / gridGenerator.cellSize);
        return new Vector2Int(x, y);
    }

    void TryMoveToClickedTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, tileLayer)) // checks for the raycast hit 
        {
            Tile clickedTile = hit.collider.GetComponent<Tile>();

            if (clickedTile != null && !clickedTile.isBlocked) // if the clicked tile is not blocked
            {
                Vector2Int playerGridPos = GetPlayerGridPosition();
                Vector2Int targetGridPos = clickedTile.gridPos;

                Tile[,] tiles = gridGenerator.GetTilesArray(); // tiles[x, y]

                Vector2Int enemyGridPos = WorldPosToGrid(enemyAI.transform.position); // gets the grid position of the enemy
                tiles[enemyGridPos.x, enemyGridPos.y].isBlocked = true;

                currentPath = Pathfinding.FindPath(tiles, playerGridPos, targetGridPos); // finds the path from player to clicked tile

                if (currentPath != null && currentPath.Count > 1) // if path found
                {
                    pathIndex = 1; // start moving to the next tile
                    StartCoroutine(MoveAlongPath());
                }
            }
        }
    }

    Vector2Int GetPlayerGridPosition()
    {
        // Finds the closest tile to current player position
        Vector3 playerPos = transform.position;
        int x = Mathf.RoundToInt(playerPos.x / gridGenerator.cellSize);
        int y = Mathf.RoundToInt(playerPos.z / gridGenerator.cellSize);
        return new Vector2Int(x, y);
    }
    IEnumerator MoveAlongPath()
    {
        isMoving = true;
        animator.SetBool("isWalking", true);

        while (pathIndex < currentPath.Count)
        {
            Vector3 targetPos = gridGenerator.GetTileAtPosition(currentPath[pathIndex].x, currentPath[pathIndex].y).worldPosition; // gets the world position of the target tile
            targetPos.y = transform.position.y;

            Vector3 direction = (targetPos - transform.position).normalized;
             
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime); // move to the target position

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // rotate the model
                }
                yield return null;
            }
            pathIndex++;
        }

        animator.SetBool("isWalking", false);
        modelTransform.rotation = Quaternion.Euler(0f, 225f, 0f);
        isMoving = false;
        currentPath = null;

        turnManager.EndPlayerTurn();
    }

}

