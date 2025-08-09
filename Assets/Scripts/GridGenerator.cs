using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10; // width of the grid
    public int height = 10; // height of the grid
    public float cellSize = 1.0f; // distance between tile centers
    public Material tileMaterial; // material to assign to each tile

    private GameObject _gridParent; // keeps hierarchy clean
    private Tile[,] tilesArray; // store references to all tiles

    void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        _gridParent = new GameObject("Grid");

        tilesArray = new Tile[width, height]; // create array to store all tiles

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cubetiles = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cubetiles.transform.SetParent(_gridParent.transform);
                cubetiles.transform.position = new Vector3(x * cellSize, 0f, y * cellSize);
                cubetiles.name = $"Tile_{x}_{y}";
                cubetiles.layer = LayerMask.NameToLayer("Tile");

                if (tileMaterial != null)
                {
                    var rend = cubetiles.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        rend.material = tileMaterial;
                    }
                }

                Tile tile = cubetiles.AddComponent<Tile>(); // add the Tile script to the cube
                tile.gridPos = new Vector2Int(x, y);
                tilesArray[x, y] = tile; // store the tile in the array
            }
        }
    }

    public Tile GetTileAtPosition(int x, int y) // gets a tile by grid coordinates
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return tilesArray[x, y];
        }
        return null; // if its out of range
    }
    public Tile[,] GetTilesArray()
    {
        return tilesArray;
    }

}
