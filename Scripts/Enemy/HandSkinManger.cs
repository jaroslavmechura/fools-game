using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSkinManger : MonoBehaviour
{
    [Header("Functionality")]
    [SerializeField] private GameObject enemyWeapon;

    [Header("Hands Parts")]
    [SerializeField] private Transform hands;

    [SerializeField] private Transform lu;
    [SerializeField] private Transform lm;
    [SerializeField] private Transform lh;

    [SerializeField] private Transform ru;
    [SerializeField] private Transform rm;
    [SerializeField] private Transform rh;

    [Header("Foot Parts")]
    [SerializeField] private Transform legs;
    [SerializeField] private Transform lf;
    [SerializeField] private Transform rf;

    [Header("--- Boss ---")]
    public bool isBoss = false;
    public int bossFootId;
    


    public void InitHandsParts(Weapon weaponIn) {
        enemyWeapon = weaponIn.gameObject;

        // assing lu, lm, lh, ru, rm, rh. They are childern in enemy Weapon in hirearhy Weapon object -> Hands -> ((LU -> LM -> LH) / (RU->RM->RH))
        hands = enemyWeapon.transform.Find("Hands");

        // left hand
        lu = hands.transform.Find("LU");
        lm = lu.transform.Find("LM");
        lh = lm.transform.Find("LH");

        // right hand
        ru = hands.transform.Find("RU");
        rm = ru.transform.Find("RM");
        rh = rm.transform.Find("RH");

        legs = transform.Find("Legs");
        lf = legs.transform.Find("LU").transform.Find("LM").transform.Find("LF").transform;
        rf = legs.transform.Find("RU").transform.Find("RM").transform.Find("RF").transform;
    }

    public void SetHandsParts(int index) {
        // index je index torsa

        lu.GetComponent<PickBodyPart>().SetArmPart(index);
        lm.GetComponent<PickBodyPart>().SetArmPart(index);
        lh.GetComponent<PickBodyPart>().SetArmPart(index);

        ru.GetComponent<PickBodyPart>().SetArmPart(index);
        rm.GetComponent<PickBodyPart>().SetArmPart(index);
        rh.GetComponent<PickBodyPart>().SetArmPart(index);

        SetFoot(index);
    }

    private void SetFoot(int index) {

        if (isBoss) {
            lf.GetComponent<PickBodyPart>().SetFootPart(bossFootId);
            rf.GetComponent<PickBodyPart>().SetFootPart(bossFootId);
            return;
        }


        int random;
        switch (index)
        {
            
            // green jacket
            case 0:
                random = Random.Range(0, 3);

                if (random == 0) // black shoes
                {
                    lf.GetComponent<PickBodyPart>().SetFootPart(0);
                    rf.GetComponent<PickBodyPart>().SetFootPart(0);
                }
                else if (random == 1)// brown shoes
                {
                    lf.GetComponent<PickBodyPart>().SetFootPart(2);
                    rf.GetComponent<PickBodyPart>().SetFootPart(2);
                }
                else if (random == 2) // leather shoes
                {
                    lf.GetComponent<PickBodyPart>().SetFootPart(3);
                    rf.GetComponent<PickBodyPart>().SetFootPart(3);
                }

                break;
            // suit
            case 1:
                random = Random.Range(0, 2);
                if (random == 0) // black shoes
                {
                    lf.GetComponent<PickBodyPart>().SetFootPart(0);
                    rf.GetComponent<PickBodyPart>().SetFootPart(0);
                }
                else if (random == 1)// brown shoes
                {
                    lf.GetComponent<PickBodyPart>().SetFootPart(2);
                    rf.GetComponent<PickBodyPart>().SetFootPart(2);
                }
                break;
            // Hoodie
            case 2:
                random = Random.Range(0, 2);
                if (random == 0) // black shoes
                {
                    lf.GetComponent<PickBodyPart>().SetFootPart(0);
                    rf.GetComponent<PickBodyPart>().SetFootPart(0);
                }
                else if (random == 1)// red shoes
                {
                    lf.GetComponent<PickBodyPart>().SetFootPart(1);
                    rf.GetComponent<PickBodyPart>().SetFootPart(1);
                }
                break;
            // Top
            case 3:
                random = Random.Range(0, 4);
                lf.GetComponent<PickBodyPart>().SetFootPart(random);
                rf.GetComponent<PickBodyPart>().SetFootPart(random);

                break;
            // LeatherJacket
            case 4:
                random = Random.Range(0, 3);
                if (random == 0) // black shoes
                {
                    lf.GetComponent<PickBodyPart>().SetFootPart(0);
                    rf.GetComponent<PickBodyPart>().SetFootPart(0);
                }
                else if (random == 1)// brown shoes
                {
                    lf.GetComponent<PickBodyPart>().SetFootPart(2);
                    rf.GetComponent<PickBodyPart>().SetFootPart(2);
                }
                else if (random == 2) // leather shoes
                {
                    lf.GetComponent<PickBodyPart>().SetFootPart(3);
                    rf.GetComponent<PickBodyPart>().SetFootPart(3);
                }

                break;
        }
    }

}
