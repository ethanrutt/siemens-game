using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardResizer : MonoBehaviour
{
    public Camera mainCamera; // Reference to the Main Camera
    public float padding = 0.1f; // Padding percentage (10%)
    public float defaultScale = 0.07f; // Default scale for cards

    void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        RectTransform parentRect = GetComponent<RectTransform>();
        if (parentRect.childCount == 0) return;

        // Calculate available width in world space
        float screenWidthInWorld = mainCamera.orthographicSize * 2.0f * mainCamera.aspect;
        float availableWidth = screenWidthInWorld - (screenWidthInWorld * padding);

        // Calculate the required scale
        RectTransform firstCardRect = parentRect.GetChild(0).GetComponent<RectTransform>();
        float cardWidth = firstCardRect.rect.width * defaultScale;
        float totalCardWidth = cardWidth * 8;

        float scaleFactor = Mathf.Min(1, availableWidth / totalCardWidth);

        // Combine default scale with calculated scale factor
        float finalScale = defaultScale * scaleFactor;

        // Apply the scale to all cards
        foreach (Transform child in parentRect)
        {
            child.localScale = new Vector3(finalScale, finalScale, 1);
        }

        // Debugging (Optional: To verify calculations)
        Debug.Log($"Screen Width: {screenWidthInWorld}, Available Width: {availableWidth}, Card Width: {cardWidth}, Scale Factor: {scaleFactor}, Final Scale: {finalScale}");
    }
}