using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [Header("--- Interaction ---")]
    public KeyCode interactKey = KeyCode.V;
    public float interactionDistance = 2f;

    [Header("--- Opening/Closing ---")]
    public float openAngle = 90f;

    private Transform doorTransform;
    private Quaternion initialRotation;
    private Quaternion openRotation;

    [Header("--- States ---")]
    public bool isOpened = false;
    public bool isLocked = false;

    private bool isOpening = false;

    [Header("--- Audio ---")]
    [SerializeField] AudioSource openSound;
    [SerializeField] AudioSource kickSlamSound;
    [SerializeField] AudioSource lockedSound;

    [Header("--- AiNavigation ---")]
    private NavMeshObstacle navMeshObstacle;

    [Header("--- Referneces ---")]
    private Transform player;
    private Rigidbody2D rb;
    
    public BoxCollider2D colCol;

    public GameObject splash;

    public float kickForce;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        doorTransform = transform.parent;
        initialRotation = doorTransform.rotation;
        openRotation = initialRotation * Quaternion.Euler(0, 0, openAngle);
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();

        navMeshObstacle = GetComponent<NavMeshObstacle>();

        if (!isOpened)
        {
            navMeshObstacle.enabled = false;
        }
        else {
            navMeshObstacle.enabled = true;
        }
    }

    void Update()
    {
        if (IsPlayerInRange() && Input.GetKeyDown(interactKey) && isLocked) {lockedSound.Play(); StartCoroutine(MoveDoorSlightly()); return; }

        if (IsPlayerInRange() && Input.GetKeyDown(interactKey))
        {
            openSound.Play();
            ToggleDoor();
        }

        if (isOpening)
        {
            doorTransform.rotation = Quaternion.Lerp(doorTransform.rotation, openRotation, Time.deltaTime * 5f);
        }
        else
        {
            doorTransform.rotation = Quaternion.Lerp(doorTransform.rotation, initialRotation, Time.deltaTime * 5f);
        }
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
        {
            return false;
        }

        float distance = Vector3.Distance(doorTransform.position, player.position);
        return distance <= interactionDistance;
    }

    public void ToggleDoor()
    {
        if (isLocked)
        {
            StartCoroutine(MoveDoorSlightly());
        }
        else
        {
            isOpened = !isOpened;
            isOpening = !isOpening;

            navMeshObstacle.enabled = !navMeshObstacle.enabled;
        }
    }

    private IEnumerator MoveDoorSlightly()
    {
        float elapsedTime = 0f;
        float duration = 0.25f;
        Quaternion targetRotation;
        if (doorTransform.rotation == initialRotation)
        {
             targetRotation = doorTransform.rotation * Quaternion.Euler(0, 0, 5f);
        }
        else {
            targetRotation = doorTransform.rotation;
        }
        

        while (elapsedTime < duration)
        {
            doorTransform.rotation = Quaternion.Lerp(doorTransform.rotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        doorTransform.rotation = initialRotation;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("KickLeg") && !isOpening && !isOpened)
        {
            
            GetKicked();
        }
    }

    private void GetKicked() {
        if (isLocked) { StartCoroutine(MoveDoorSlightly()); return; }

        colCol.enabled = false;
     

       

        
        GameObject shards = Instantiate(splash, transform.position, Quaternion.identity);
        shards.GetComponent<EffectDestructor>().RotateSelf();
        

        Destroy(gameObject);
    }

    public void PatrolClose() {
        Invoke("ToggleDoor", 5f);
    }
}
