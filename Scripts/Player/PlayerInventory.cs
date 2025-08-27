using System.Collections.Generic;
using UnityEngine;


public class PlayerInventory : MonoBehaviour
{
    [Header("--- Stats ---")]
    [SerializeField] private float cameraDistanceAdditional = 0f;

    [Header("--- Prefabs ---")]
    [SerializeField] private List<GameObject> weaponsPrefabs;
    [SerializeField] private List<GameObject> pickupsPrefabs;
    public List<GameObject> throwWeaponsPrefabs;


    [Header("--- References ---")]
    public Transform weaponSpawnPoint;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private UIWeaponSlotColor weaponSlotColor;

    [Header("--- Attributes ---")]
    public List<GameObject> weapons;
    public List<bool> haveWeapon;
    public int equipedId;
    [SerializeField] private int weaponsSlot;
    public bool haveFists = true;

    [Header("--- Granades ---")]
    [SerializeField] private GameObject granadeWeaponObject;
    public int fragCarried;

    [Header("--- Audio ---")]
    [SerializeField] private AudioSource pickUpSound;


    [Header("--- Specials ---")]
    [SerializeField] private bool isForTrailer;
    [SerializeField] private float cameraDistanceForTrailer;

    void Start()
    {
        //if (GetComponent<PlayerControler>().isInPlayerRoom) return;

        playerCamera = Camera.main;
        playerInput = GetComponent<PlayerInput>();


        weapons[0] = Instantiate(weaponsPrefabs[0], weaponSpawnPoint);
        foreach (GameObject weapon in weapons)
        {
            if (weapon != null)
            {
                weapon.SetActive(false);
            }

        }

        weapons[0].SetActive(true);

        playerInput.currWeapon = weapons[0].GetComponent<Weapon>();
        playerInput.currWeapon.GetComponent<Weapon>().isReloading = false;

        equipedId = 0;
    }


    void Update()
    {
        if (GetComponent<PlayerControler>().isInPlayerRoom) return;
        ZoomCameraIn();
    }


    void ZoomCameraIn()
    {
        if (isForTrailer)
        {
            playerCamera.orthographicSize = cameraDistanceForTrailer;
            return;
        }

        if (playerInput.currWeapon == null) return;
        Weapon cache = playerInput.currWeapon.GetComponent<Weapon>();
        if (cache != null)
        {
            playerCamera.orthographicSize = Mathf.Lerp(playerCamera.orthographicSize, cache.cameraWiew + cameraDistanceAdditional, Time.deltaTime * 2f);
        }
        else
        {
            playerCamera.orthographicSize = Mathf.Lerp(playerCamera.orthographicSize, cameraDistanceAdditional, Time.deltaTime* 2f);
        }
    }


    public void EquipWeapon(int groupId)
    {

        if (haveWeapon[groupId])
        {
            foreach (GameObject weapon in weapons)
            {
                if (weapon != null)
                {
                    weapon.SetActive(false);
                }
            }

            if (groupId == (int)WeaponSlot.Granade && weapons[groupId] == null) 
            {
                weapons[groupId] = Instantiate(weaponsPrefabs[(int)WeaponID.Granade], weaponSpawnPoint);
               
                
            }

            weapons[groupId].SetActive(true);
            /*
            GameObject tempWeap = weapons[groupId];
            Weapon tempWeapScript = tempWeap.GetComponent<Weapon>();
            weapons[groupId] = Instantiate(weaponsPrefabs[(int)weapons[groupId].GetComponent<Weapon>().weaponId], weaponSpawnPoint);
            weapons[groupId].GetComponent<Weapon>().magCurrent = tempWeapScript.magCurrent;
            weapons[groupId].GetComponent<Weapon>().ammoCarried = tempWeapScript.ammoCarried;

            Destroy(tempWeap);*/

            if (playerInput == null) playerInput = GetComponent<PlayerInput>();
            playerInput.currWeapon = weapons[groupId].GetComponent<Weapon>();
            playerInput.currWeapon.GetComponent<Weapon>().isReloading = false;

            equipedId = groupId;

            if ((int)weapons[equipedId].GetComponent<Weapon>().weaponId == 9 || isForTrailer)
            {
                Cursor.visible = false;
            }
            else
            {
                Cursor.visible = true;
            }
            playerInput.currWeapon.MuzzleFlashEnd();

            if (playerInput.currWeapon.weaponId != WeaponID.Fists)
            {
                GetComponent<HandSkinManger>().InitHandsParts(playerInput.currWeapon);
                GetComponent<HandSkinManger>().SetHandsParts(GetComponent<PlayerSkin>().torsoId);
            }
            else
            {
                playerInput.currWeapon.GetComponent<FistSkin>().SetHands(GetComponent<PlayerSkin>().torsoId);
            }

            if (weaponSlotColor != null) 
            {
                weaponSlotColor.ChangeSlot(groupId);
            }
                

            if (isForTrailer)
            {
                playerCamera.GetComponent<CameraFollow>().maxOffsetDistance = 7.5f;
            }
            else {
                playerCamera.GetComponent<CameraFollow>().maxOffsetDistance = playerInput.currWeapon.cameraWiew * 1.6f;
            }

            
        }
    }

