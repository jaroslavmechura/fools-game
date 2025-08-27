using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBox : MonoBehaviour
{
    [Header("--- Interaction ---")]
    [SerializeField] public KeyCode interactKey = KeyCode.Space;

    [SerializeField] private GameObject doorObject;
    [SerializeField] private bool isUsed;
    [SerializeField] private float healthToGive;
    [SerializeField] private float interactionDistance;
    [SerializeField] private Transform player;

    [Header("--- Audio ---")]
    [SerializeField] private AudioSource audioSource;

    [Header("--- Rotation ---")]
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float rotationSpeed = 5f;


    void Start()
    {
        isUsed = false;

        initialRotation = doorObject.transform.rotation;
        targetRotation = initialRotation;
    }

    void Update()
    {
        if (IsPlayerInRange() && !isUsed && Input.GetKeyDown(interactKey))
        {
            player.GetComponent<PlayerControler>().HealthPickUp(healthToGive);

            audioSource.Play();

            isUsed = true;
            targetRotation = doorObject.transform.rotation * Quaternion.Euler(0, 0, 30f);
        }

        doorObject.transform.rotation = Quaternion.Lerp(doorObject.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= interactionDistance;
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
}
