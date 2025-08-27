using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScriptElevator : MonoBehaviour
{
    [Header("--- References ---")]
    [SerializeField] private List<Light2D> lightToTurnRed;
    [SerializeField] private List<Door> doorsToUnlock;
    [SerializeField] private List<Enemy> enemiesToActivate;
    [SerializeField] private AudioSource alarmTogle;
    [SerializeField] private GameObject oldNextLevel;
    private Transform player;
    public GameObject text;

    [Header("--- Interactions ---")]
    public KeyCode interactKey = KeyCode.V;
    public float interactionDistance = 5f;

    private bool activated = false;

    

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        if (IsPlayerInRange() && Input.GetKeyDown(interactKey) && !activated)
        {
            activated = true;
            Actions();
        }
    }

    private void Actions()
    {
        foreach (Light2D light in lightToTurnRed) {
            light.color = Color.red;
            light.intensity += 3f;
        }

        foreach (Door door in doorsToUnlock) {
            door.isLocked = false;
            door.ToggleDoor();
        }

        foreach(Enemy enemy in enemiesToActivate)
        {
            enemy.isDeactive = false;
            enemy.isChasingPlayer = true;
            enemy.chaseLetDownTimer = enemy.chaseLetDownLength;

        }

        alarmTogle.Play();

        oldNextLevel.SetActive(false);
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
        {
            text.SetActive(false);
            return false;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        text.SetActive(distance <= interactionDistance);

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