    public void ThrowWeapon() {
        haveWeapon[equipedId] = false;
        weapons[equipedId].GetComponent<Weapon>().DestroyOnThrow();
        weapons[equipedId] = null;

        if (equipedId == 0 && haveFists == false)
        {
            haveFists = true;
            weapons[0] = Instantiate(weaponsPrefabs[0], weaponSpawnPoint);
            playerInput.currWeapon = weapons[0].GetComponent<Weapon>();
            equipedId = 0;
            haveWeapon[equipedId] = true;

            playerInput.currWeapon.GetComponent<FistsWeapon>().nextGrabTime += 0.5f;

        }
    }

    public int PickUpWeapon(int weaponId, int groupId, int ammo, int inMag) {
        if (haveWeapon[groupId])
        {
            if (groupId == 0 && haveFists) {
                pickUpSound.Play();
                Destroy(weapons[groupId]);
                weapons[groupId] = Instantiate(weaponsPrefabs[weaponId], weaponSpawnPoint);
                weapons[groupId].SetActive(false);
                haveFists = false;
                if (equipedId == 0) {
                    EquipWeapon(groupId);
                }

                return 1;
            }
            else if ((int)weapons[groupId].GetComponent<Weapon>().weaponId == weaponId && groupId != 0)
            {
                weapons[groupId].GetComponent<Weapon>().ammoCarried += ammo;
                return 1;
            }
            /*else if (Input.GetKeyUp(KeyCode.T))
            {
                GameObject pickup = Instantiate(pickupsPrefabs[(int)weapons[groupId].GetComponent<Weapon>().weaponId], transform.position, Quaternion.identity);
                pickUpSound.Play();
                pickup.transform.parent = null;
                pickup.GetComponent<WeaponPickUp>().isDropped = true;
                pickup.GetComponent<WeaponPickUp>().inMag = weapons[groupId].GetComponent<Weapon>().magCurrent;
                pickup.GetComponent<WeaponPickUp>().ammo = weapons[groupId].GetComponent<Weapon>().ammoCarried;

                Destroy(weapons[groupId]);

                weapons[groupId] = Instantiate(weaponsPrefabs[weaponId], weaponSpawnPoint);
                weapons[groupId].GetComponent<Weapon>().ammoCarried = ammo;
                weapons[groupId].GetComponent<Weapon>().magCurrent = inMag;

                EquipWeapon(groupId);
                return 1;
            }*/
            return 0;

        } 
        else {
            haveWeapon[groupId] = true;
            weapons[groupId] = Instantiate(weaponsPrefabs[weaponId], weaponSpawnPoint);
            pickUpSound.Play();
            weapons[groupId].GetComponent<Weapon>().ammoCarried = ammo;
            weapons[groupId].GetComponent<Weapon>().magCurrent = weapons[groupId].GetComponent<Weapon>().magCapacity;
            weapons[groupId].SetActive(false);
            return 1;
        }
    }
    public int PickUpWeapon(int weaponId, int groupId)
    {
        if (haveWeapon[groupId])
        {
            if (groupId == 0)
            {
                pickUpSound.Play();
                Destroy(weapons[groupId]);
                weapons[groupId] = Instantiate(weaponsPrefabs[weaponId], weaponSpawnPoint);
                haveFists = false;
                EquipWeapon(groupId);
                return 1;
            }
        }
        return 0;
    }

    public int LoadWeapon(int weaponId, int groupId, int ammo, int inMag)
    {

            haveWeapon[groupId] = true;
            weapons[groupId] = Instantiate(weaponsPrefabs[weaponId], weaponSpawnPoint);
            pickUpSound.Play();
            weapons[groupId].GetComponent<Weapon>().ammoCarried = ammo;
            weapons[groupId].GetComponent<Weapon>().magCurrent = weapons[groupId].GetComponent<Weapon>().magCapacity;
            weapons[groupId].SetActive(false);

            

            return 1;
        
    }
}
