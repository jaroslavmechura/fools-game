using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWeaponSlotColor : MonoBehaviour
{
    [SerializeField] private List<Vector2> slots;
    [SerializeField] private int currSlotId;

    [SerializeField] private List<Color> slotsColors;

    [SerializeField] private float smoothSpeed;

    private RectTransform rectTransform;
    private Color targetColor;
    private Vector3 targetPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // Initialize target position and color
        targetPosition = slots[currSlotId];
        targetColor = slotsColors[currSlotId];
    }

    public void ChangeSlot(int id)
    {
        currSlotId = id;
        // Update target position and color when changing slot
        targetPosition = slots[currSlotId];
        targetColor = slotsColors[currSlotId];
    }

    private void Update()
    {
        // Smoothly move to target position
        rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, targetPosition, smoothSpeed * Time.deltaTime);
        // Smoothly change color to target color
        GetComponent<UnityEngine.UI.Image>().color = Color.Lerp(GetComponent<UnityEngine.UI.Image>().color, targetColor, smoothSpeed * Time.deltaTime);
    }
}


