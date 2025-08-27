using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
    [Header("--- BodyPartsIDs")]
    public int headId;
    public int torsoId;
    public int shoeId;

    [Header("--- BodyPartsCount")]
    public int headCount = 17;
    public int torsoCount = 4;
    public int shoeCount = 3;

    [Header("--- BodyPartsReferences ---")]
    [SerializeField] private PickBodyPart headScript;
    [SerializeField] private PickBodyPart torsoScript;
    [SerializeField] private PickBodyPart leftShoeScript;
    [SerializeField] private PickBodyPart rightShoeScript;
    [SerializeField] private FistSkin skinManger;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            SetBodyPartsRandomForTrailer();
        }
    }

    public void SetBodyParts() {
        SetPlayerHead();
        SetPlayerTorso();
        SetPlayerShoes();
    }

    public void SetBodyPartsRandomForTrailer()
    {
        headId = Random.Range(0, headCount+1);
        torsoId = Random.Range(0, torsoCount+1);
        shoeId = Random.Range(0, shoeCount+1);

        SetPlayerHead();
        SetPlayerTorso();
        SetPlayerShoes();
    }

    public void SetPlayerHead() {
        headScript.SetHead(headId);
    }

    public void SetPlayerTorso()
    {
        torsoScript.SetTorso(torsoId);

       

        skinManger = gameObject.GetComponentInChildren<FistSkin>();
        if (skinManger != null) 
        {
            skinManger.SetHands(torsoId);
        }

        Invoke("LateSet", 0.5f);

    }

    private void LateSet() {

        GetComponent<PlayerInventory>().EquipWeapon(GetComponent<PlayerInventory>().equipedId);
    }

    public void SetPlayerShoes()
    {
        leftShoeScript.SetFootPart(shoeId);
        rightShoeScript.SetFootPart(shoeId);
    }


    public void SetTorsoIndex(int i) {
        torsoId += i;
        if (torsoId > torsoCount) torsoId = 0;
        else if (torsoId < 0) torsoId = torsoCount;

        SetPlayerTorso();
    }

    public void SetShoeIndex(int i)
    {
        shoeId += i;
        if (shoeId > shoeCount) shoeId = 0;
        if (shoeId < 0) shoeId = shoeCount;
        SetPlayerShoes();
    }
    public void SetHairIndex(int i)
    {
        if (i > 0)
        {
            if (headId == 0) headId = 1;
            else if (headId == 1) headId = 2;
            else if (headId < 8) headId = 8;
            else if (headId < 12) headId = 12;
            else headId = 0;
        }
        else {
            if (headId == 0) headId = 12;
            else if (headId >= 12) headId = 8;
            else if (headId >= 8) headId = 2;
            else if (headId == 2) headId = 1;
            else headId = 0;

        }

        SetPlayerHead();
    }
    public void SetHairColorIndex(int i)
    {
        if (headId == 0 || headId == 1) return;
        if (i > 0)
        {
            if (headId < 8)
            {
                headId++;
                if (headId == 8) { headId = 2; SetPlayerHead(); return; }
            }
            else if (headId < 12)
            {
                headId++;
                if (headId == 12) { headId = 8; SetPlayerHead(); return; }
            }
            else if (headId < 18)
            {
                headId++;
                if (headId == 18) { headId = 12; SetPlayerHead(); return; }
            }
        }
        else {
            if (headId < 8)
            {
                headId--;
                if (headId == 1) { headId = 7; SetPlayerHead(); return; }
            }
            else if (headId < 12)
            {
                headId--;
                if (headId == 7) { headId =11; SetPlayerHead(); return; }
            }
            else if (headId < 18)
            {
                headId--;
                if (headId == 11) { headId = 17; SetPlayerHead(); return; }
            }
        }
        SetPlayerHead();

    }
}
