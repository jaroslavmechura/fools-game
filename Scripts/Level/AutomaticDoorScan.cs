using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoorScan : MonoBehaviour
{
    [SerializeField] private AutomaticDoor automaticDoor;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            automaticDoor.Open();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            automaticDoor.Close();
        }
    }
}
