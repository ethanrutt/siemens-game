using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotsPositioner : MonoBehaviour
{
    public Camera mainCamera; // Reference to the Main Camera
    public Transform slotsParent; // Reference to the CardSlots container
    public float padding = 0.1f; // Padding from edges as a percentage of the screen width
    public float minSpacing = 0.1f; // Minimum spacing between slots
    public float maxSpacing = 1.0f; // Maximum spacing between slots

    void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Calculate screen dimensions in world space
        float screenWidthInWorld = mainCamera.orthographicSize * 2.0f * mainCamera.aspect;

        // Get the number of child slots
        int slotCount = slotsParent.childCount;
        if (slotCount == 0) return;

        // Calculate the width of a single slot using its scale and bounds
        Transform firstSlot = slotsParent.GetChild(0);
        float slotWidth = firstSlot.localScale.x; // Assuming uniform scaling (use bounds for non-uniform scaling)
        float totalSlotWidth = slotWidth * slotCount;

        // Calculate available width for spacing
        float availableWidth = screenWidthInWorld - (screenWidthInWorld * padding * 2) - totalSlotWidth;
        float spacing = Mathf.Clamp(availableWidth / (slotCount - 1), minSpacing, maxSpacing);

        // Position slots dynamically
        for (int i = 0; i < slotCount; i++)
        {
            Transform slot = slotsParent.GetChild(i);
            float xPosition = -(screenWidthInWorld / 2) + (screenWidthInWorld * padding) + i * (slotWidth + spacing) + (slotWidth / 2);
            //float yPosition = -(mainCamera.orthographicSize) + (slot.localScale.y) / 2;
            slot.localPosition = new Vector3(xPosition, slot.localPosition.y, slot.localPosition.z);
        }

        // Debugging (Optional)
        Debug.Log($"Screen Width: {screenWidthInWorld}, Slot Width: {slotWidth}, Spacing: {spacing}");
    }
}