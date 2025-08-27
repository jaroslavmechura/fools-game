using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GranadeWeapon : Weapon
{
    [Header("--- Granade Settings ---")]
    [SerializeField] private bool isCharging;
    [SerializeField] private GameObject granadePrefab;
    [SerializeField] private Transform granadeParent;

    [SerializeField] private GameObject granadeObject;
    [SerializeField] private Granade granade;

    [Header("--- References ---")]
    [SerializeField] private PlayerInventory playerInventory;

    private void Start()
    {
        isCharging = false;
        cameraScript = Camera.main.gameObject.GetComponent<CameraFollow>();
        playerInventory = GetComponentInParent<PlayerInventory>();

        PrepGranade();

    }

    void Update()
    {
        if (granadeObject != null)
        {
            // Update the grenade object position to match the hand position
            granadeObject.transform.position = granadeParent.transform.position;
        }
    }

    public override void WeaponInputActions()
    {
        if (granadeObject == null ) {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isWallClip) ThrowPrep();
        }
        else if (Input.GetMouseButtonUp(0) && isCharging) 
        {
            if (!isWallClip) { ThrowGranade(); } else { DropGranade(); }
        }
    }

    private void ThrowPrep() 
    {
        animator.SetTrigger("Charge");

        granade.RemovePin();

        isCharging = true;
    }

    private void ThrowGranade()
    {
        animator.SetTrigger("Throw");

        granadeObject = null;
        isCharging = false;


        granade.Throw();

        playerInventory.fragCarried--;
        if (playerInventory.fragCarried > 0)
        {
            Invoke("PrepGranade", 1f);
        }
        else 
        {
            Invoke("BackToWeapons", 1f);
        }

        
    }
    private void DropGranade()
    {
        animator.SetTrigger("Throw");

        granadeObject = null;
        isCharging = false;


        granade.ThrowThrow(true);

        playerInventory.fragCarried--;
        if (playerInventory.fragCarried > 0)
        {
            Invoke("PrepGranade", 1f);
        }
        else
        {
            Invoke("BackToWeapons", 1f);
        }


    }

    public void PrepGranade() {
        

        granadeObject = Instantiate(granadePrefab, granadeParent);
        granade = granadeObject.GetComponent<Granade>();
    }

    public void BackToWeapons() {
        playerInventory.EquipWeapon((int)WeaponSlot.Melee);
        Destroy(gameObject);
    }

}

