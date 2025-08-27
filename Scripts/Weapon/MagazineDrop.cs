
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineDrop : MonoBehaviour
{
    [Header("--- MagDrop ---")]
    [SerializeField] private GameObject magazine;
    [SerializeField] private float forceToRight = 10f; 
    [SerializeField] private float twistAmount = 5f;
    [SerializeField] private float scaleSize;
    [SerializeField] private float spawnRotationValue;


    public void SpawnMagazine()
    {
        GameObject emptyMagazine = Instantiate(magazine, transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + Random.Range(-spawnRotationValue, spawnRotationValue)));
        emptyMagazine.transform.localScale *= scaleSize;
        Rigidbody2D rbEmptyCase = emptyMagazine.GetComponent<Rigidbody2D>();

        Vector3 localRight = transform.TransformDirection(Vector3.right);

        float twistAngle = Random.Range(-spawnRotationValue, spawnRotationValue);
        Quaternion twistRotation = Quaternion.Euler(0, 0, twistAngle);
        localRight = twistRotation * localRight;

        float randomSpeedForce = Random.Range(0.25f, forceToRight);

        rbEmptyCase.AddForce(localRight * randomSpeedForce, ForceMode2D.Impulse);

        float twistDirection = Random.Range(-1f, 1f);
        if (twistDirection == 0) twistDirection = -1f;
        rbEmptyCase.angularVelocity = twistDirection * twistAmount;

        rbEmptyCase.drag = 5f;
        rbEmptyCase.angularDrag = 10f;
    }
}