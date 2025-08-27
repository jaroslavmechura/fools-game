using UnityEngine;

public class ThrowableObject : LevelObject
{
    [Header("--- Throw ---")]
    [SerializeField] private float throwForce;
    [SerializeField] private float rotationForce;
    [SerializeField] private float stunDuration;
    [SerializeField] private float damage;

    [Header("--- Grab ---")]
    [SerializeField] private GameObject grabPoint;
    [SerializeField] private bool notPickedUpYet;
    [SerializeField] private bool isGrabbed;

    [Header("--- Audio ---")]
    public AudioSource wallHitSound;
    public AudioSource personHitSound;
    public AudioSource weaponThrowSound;

    [Header("--- Physics ---")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D objectCollider;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) Debug.LogError("RigidBody not found.");

        objectCollider = GetComponent<BoxCollider2D>();
        if (objectCollider == null) Debug.LogError("BoxCollider not found.");
    }


    void Update()
    {
        if (isGrabbed)
        {
            objectCollider.enabled = false;
            transform.position = grabPoint.transform.position;
        }
    }


    private void EnableCollider()
    {
        objectCollider.enabled = true;
    }


    public void SetGrabPoint(GameObject inputPoint)
    {
        objectCollider.enabled = false;
        grabPoint = inputPoint;
        isGrabbed = true;
    }


    public void Throw(Vector3 casterPosition) 
    {
        isGrabbed = false;
        grabPoint = null;

        gameObject.transform.parent = null;

        Vector3 throwDirection = gameObject.transform.position - casterPosition;

        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.mass = 1f;

        rb.AddForce(throwDirection.normalized * throwForce, ForceMode2D.Impulse);

        if (Random.Range(0, 2) == 0)
        {
            rb.AddTorque(rotationForce, ForceMode2D.Impulse);
        }
        else {
            rb.AddTorque(-rotationForce, ForceMode2D.Impulse);
        }

        notPickedUpYet = false;

        GetComponent<SpriteRenderer>().sortingLayerName = "ThrowableObjectsAbove";

        Invoke("EnableCollider", 0.1f);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (notPickedUpYet) return;

        gameObject.GetComponent<LevelObject>().MaterialHit();

        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<Enemy>().TakeDamage(damage, 0f, transform, false, DamageType.Blunt);
            collision.collider.GetComponent<Enemy>().TakeStun(stunDuration);

            personHitSound.Play();
        }
        else if (collision.collider.CompareTag("Player"))
        {

            collision.collider.GetComponent<PlayerControler>().TakeDamage(damage, transform.position);
            personHitSound.Play();
            
        }
        else {
            wallHitSound.Play();
        }

        Destroy(gameObject);
    }
}
