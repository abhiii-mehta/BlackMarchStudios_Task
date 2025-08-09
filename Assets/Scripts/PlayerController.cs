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

    void Update()
    {
        if (isMoving)
            return; // disable input while moving

        if (Input.GetMouseButtonDown(0))
        {
            TryMoveToClickedTile();
        }
    }

    void TryMoveToClickedTile() // trying to move to the clicked tile
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, tileLayer)) // check for the raycast hit 
        {
            Tile clickedTile = hit.collider.GetComponent<Tile>();

            if (clickedTile != null && !clickedTile.isBlocked) // if the clicked tile is not blocked
            {
                Vector2Int playerGridPos = GetPlayerGridPosition();
                Vector2Int targetGridPos = clickedTile.gridPos;
                currentPath = Pathfinding.FindPath(gridGenerator.GetTilesArray(), playerGridPos, targetGridPos); // find path

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
        // Find closest tile to current player position
        Vector3 playerPos = transform.position;
        int x = Mathf.RoundToInt(playerPos.x / gridGenerator.cellSize);
        int y = Mathf.RoundToInt(playerPos.z / gridGenerator.cellSize);
        return new Vector2Int(x, y);
    }

    IEnumerator MoveAlongPath()
    {
        isMoving = true;

        while (pathIndex < currentPath.Count)
        {
            Vector3 targetPos = gridGenerator.GetTileAtPosition(currentPath[pathIndex].x, currentPath[pathIndex].y).worldPosition;
            targetPos.y = transform.position.y; // keep player height

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            pathIndex++;
        }

        isMoving = false;
        currentPath = null;
    }
}
