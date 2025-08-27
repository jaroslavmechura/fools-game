using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedInteraction : MonoBehaviour
{
    public float pushForce;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();


        // Apply force in the random direction
        rb.AddForce(Random.insideUnitCircle.normalized * pushForce, ForceMode2D.Force);
    }


}
