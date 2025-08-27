using System.Collections.Generic;
using UnityEngine;

public class Fist : MonoBehaviour
{
    [Header("--- Stats ---")]
    public float damage;

    [SerializeField] private float punchSpeed;
    [SerializeField] private float duration;


    [Header("--- Attributes ---")]
    public bool isPunching = false;

    [SerializeField] private Vector3 originalPosition;

    [Header("--- Audio ---")]
    public AudioSource hitSound;

    [SerializeField] private List<AudioClip> hitSounds;


    private void Start()
    {
        originalPosition = transform.localPosition;
    }


    void Update()
    {
        if (isPunching)
        {
            transform.Translate(Vector3.up * punchSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && isPunching)
        {
            float punchRotationX = gameObject.transform.localRotation.x;
            float punchRotationY = gameObject.transform.localRotation.y;

            Vector2 punchDirection = new Vector2(punchRotationX, punchRotationY);

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(damage, 0f, transform, false, DamageType.Blunt);
            enemy.TakePush(punchDirection, punchSpeed/4, duration/5);
            enemy.wasPunched = true;

            hitSound.clip = hitSounds[Random.Range(0, hitSounds.Count)];
            hitSound.Play();
        }
    }


    public void Punch()
    {
        transform.Translate(Vector2.down * 0.75f);
 
        isPunching = true;
        Invoke("EndPunch", duration);
    }


    void EndPunch()
    {
        isPunching = false;
        transform.localPosition = originalPosition;
    }
}
