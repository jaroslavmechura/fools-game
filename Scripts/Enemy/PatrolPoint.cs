using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    [Header("--- PointStats ---")]
    public float waitAtPoint;
    public bool isStay;
    public float topBorder;
    public float bottomBorder;

    void Start()
    {
        if (isStay)
        {
            waitAtPoint = Random.Range(bottomBorder, topBorder);
        }
        else {
            waitAtPoint = 0f;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null) { spriteRenderer.enabled = false; }

    }
}
