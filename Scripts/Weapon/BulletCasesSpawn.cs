using UnityEngine;

public class BulletCasesSpawn : MonoBehaviour
{
    [SerializeField] private GameObject bulletCase;
    [SerializeField] private float forceToRight = 10f;
    [SerializeField] private float twistAmount = 5f;
    [SerializeField] private float scaleSize;
    [SerializeField] private float spawnRotationValue;

    public void SpawnBulletCase()
    {
        GameObject emptyCase = Instantiate(bulletCase, transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + Random.Range(-spawnRotationValue, spawnRotationValue)));
        emptyCase.transform.localScale *= scaleSize;
        Rigidbody2D rbEmptyCase = emptyCase.GetComponent<Rigidbody2D>();

        Vector3 localRight = transform.TransformDirection(Vector3.right);

        float twistAngle = Random.Range(-spawnRotationValue, spawnRotationValue);
        Quaternion twistRotation = Quaternion.Euler(0, 0, twistAngle);
        localRight = twistRotation * localRight;

        float randomSpeedForce = Random.Range(0.25f, forceToRight);

        rbEmptyCase.AddForce(localRight * randomSpeedForce, ForceMode2D.Impulse);

        float twistDirection = Random.Range(-1f, 1f);
        if (twistDirection == 0) twistDirection = -1f;
        rbEmptyCase.angularVelocity = twistDirection * twistAmount;

        rbEmptyCase.drag = 2f; 
        rbEmptyCase.angularDrag = 3f;
    }
}