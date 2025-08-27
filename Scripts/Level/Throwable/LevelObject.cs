using UnityEngine;

public class LevelObject : MonoBehaviour
{
    [Header("--- Stats ---")]
    public float health;

    [Header("--- Visuals ---")]
    [SerializeField] private GameObject shardsParticles;

    [Header("--- Attributes ---")]
    [SerializeField] private bool isDoor = false;


    public void TakeDamage(float damage)
    {
        MaterialHit();

        if (isDoor && gameObject.GetComponent<Door>().isLocked) return;

        health -= damage;


        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void MaterialHit()
    {
        GameObject shards = Instantiate(shardsParticles, transform.position, Quaternion.identity);
        shards.GetComponent<EffectDestructor>().RotateSelf();
    }
}
