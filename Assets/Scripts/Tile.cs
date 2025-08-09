using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Tile : MonoBehaviour
{
    public Vector2Int gridPos; // (x,y) of the grid
    public bool isBlocked = false; // to check if the tile is blocked, defualt is false
    public Vector3 worldPosition; // the position in the world space
    public GameObject occupant; // to assing player/enemy to the tile

    private Renderer _renderer; // would allow simple highlights / reset
    private Color _originalColor;
    private Vector3 _originalScale;

    void Awake()
    {
        worldPosition = transform.position; // would cache the world position and renderer to access it quickly later 
        _renderer = GetComponent<Renderer>();

        if (_renderer != null)
        {
            _originalColor = _renderer.material.color; // stores the colour of the renderer to be restored later
        }
        _originalScale = transform.localScale; // stores the scale of the tile to be restored later
    }
    public void Highlight(Color highlightColor, float scaleMultiplier = 1.05f) // highlights the tile
    {
        if (_renderer != null)
        {
            _renderer.material = new Material(_renderer.material);
            _renderer.material.color = highlightColor;
        }

        transform.localScale = _originalScale * scaleMultiplier;
    } 
    public void ResetVisual() // reset all th4e visuals back to original
    {
        if (_renderer != null)
        {
            _renderer.material.color = _originalColor;
        }

        transform.localScale = _originalScale;
    }
}
