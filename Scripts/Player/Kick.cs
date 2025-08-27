using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour
{
    [Header("--- Stats ---")]
    [SerializeField] private float stunDuration;
    [SerializeField] private float kickStrength;

    [Header("--- Audio ---")]
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private List<AudioClip> hitSounds;


    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Vector2 weaponPosition = transform.position;
            Enemy enemy = collision.collider.GetComponent<Enemy>();
            enemy.TakeStun(3f, 0f);
            enemy.TakePush(weaponPosition, kickStrength, stunDuration);
            enemy.wasKicked = true;
            hitSound.clip = hitSounds[Random.Range(0, hitSounds.Count)];
            hitSound.Play();
        }
    }
}
