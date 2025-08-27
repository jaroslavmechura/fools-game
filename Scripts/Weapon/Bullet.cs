using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("--- Stats---")]
    public float speed;

    [SerializeField] private float damage;
    [SerializeField] private float health;

    [SerializeField] private DamageType damageType;

    [Header("--- Physics ---")]
    public Rigidbody2D rb;

    [SerializeField] private float forceSeconds;
    [SerializeField] private float maxRotationAngle;

    [Header("--- Visuals ---")]
    [SerializeField] private GameObject wallHitParticles;

    [Header("--- Audio ---")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask roomLayer;
    [SerializeField] private float hearingRadius;
    [SerializeField] private float roomRadius;
    [SerializeField] private bool hearOn = true;
    [SerializeField] private bool debugDrawEnabled = true;

    private void Start()
    {
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
                enemy.tempPatrolPoints.Clear();
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
        }
    }

    public void RotateSelf(bool byEnemy) {
        float randomRotation;
        if (byEnemy)
        {
            randomRotation = Random.Range(-maxRotationAngle * 2f, maxRotationAngle * 2f);
            speed *= 0.75f;
        }
        else {
            randomRotation = Random.Range(-maxRotationAngle, maxRotationAngle);
        }

        transform.Rotate(Vector3.forward, randomRotation);
        rb = GetComponent<Rigidbody2D>();
        ApplyUpwardForce();
    }

    public void ApplyUpwardForce()
    {
        Vector2 force = transform.up * speed;
        rb.AddForce(force, ForceMode2D.Impulse);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ObjectHit"))
        {
            ObjectHit objectHit = other.GetComponent<ObjectHit>();

            TakePenetrationalDamage(objectHit.damageToBullet, objectHit.slowToBullet, objectHit.damageReductionToBullet);
            objectHit.TakeDamage(damage);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("BulletBlock"))
        {
            Destroy(gameObject);
        }

        if (collision.collider.CompareTag("Player"))
        {
            if (!collision.gameObject.GetComponent<PlayerInput>().isDodging && !collision.gameObject.GetComponent<PlayerInput>().isDashing) 
            {
                collision.gameObject.GetComponent<PlayerControler>().TakeDamage(damage, transform.position);
            }
        }

        if (collision.collider.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage, forceSeconds, transform, gameObject.CompareTag("ShotgunBullet"), damageType);
        }

        if (collision.collider.CompareTag("Wall"))
        {
            Vector3 fromWall = collision.collider.transform.position - transform.position;
            float targetRotationX = Mathf.Atan2(fromWall.y, -fromWall.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(targetRotationX, 90.0f, 0.0f); // Rotate around Y-axis

            GameObject particles = Instantiate(wallHitParticles, collision.contacts[0].point, transform.rotation);
            Destroy(particles, 1f);
        }

        if ((collision.collider.CompareTag("WeaponBullet") && gameObject.CompareTag("ShotgunBullet")) || (collision.collider.CompareTag("ShotgunBullet") && gameObject.CompareTag("ShotgunBullet")))
        {
            return;
        }
        else
        {
            Destroy(gameObject);
        }

    }


    private void DestroyBullet()
    {
        Destroy(gameObject);
    }


    public void TakePenetrationalDamage(float dmgIn, float slowDownIn, float dmgReduceIn)
    {
        health -= dmgIn;
        damage -= dmgReduceIn;

        if (!gameObject.CompareTag("ShotgunBullet"))
        {
            speed -= slowDownIn;
        }
        else { 
            float randomRotation = Random.Range(-30, 30);
            transform.Rotate(transform.forward, randomRotation);
        }
        

        Vector2 force = transform.up * speed;
        rb.AddForce(force, ForceMode2D.Impulse);

        if (health <= 0)
        {
            DestroyBullet();
        }
    }
}
