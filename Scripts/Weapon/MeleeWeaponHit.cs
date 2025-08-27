using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponHit : MonoBehaviour
{
    public ParticleSystem meleeCoverHit;
    private BoxCollider2D collider;

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("MeleeWeapon"))
        {
            if (Time.timeScale == 1.0f)
            {
                Time.timeScale = 0.5f;
                Invoke("ResetTime", 0.25f);
            }
            Vector3 collisionPosition = collision.GetContact(0).point;
            Quaternion collisionRotation = Quaternion.Euler(0, 0, collision.transform.eulerAngles.z);

            GameObject room = GameObject.FindWithTag("MeleeWeaponRoom");

            if (room != null) {
                room.GetComponent<MeleeWeaponsRoom>().AddMeleeCounter();
            }
            Instantiate(meleeCoverHit, collisionPosition, collisionRotation);
        }
    }

    private void ResetTime() {
        Time.timeScale = 1.0f;  
    }

}
