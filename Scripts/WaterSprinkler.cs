using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSprinkler : MonoBehaviour
{
    // Speed of rotation
    public float rotationSpeed = 50f;

    private Transform deviceTransform; // Reference to the transform of the child object named "Device"

    void Start()
    {
        // Find the child object named "Device" and get its transform
        deviceTransform = transform.Find("Device");

        if (deviceTransform == null)
        {
            Debug.LogError("Child object named 'Device' not found!");
        }
    }

    void Update()
    {
        // Check if the child transform reference is valid
        if (deviceTransform != null)
        {
            // Rotate the child object around its local Z-axis at the specified speed
            deviceTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}
