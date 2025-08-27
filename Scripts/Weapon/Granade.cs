using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngineInternal;

public class Granade : MonoBehaviour
{
    [Header("--- Type ---")]
    [SerializeField] private GranadeType granadeType;

    [Header("--- Stats ---")]
    [SerializeField] private float detonationTimer;

    [Header("--- Throw ---")]
    [SerializeField] private float throwForce;

    [Header("--- Explosion ---")]
    [SerializeField] private float range;
    [SerializeField] private float damage;
    [SerializeField] private float stunLength;
    [SerializeField] private float explosionForce;

    [SerializeField] private LayerMask explosionMask;
    [SerializeField] private LayerMask wallMask;

    [Header("--- Visuals ---")]
    [SerializeField] private Animator animator;

    [Header("--- Audio ---")]
    [SerializeField] private AudioSource prepSound;
    [SerializeField] private AudioSource throwSound;

    [Header("--- References ---")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject pin;
    [SerializeField] private GameObject safety;
    [SerializeField] private GameObject granadeExplosion;
    
    [Header("--- Debug ---")]
    [SerializeField] private bool debugDrawEnabled;
 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
  
    }

    private void Update()
    {
        if (transform.position.z <= 0) {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }
    }


    public void RemovePin() {
        if (granadeType != GranadeType.Impact) {
            Invoke("Detonate", detonationTimer);
        }
       
        pin.transform.SetParent(null);
        pin.GetComponent<Rigidbody2D>().isKinematic = false;
        pin.GetComponent<Collider2D>().enabled = true;
        safety.GetComponent<Rigidbody2D>().mass = 0.10f;
        safety.GetComponent<Rigidbody2D>().drag = 0.01f;

        prepSound.Play();

        Debug.Log("Pin removed.");
    }

    public void Throw() { 
        Invoke("ThrowThrow", 0.15f);

        throwSound.Play();
    }
    public void ThrowThrow()
    {

        transform.SetParent(null);
        rb.AddForce((transform.up + transform.right) * throwForce, ForceMode2D.Impulse);

        rb.freezeRotation = true;

        safety.transform.SetParent(null);
        safety.GetComponent<Collider2D>().enabled = true;
        Vector3 localRight = transform.TransformDirection(Vector3.right);
        float twistAngle = Random.Range(-45, 45);
        Quaternion twistRotation = Quaternion.Euler(0, 0, twistAngle);
        localRight = twistRotation * localRight;
        safety.GetComponent<Rigidbody2D>().isKinematic = false;
        safety.GetComponent<Rigidbody2D>().mass = 0.05f;
        safety.GetComponent<Rigidbody2D>().drag = 0.1f;
        safety.GetComponent<Rigidbody2D>().AddForce(localRight * 0.25f, ForceMode2D.Impulse);
        safety.GetComponent<Rigidbody2D>().angularDrag = 1.15f;
        safety.GetComponent<Rigidbody2D>().AddTorque(0.1f, ForceMode2D.Impulse);

        Debug.Log("Throw.");
    }

    public void ThrowThrow(bool drop)
    {
        transform.SetParent(null);
        rb.AddForce((transform.up + transform.right) * 1, ForceMode2D.Impulse);
        

        safety.transform.SetParent(null);
        safety.GetComponent<Collider2D>().enabled = true;
        Vector3 localRight = transform.TransformDirection(Vector3.right);
        float twistAngle = Random.Range(-45, 45);
        Quaternion twistRotation = Quaternion.Euler(0, 0, twistAngle);
        localRight = twistRotation * localRight;
        safety.GetComponent<Rigidbody2D>().isKinematic = false;
        safety.GetComponent<Rigidbody2D>().mass = 0.05f;
        safety.GetComponent<Rigidbody2D>().drag = 0.1f;
        safety.GetComponent<Rigidbody2D>().AddForce(localRight * 0.25f, ForceMode2D.Impulse);
        safety.GetComponent<Rigidbody2D>().angularDrag = 1.15f;
        safety.GetComponent<Rigidbody2D>().AddTorque(0.1f, ForceMode2D.Impulse);

        Debug.Log("Throw.");
    }

    public void Detonate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, explosionMask);

        foreach (Collider2D hit in hits)
        {
            if (IsWallBlockingExplosion(hit.gameObject)) continue;

            // Calculate the distance between the hit object and the epicenter
            float distanceFromEpicenter = Vector2.Distance(transform.position, hit.transform.position);

            // Calculate damage based on the distance from the epicenter
            float scaledDamage = damage * (1 - Mathf.Clamp01(distanceFromEpicenter / range));

            if (hit.tag == "Bullet" || hit.tag == "ThrowableObject")
            {
                // Apply explosion force to the hit object
                Rigidbody2D hitRb = hit.gameObject.GetComponent<Rigidbody2D>();
                if (hitRb != null)
                {
                    Vector2 direction = hit.transform.position - transform.position;
                    hitRb.AddForce(direction.normalized * explosionForce/2, ForceMode2D.Impulse);
                }
            }

            else if (hit.tag == "BulletCasing")
            {
                // Apply explosion force to the hit object
                Rigidbody2D hitRb = hit.gameObject.GetComponent<Rigidbody2D>();
                if (hitRb != null)
                {
                    Vector2 direction = hit.transform.position - transform.position;
                    hitRb.AddForce(direction.normalized * explosionForce/50, ForceMode2D.Impulse);
                }
            }
            else if (hit.tag == "Player")
            {
                // Apply scaled damage to the player
                hit.gameObject.GetComponent<PlayerControler>().TakeDamage(scaledDamage, transform.position);
            }
            else if (hit.tag == "Enemy")
            {
                // Apply stun and scaled damage to the enemy
                hit.gameObject.GetComponent<Enemy>().TakeStun(stunLength);

                // Apply explosion force to the enemy
                Rigidbody2D hitRb = hit.gameObject.GetComponent<Rigidbody2D>();
                if (hitRb != null)
                {
                    Vector2 direction = hit.transform.position - transform.position;
                    hitRb.AddForce(direction.normalized * explosionForce, ForceMode2D.Impulse);
                }
                
                hit.gameObject.GetComponent<Enemy>().TakeGranade(scaledDamage, stunLength, transform);
            }
            else if (hit.tag == "ObjectHit")
            {
                // Apply scaled damage to the object
                hit.gameObject.GetComponent<ObjectHit>().TakeDamage(scaledDamage/5);
            }
        }

        Debug.Log("Explosion.");
        GameObject explosion = Instantiate(granadeExplosion, transform.position, Quaternion.identity, null);
        explosion.transform.SetParent(null);
        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        if (debugDrawEnabled)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }


    // Check if there is a wall blocking the explosion impact
    private bool IsWallBlockingExplosion(GameObject obj)
    {
        Vector2 explosionCenter = transform.position;
        Vector2 targetPosition = obj.transform.position;
        RaycastHit2D hit = Physics2D.Linecast(explosionCenter, targetPosition, wallMask);
        return (hit.collider != null);
    }
}
