using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WeaponPickUp : MonoBehaviour
{
    [Header("--- Stats ---")]
    [SerializeField] private WeaponID weaponId;
    [SerializeField] private WeaponSlot slotId;
    public int ammo;
    public int inMag;
    [SerializeField] private int capacity;

    [Header("--- Scaling ---")]
    [SerializeField] private float scaleSpeed = 1.0f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float minScale = 0.8f;
    private Vector3 originalScale;
    private float timeCounter = 0.0f;
    private float scaleFactor;

    [Header("--- PickUpPoint ---")]
    [SerializeField] private bool isRespawn = false;
    [SerializeField] private float respawnTimer;

    [Header("--- References ---")]
    [SerializeField] private Collider2D pickUpCollider;

    public bool prepAmmo =false;
    public bool isDropped = false;

    [Header("--- Light ---")]
    public float maxIntensity;
    public float maxRangeFromPlayer;

    private Light2D pickupLight;
    private Transform playerT;

    void Start()
    {
        originalScale = transform.localScale;
        pickUpCollider = GetComponent<Collider2D>();
        if (pickUpCollider == null) print("prti ti smrdel");

        if (prepAmmo)
        {
            ammo = Random.Range(capacity, capacity * 2);
            inMag = capacity;
        }
        else if (isDropped) {
            ammo = ammo;
            inMag = inMag;
        }
        else
        {
            ammo = Random.Range(capacity/2, capacity + 1);
            inMag = Random.Range(1, capacity + 1);
        }
     
        pickupLight = transform.Find("Light 2D").GetComponent<Light2D>();
        maxIntensity = pickupLight.intensity * 1.1f;

        playerT = GameObject.FindWithTag("Player").transform;
        
    }


    void Update()
    {
        timeCounter += Time.deltaTime * scaleSpeed/2;
        scaleFactor = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(timeCounter) + 1) / 2);

        transform.localScale = originalScale * scaleFactor;

        float distanceFromPlayer = Vector3.Distance(transform.position, playerT.position);

        // Upravit intenzitu svìtla na základì vzdálenosti
        if (distanceFromPlayer > maxRangeFromPlayer)
        {
            pickupLight.intensity = 0f;
        }
        else
        {
            float t = 1 - (distanceFromPlayer / maxRangeFromPlayer);
            pickupLight.intensity = Mathf.Lerp(0f, maxIntensity, t);
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {

            if (weaponId == WeaponID.Granade)
            {
                collision.gameObject.GetComponent<PlayerInventory>().fragCarried += 1;
                if (isRespawn)
                {
                    StartCoroutine(RespawnIn());
                }
                else
                {
                    pickUpCollider.enabled = false;
                    Destroy(gameObject, 0.1f);
                }
                return;
            }

            if (collision.gameObject.GetComponent<PlayerInventory>().PickUpWeapon((int)weaponId, (int)slotId, ammo, inMag) == 1) {
                if (isRespawn) {
                    StartCoroutine(RespawnIn());
                }
                else {
                    pickUpCollider.enabled = false;
                    Destroy(gameObject, 0.1f); 
                }
               
            }

        }
    }

    private IEnumerator RespawnIn() {
        TurnSelf(false);

        yield return new WaitForSeconds(respawnTimer);


        if (prepAmmo)
        {
            ammo = Random.Range(capacity, capacity * 2);
            inMag = capacity;
        }
        else
        {
            ammo = Random.Range(1, capacity + 1);
            inMag = Random.Range(1, capacity + 1);
        }

        TurnSelf(true);

    }

    private void TurnSelf(bool turn) {
        foreach (Transform child in gameObject.transform) { 
            child.gameObject.SetActive(turn);
        }

        pickUpCollider.enabled = turn;
    }
}
