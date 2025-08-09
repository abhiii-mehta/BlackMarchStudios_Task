using UnityEngine;
public class GridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10; //width of the grid
    public int height = 10; //height of the grid
    public float cellSize = 1.0f; // distance between tile centers
    public Material tileMaterial; // material to assign to each tile
    private GameObject _gridParent; // to keep the hierarchy clean

    void Start()
    {
        GenerateGrid(); // generates the grid
    }
    public void GenerateGrid()
    {
        _gridParent = new GameObject("Grid"); // creates a parent object to keep the hierrarchy clean

        for (int x = 0; x < width; x++) // loops through to spawn the grid
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cubetiles = GameObject.CreatePrimitive(PrimitiveType.Cube); // creates a cube which has MeshRenderer and Collider by default

                cubetiles.transform.SetParent(_gridParent.transform);
                cubetiles.transform.position = new Vector3(x * cellSize, 0f, y * cellSize); // sets the position
                cubetiles.name = $"Tile_{x}_{y}"; // sets the name

                if (tileMaterial != null) // if a material is assigned
                {
                    var rend = cubetiles.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        rend.material = tileMaterial;
                    }
                }

                Tile tile = cubetiles.AddComponent<Tile>(); // adds the Tile component
                tile.gridPos = new Vector2Int(x, y);
            }
        }
    }
}
