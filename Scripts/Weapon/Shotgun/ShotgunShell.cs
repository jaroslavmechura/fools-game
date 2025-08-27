using System.Collections.Generic;
using UnityEngine;

public class ShotgunShell : MonoBehaviour
{

    [SerializeField] private GameObject singleBullet;
    [SerializeField] private int bulletCountPerShot;
    [SerializeField] private float spreadAngle;

    [Header("--- Audio ---")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private float hearingRadius;
    [SerializeField] private float roomRadius;
    [SerializeField] private bool hearOn = true;
    [SerializeField] private bool debugDrawEnabled = true;

    void Start()
    {
        for (int i = 0; i < bulletCountPerShot; i++)
        {
            float randomRotation = Random.Range(-spreadAngle / 2, spreadAngle / 2);

            GameObject newBullet = Instantiate(singleBullet, transform.position, transform.rotation);
            newBullet.transform.Rotate(Vector3.forward, randomRotation, Space.Self);
        }

        if (hearOn) DetectEnemies();

    }

    private void DetectEnemies()
    {
        List<Transform> tempToPass = new List<Transform>();

        Collider2D[] coll = Physics2D.OverlapCircleAll(transform.position, roomRadius, roomLayer);
        foreach (Collider2D room in coll)
        {
            foreach (Transform x in room.transform)
            {
                tempToPass.Add(x);
            }
        }


        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, hearingRadius, enemyLayer);
        foreach (Collider2D collider in colliders)
        {

            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Call a method in Enemy to alert them

                foreach (Transform t in tempToPass)
                {
                    enemy.tempPatrolPoints.Add(t);
                }

                enemy.EnterSearchMode();
            }

        }
    }

    private void OnDrawGizmos()
    {
        if (debugDrawEnabled)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, hearingRadius);
            Destroy(gameObject, 1f); // Destroy the gizmo after 1 second
        }
    }


}
