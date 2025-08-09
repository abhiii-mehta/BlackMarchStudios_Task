using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "GameData/Obstacle Data")]
public class ObstacleData : ScriptableObject
{
    public int width = 10;
    public int height = 10;
    public bool[] blocked; // stores the blocked state of each tile

    private void OnValidate() // gets called whern the asset is created or reset in the inspector
    {
        if (blocked == null || blocked.Length != width * height) // if the array is null or doesn't match the size
        {
            blocked = new bool[width * height];
        }
    }

    public void SetBlocked(int x, int y, bool value) // sets the blocked state of a tile
    {
        blocked[y * width + x] = value;
    }

    public bool GetBlocked(int x, int y) // gets the blocked state of a tile
    {
        return blocked[y * width + x];
    }
}
