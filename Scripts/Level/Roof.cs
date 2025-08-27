using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float maxAlpha = 255f; // Change to 255 for 0-255 range
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float lerpSpeed = 2f; // Adjust the speed of interpolation

    public bool playerInside = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    { 
        // Check if the player is inside the trigger collider
        if (!playerInside)
        {
            // Interpolate alpha towards maxAlpha
            float newAlpha = Mathf.Lerp(spriteRenderer.color.a * 255, maxAlpha, lerpSpeed * Time.deltaTime) / 255; // Convert back to 0-1 range
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
        }
        else
        {
            // Interpolate alpha towards minAlpha
            float newAlpha = Mathf.Lerp(spriteRenderer.color.a * 255, minAlpha, lerpSpeed * Time.deltaTime) / 255; // Convert back to 0-1 range
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
