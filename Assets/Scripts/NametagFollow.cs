using TMPro;
using UnityEngine;

public class NameTagFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, 0); // offset above the player
    private Camera mainCamera;
    private TextMeshProUGUI nameTagText;

    void Start()
    {
        mainCamera = Camera.main;
        nameTagText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (target == null) return;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset); // convert world position to screen position

        if (screenPos.z < 0) // if the object is behind the camera
        {
            nameTagText.enabled = false;
        }
        else // if the object is in front of the camera
        {
            nameTagText.enabled = true;
            transform.position = screenPos;
        }
    }
}
