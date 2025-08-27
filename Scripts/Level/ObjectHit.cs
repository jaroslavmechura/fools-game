using UnityEngine;

public class ObjectHit : MonoBehaviour
{
    [Header("--- Bullet Impact ---")]
    public float damageToBullet;
    public float slowToBullet;
    public float damageReductionToBullet;

    [Header("--- Attributes ---")]
    [SerializeField] private GameObject parent;

    private LevelObject objectCache;


    public void TakeDamage(float dmg) {
        if (parent == null) return;
        objectCache = parent.GetComponent<LevelObject>();
        if (objectCache != null) 
        {
            objectCache.TakeDamage(dmg);
        }
       
    }

}
