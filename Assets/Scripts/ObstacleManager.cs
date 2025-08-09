using UnityEngine;
using System.Collections;
public class ObstacleManager : MonoBehaviour
{
    public ObstacleData obstacleData;     // assign in Inspector
    public GameObject obstaclePrefab;     // a red sphere prefab (or null to create primitive)
    public float heightOffset = 0.5f;     // how high above the tile the sphere should be
    public GridGenerator gridGenerator;   // assign in Inspector so we can find tiles

    IEnumerator Start()
    {
        if (obstacleData == null)
        {
            Debug.LogWarning("ObstacleManager: No ObstacleData assigned.");
            yield break;
        }

        if (gridGenerator == null)
        {
            Debug.LogWarning("ObstacleManager: No GridGenerator assigned.");
            yield break;
        }

        yield return null;
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        for (int x = 0; x < obstacleData.width; x++)
        {
            for (int y = 0; y < obstacleData.height; y++)
            {
                if (obstacleData.GetBlocked(x, y)) // called when the tile is marked as blocked 
                {
                    Vector3 spawnPos = new Vector3(x, heightOffset, y);

                    GameObject obstacleGO;

                    if (obstaclePrefab != null) // uses a prefab if assigned, otherwise generates one placeholder
                    {
                        obstacleGO = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
                    }
                    else
                    {
                        obstacleGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        obstacleGO.transform.position = spawnPos;
                        obstacleGO.GetComponent<Renderer>().material.color = Color.red;
                    }

                    obstacleGO.name = $"Obstacle_{x}_{y}";
                    Tile tile = gridGenerator.GetTileAtPosition(x, y); // finds the tile at this position 
                    if (tile != null)
                    {
                        tile.isBlocked = true;
                    }
                }
            }
        }
    }
}
