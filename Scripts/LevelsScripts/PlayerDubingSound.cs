using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDubingSound : MonoBehaviour
{

    [SerializeField] private AudioSource triggerAudio;

    [SerializeField] private bool played = false;

    public bool isOneTimer = true;

    public string eventName;

    private void Start()
    {
        if (isOneTimer && PlayerPrefs.HasKey(eventName)) {
            if (PlayerPrefs.GetInt(eventName) == 1) {
                Destroy(gameObject);
            }
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (played) return;

        if (collision.CompareTag("Player"))
        {
            triggerAudio.Play();
            played = true;

            if (isOneTimer) {
                PlayerPrefs.SetInt(eventName, 1);
            }
        }
    }
}
