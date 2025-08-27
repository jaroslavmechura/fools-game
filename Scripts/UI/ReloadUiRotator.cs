using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadUiRotator : MonoBehaviour
{
    public float rotationSpeed = 30f;
    void Update()
    {
        transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
    }
}
