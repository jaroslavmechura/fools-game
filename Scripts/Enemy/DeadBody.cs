using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    [SerializeField] private int headTest;
    [SerializeField] private int torsoTest;

    [Header("--- Body Parts ---")]
    [SerializeField] private GameObject torso;
    [SerializeField] private GameObject lArm;
    [SerializeField] private GameObject rArm;
    [SerializeField] private GameObject lLeg;
    [SerializeField] private GameObject rLeg;
    [SerializeField] private GameObject head;

    [Header("--- Sprites ---")]
    [SerializeField] private List<Sprite> torsoSprite;

    [SerializeField] private List<Sprite> greenLArmSprite;
    [SerializeField] private List<Sprite> brownLArmSprite;
    [SerializeField] private List<Sprite> suitLArmSprite;
    [SerializeField] private List<Sprite> topLArmSprite;
    [SerializeField] private List<Sprite> purpleLArmSprite;
    [SerializeField] private List<float> xLArmPos;
    [SerializeField] private List<float> yLArmPos;

    [SerializeField] private List<Sprite> greenRArmSprite;
    [SerializeField] private List<Sprite> brownRArmSprite;
    [SerializeField] private List<Sprite> suitRArmSprite;
    [SerializeField] private List<Sprite> topRArmSprite;
    [SerializeField] private List<Sprite> purpleRArmSprite;
    [SerializeField] private List<float> xRArmPos;
    [SerializeField] private List<float> yRArmPos;

    [SerializeField] private List<Sprite> legSprite;
    [SerializeField] private List<float> xLLegPos;
    [SerializeField] private List<float> yLLegPos;
    [SerializeField] private List<float> xRLegPos;
    [SerializeField] private List<float> yRLegPos;

    [SerializeField] private List<Sprite> headSprites;
    [SerializeField] private List<Sprite> headHalfSprites;
    [SerializeField] private List<float> yHeadPos;

    [Header("--- Bullet Holes ---")]
    [SerializeField] private List<GameObject> headBulletHoles;
    [SerializeField] private List<GameObject> torsoBulletHoles;
    [SerializeField] private List<GameObject> lLegBulletHoles;
    [SerializeField] private List<GameObject> rLegBulletHoles;
    [SerializeField] private GameObject bigBulletHole;

    [SerializeField] private List<Sprite> bulletHolesSprites;
    [SerializeField] private List<Sprite> bigBulletHolesSprites;

    [SerializeField] public int totalBulletHoles;

    [Header("--- Bools ---")]
    [SerializeField] private bool isSpecialHeads;

    [Header("--- BulletHolesSpecifications ---")]
    [SerializeField] private int heavyDamage;
    [SerializeField] private int lightDamage;
    [SerializeField] private int limbChance;
    

    public void SetDamageCounts(int heavy, int light, int limb) {
        heavyDamage = heavy;
        lightDamage = light;
        limbChance = limb;

        totalBulletHoles = lightDamage + (heavyDamage/2);

        print(totalBulletHoles);
    }

    void InitHead(int headID)
    {
        if (Random.Range(0, 101) < limbChance)
        {
            int random = Random.Range(0, headHalfSprites.Count);
            head.GetComponent<SpriteRenderer>().sprite = headHalfSprites[random];
            head.transform.localPosition = new Vector2(0f, yHeadPos[random]); 
            isSpecialHeads = true;
        }
        else
        {
            head.GetComponent<SpriteRenderer>().sprite = headSprites[headID];
            head.transform.localPosition = new Vector2(0f, 2.45f);
            isSpecialHeads = false;
        }
    }

    void InitTorso(int torsoID)
    {
        torso.GetComponent<SpriteRenderer>().sprite = torsoSprite[torsoID];
        if (torsoID == 4) {
            torso.transform.localPosition = new Vector2(0.15f, 0.7f);
        }
    }
    public void RotateSelf()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            Vector3 toPlayer = player.transform.position - transform.position;

            float targetRotationZ = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0.0f, 0.0f, targetRotationZ + 90f);
        }
    }

    void InitHands(int handID)
    {
        int chance = 1;

        if (Random.Range(0, 101) < limbChance)
        {
            chance = 0;
        }

        if (isSpecialHeads) chance = 1;

        int type = Random.Range(0, greenLArmSprite.Count - chance);
        lArm.GetComponent<SpriteRenderer>().sprite = GetLeftArmSprite(handID, type);
        lArm.transform.localPosition = new Vector2(xLArmPos[type], yLArmPos[type]);

        type = Random.Range(0, greenLArmSprite.Count - chance);
        rArm.GetComponent<SpriteRenderer>().sprite = GetRightArmSprite(handID, type);
        rArm.transform.localPosition = new Vector2(xRArmPos[type], yRArmPos[type]);
    }

    void InitLegs()
    {
        int chance = 1;

        if (Random.Range(0, 101) < limbChance) {
            chance = 0;
        }

        if (isSpecialHeads) chance = 1;

        int legID = Random.Range(0, legSprite.Count - chance);
        lLeg.GetComponent<SpriteRenderer>().sprite = legSprite[legID];
        lLeg.transform.localPosition = new Vector2(xLLegPos[legID], yLLegPos[legID]);

        legID = Random.Range(0, legSprite.Count - chance);
        rLeg.GetComponent<SpriteRenderer>().sprite = legSprite[legID];
        rLeg.transform.localPosition = new Vector2(xRLegPos[legID], yRLegPos[legID]);
    }

    void InitBulletHoles()
    {
        int random = Random.Range(0, 101);

        if (random < 25) {
            if (!isSpecialHeads)
            {
                SetBulletHoles(headBulletHoles, 1);
            }
            SetBulletHoles(torsoBulletHoles, 2);
            SetBulletHoles(lLegBulletHoles, 1);
            SetBulletHoles(rLegBulletHoles, 1);
        }

        else if (random < 50) {
            if (!isSpecialHeads)
            {
                SetBulletHoles(headBulletHoles, 2);
            }
            SetBulletHoles(torsoBulletHoles, 1);
            SetBulletHoles(lLegBulletHoles, 1);
            SetBulletHoles(rLegBulletHoles, 1);
        }

        else if (random < 75) {
            SetBulletHoles(torsoBulletHoles, 1);
            if (!isSpecialHeads)
            {
                SetBulletHoles(headBulletHoles, 2);
            }
            SetBulletHoles(lLegBulletHoles, 1);
            SetBulletHoles(rLegBulletHoles, 1);
        }

        else {
            SetBulletHoles(lLegBulletHoles, 1);
            SetBulletHoles(torsoBulletHoles, 2);
            SetBulletHoles(rLegBulletHoles, 1);
            if (!isSpecialHeads)
            {
                SetBulletHoles(headBulletHoles, 1);
            }
        }


        if (heavyDamage > 0) {
            bigBulletHole.GetComponent<SpriteRenderer>().sprite = bigBulletHolesSprites[Random.Range(0, bigBulletHolesSprites.Count)];
        }
    }

    void SetBulletHoles(List<GameObject> bulletHoles, int max)
    {
        int maxHolesPerPart = max;
        int holesOnPart = 0;

        for (int i = 0; i < bulletHoles.Count; i++)
        {
            if (holesOnPart >= maxHolesPerPart)
            {
                bulletHoles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            else
            {
                
                if (totalBulletHoles > 0)
                {
                    bulletHoles[i].GetComponent<SpriteRenderer>().sprite = bulletHolesSprites[Random.Range(0, bulletHolesSprites.Count)];
                    totalBulletHoles--;
                    holesOnPart++;
                    print("HoleSpawned" + bulletHoles[i].gameObject.name.ToString());
                }
                else
                {
                    bulletHoles[i].GetComponent<SpriteRenderer>().sprite = null;
                }
            }
        }
    }

    public void InitDeadBody(int torsoID, int headID)
    {
        InitHead(headID);
        InitTorso(torsoID);
        InitHands(torsoID);
        InitLegs();
        InitBulletHoles();
        RotateSelf();

        transform.localScale *= 0.375f;
    }

    private Sprite GetLeftArmSprite(int handID, int type)
    {
        switch (handID)
        {
            case 0: return greenLArmSprite[type];
            case 1: return suitLArmSprite[type];
            case 2: return purpleLArmSprite[type];
            case 3: return topLArmSprite[type];
            case 4: return brownLArmSprite[type];
        }
        return null;
    }

    private Sprite GetRightArmSprite(int handID, int type)
    {
        switch (handID)
        {
            case 0: return greenRArmSprite[type];
            case 1: return suitRArmSprite[type];
            case 2: return purpleRArmSprite[type];
            case 3: return topRArmSprite[type];
            case 4: return brownRArmSprite[type];
        }
        return null;
    }
}
