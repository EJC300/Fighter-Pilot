using UnityEngine;

public class WorldSpaceFix : MonoBehaviour
{

    [Header("References")]
    public Transform playerTarget;
    public Camera mainCamera;
    public CanvasGroup canvasGroup; // Add a CanvasGroup to your UI to allow fading

    [Header("Settings")]
    public Vector3 worldOffset = new Vector3(0, 2f, 0);
    [Range(0.01f, 0.1f)] public float screenMargin = 0.05f; // Distance from screen edge (5%)
    public float offScreenAlpha = 0.5f; // Dim the UI when the player is off-screen

    void LateUpdate()
    {
        if (playerTarget == null || mainCamera == null) return;

        // 1. Calculate the target world position
        Vector3 targetWorldPos = playerTarget.position + playerTarget.forward;

        // 2. Convert to Viewport Space (X/Y range from 0 to 1, Z is distance in front of camera)
        Vector3 viewportPos = mainCamera.WorldToScreenPoint(targetWorldPos);

        // 3. Handle the player being physically behind the camera
     

   

        // 5. Clamp the positions to the screen boundaries
        viewportPos.x = Mathf.Clamp(viewportPos.x, -Screen.width, Screen.width);
        viewportPos.y = Mathf.Clamp(viewportPos.y, -Screen.height, Screen.height);

        // 6. Convert clamped viewport position back to screen pixels
        Vector3 screenPos = (viewportPos);
     
        transform.position = screenPos;

       
    }
}




