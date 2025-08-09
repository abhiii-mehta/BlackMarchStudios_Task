using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MouseSelector : MonoBehaviour
{
    public Camera mainCamera; // main camera
    public TextMeshProUGUI tileInfoText; // UI text that would show the grid position
    public LayerMask raycastLayer = ~0;   // the layers which would have raycast against

    private Tile _lastHoveredTile = null; // to remember the last tile to reset the visuals

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main; // use Camera.main if anything isnt assignmed

        if (tileInfoText != null)
            tileInfoText.text = ""; // sets the UI text to empty at start
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // checks the ray from camera
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 100f, raycastLayer)) // check for the raycast hit
        {
            Tile tile = hitInfo.collider.GetComponent<Tile>(); // gets the Tile component from the collider

            if (tile != null)
            {
                if (tile != _lastHoveredTile) // when player is hovering over a new tile, highlight it and reset previous one
                {
                    if (_lastHoveredTile != null)
                        _lastHoveredTile.ResetVisual();

                    tile.Highlight(Color.yellow, 1.05f);
                    _lastHoveredTile = tile;
                }

                if (tileInfoText != null) // updates the UI text with the tile info
                {
                    tileInfoText.text = $"Grid Position: ({tile.gridPos.x}, {tile.gridPos.y})\nBlocked: {tile.isBlocked}";
                }

                return;
            }
        }

        ClearHover(); // resets the visuals and the UI text
    }

    private void ClearHover() // resetting
    {
        if (_lastHoveredTile != null)
        {
            _lastHoveredTile.ResetVisual();
            _lastHoveredTile = null;
        }

        if (tileInfoText != null)
            tileInfoText.text = "";
    }
}
