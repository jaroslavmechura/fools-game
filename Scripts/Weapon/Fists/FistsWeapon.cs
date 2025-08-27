using UnityEngine;

public class FistsWeapon : Weapon
{
    [Header("--- References ---")]
    [SerializeField] private bool left;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private Fist leftFistScript;
    [SerializeField] private Fist rightFistScript;

    [Header("--- Grab ---")]
    public bool isHoldingSomething;
    [SerializeField] private bool isGrabbing;

    [SerializeField] private GameObject grabPoint;
    [SerializeField] private GameObject heldObject;
    [SerializeField] private BoxCollider2D grabCollider;

    [SerializeField] private float grabSpeed;
    [SerializeField] private float grabDuration;
    [SerializeField] private float grabCooldown;
    public float nextGrabTime;
    [SerializeField] private float grabPushForce;

    [SerializeField] private Vector3 lHandOriginalPosition;
    [SerializeField] private Vector3 rHandOriginalPosition;
    [SerializeField] private Vector2 grabPointOriginalPosition;
    [SerializeField] private Vector3 originalPos;

    void Start()
    {
        left = false;
        leftFistScript = leftArm.GetComponent<Fist>();
        rightFistScript = rightArm.GetComponent<Fist>();


        lHandOriginalPosition = leftArm.transform.localPosition;
        rHandOriginalPosition = rightArm.transform.localPosition;
        grabPointOriginalPosition = grabCollider.offset;
        originalPos = transform.localPosition;
        
        
        grabCollider.enabled = false;
        nextGrabTime = Time.time;

        cameraScript = Camera.main.gameObject.GetComponent<CameraFollow>();
    }


    void Update()
    {
      
        if (isGrabbing)
        {
            leftArm.transform.Translate(Vector3.up * grabSpeed / 2f * Time.deltaTime);
            rightArm.transform.Translate(Vector3.up * grabSpeed / 2f * Time.deltaTime);

            Vector2 colliderOffset = grabCollider.offset;
            colliderOffset.y += grabSpeed * Time.deltaTime /2f;
            grabCollider.offset = colliderOffset;

            transform.Translate(Vector3.up * grabSpeed / 2 * Time.deltaTime);

        }
    }


    public override void WeaponInputActions()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > nextFireTime && !isGrabbing && !isHoldingSomething)
        {
            if (left)
            {
                leftFistScript.Punch();
                
                cameraScript.CameraShake(cameraShakeForce);
                
            }
            else
            {
                rightFistScript.Punch();
                cameraScript.CameraShake(cameraShakeForce);
            }

            left = !left;
            nextFireTime = Time.time + 1f / fireRate;

            fireSound.clip = shotClips[0];
            fireSound.Play();
        }
        else if (Input.GetMouseButtonDown(0) && Time.time > nextFireTime && heldObject.CompareTag("Enemy") && isHoldingSomething) {
            left = !left;
            nextFireTime = Time.time + 1f / fireRate;

            fireSound.clip = shotClips[0];
            fireSound.Play();
            leftFistScript.hitSound.Play();
            rightFistScript.hitSound.Play();

            float health = heldObject.GetComponent<Enemy>().health;

            heldObject.GetComponent<Enemy>().TakeDamage(leftFistScript.damage, 0f, transform, false, DamageType.Blunt);

            if (health <= leftFistScript.damage) 
            {
                heldObject.transform.parent = null;

                heldObject.GetComponent<Enemy>().isGrabbed = false;

                heldObject = null;

                isHoldingSomething = false;
            }
            
        }
        else if (Input.GetMouseButtonDown(1) && Time.time > nextGrabTime && !isHoldingSomething)
        {
            Grab();
            nextGrabTime = Time.time + grabCooldown;
        }
        else if (Input.GetMouseButtonDown(1) && isHoldingSomething)
        {
            ThrowHeldObject();
        }
    }


    private void Grab()
    {
        isGrabbing = true;
        grabCollider.enabled = true;
        Invoke("EndGrabEmpty", grabDuration/10);
    }


    private void EndGrabEmpty()
    {
        isGrabbing = false;
        grabCollider.enabled = false;
        leftArm.transform.localPosition = lHandOriginalPosition;
        rightArm.transform.localPosition = rHandOriginalPosition;
        grabCollider.offset = grabPointOriginalPosition;
        transform.localPosition = originalPos;
    }


    private void EndGrab()
    {
        isGrabbing = false;
        grabCollider.enabled = false;
        leftArm.transform.localPosition = lHandOriginalPosition;
        rightArm.transform.localPosition = rHandOriginalPosition;
        grabCollider.offset = grabPointOriginalPosition;
        transform.localPosition = originalPos;

        if (heldObject != null && heldObject.CompareTag("Enemy"))
        {
            heldObject.transform.parent = null;
            heldObject.GetComponent<Enemy>().isGrabbed = false;
            heldObject = null;
        }

        isHoldingSomething = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //print("LOLOLOLO:    " + other.name);
        if (other.CompareTag("Enemy") && isGrabbing)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.isGrabbed = true;
                enemy.SetGrabPoint(grabPoint);
                isHoldingSomething = true;
                EndGrabEmpty();
                heldObject = other.gameObject;
            }
        }
        if (other.CompareTag("ThrowableObject") && isGrabbing)
        {
            ThrowableObject spawned = other.GetComponent<ThrowableObject>();
            if (spawned != null)
            {
                spawned.SetGrabPoint(grabPoint);
                isHoldingSomething = true;
                EndGrabEmpty();
                heldObject = other.gameObject;
            }
        }

    }


    private void ThrowHeldObject()
    {
        if (heldObject != null && heldObject.CompareTag("Enemy"))
        {
            Enemy enemy = heldObject.GetComponent<Enemy>();
            EndGrab();

            enemy.isGrabbed = false;
            enemy.SetGrabPoint(null);

            float angle = transform.localEulerAngles.z; // Get the local rotation angle in degrees
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));


            enemy.TakeStun(grabDuration, grabPushForce);
            Vector2 weaponPosition = transform.position; // Replace 'fistsWeapon' with the actual reference to your FistsWeapon object
            enemy.TakePush(weaponPosition, grabPushForce, grabDuration);

        }
        if (heldObject != null && heldObject.CompareTag("ThrowableObject"))
        {
            ThrowableObject spawned = heldObject.GetComponent<ThrowableObject>();
            EndGrab();


            Vector3 positionIn = transform.position;

            spawned.Throw(positionIn);

        }
    }


    public void ReleaseObject()
    {
        if (heldObject != null)
        {
            heldObject.transform.parent = null;
            heldObject = null;
        }
        isHoldingSomething = false;
    }
}
