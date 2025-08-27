using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseManager : MonoBehaviour
{
    [Header("--- DamageTracker ---")]
    [SerializeField] private int heavyDamage;
    [SerializeField] private int lightDamage;
    [SerializeField] private int slashDamage;
    [SerializeField] private int bluntDamage;

    [SerializeField] private int limbDamage;

    [SerializeField] private int total;

    public void CountDamage(DamageType damageType) {
        if (damageType == DamageType.Heavy)
        {
            heavyDamage++;
            if (Random.Range(0, 100) < 90) {
                limbDamage++;
            }
           
        }
        else if (damageType == DamageType.Light)
        {
            lightDamage++;
        }
        else if (damageType == DamageType.Slash) {
            slashDamage++;
            limbDamage++;
        }
        total++;
    }

    public int GetHeavyCount() { return heavyDamage; }
    public int GetLightCount() { return lightDamage; }
    public int GetSlashCount() { return slashDamage; }
    public int GetLimbChance() {
        if (total == 0) return 100;


        return (int)((limbDamage / total) * 100); 
    }

}
