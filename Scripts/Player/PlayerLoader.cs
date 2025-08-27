using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLoader : MonoBehaviour
{
    [Header("--- Attributes ---")]
    [SerializeField]private bool isFirstLevel;

    [Header("--- References ---")]
    [SerializeField] private PlayerControler playerController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerSkin playerSkin;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null) Debug.LogError("playerInput not set.");
        playerInventory = GetComponent<PlayerInventory>();
        if (playerInventory == null) Debug.LogError("playerInventory not set.");
        playerController = GetComponent<PlayerControler>();
        if (playerController == null) Debug.LogError("playerController not set.");
        playerSkin = GetComponent<PlayerSkin>();
        if (playerSkin == null) Debug.LogError("playerSkin not set.");

        if (!isFirstLevel) LoadPlayerState();
        
    }


    public void SavePlayerState()
    {
        // PlayerControler script data
        PlayerPrefs.SetFloat("currHealth", playerController.health);
        PlayerPrefs.SetFloat("maxHealth", playerController.maxHealth);

        // PlayerInventory script data
        PlayerPrefs.SetInt("weaponCurrentSlot", playerInventory.equipedId);

        // skin
        PlayerPrefs.SetInt("playerHeadIndex", playerSkin.headId);
        PlayerPrefs.SetInt("playerTorsoIndex", playerSkin.torsoId);
        PlayerPrefs.SetInt("playerShoeIndex", playerSkin.shoeId);

        //meleeSlot
        if (!playerInventory.haveFists)
        {
            PlayerPrefs.SetInt("weaponMeleeSlotHaveWeapon", 1);
            PlayerPrefs.SetInt("weaponMeleeSlotId", (int)playerInventory.weapons[0].GetComponent<Weapon>().weaponId);
        }
        else
        {
            PlayerPrefs.SetInt("weaponMeleeSlotHaveWeapon", 0);
        }
        //pistolSlot
        if (playerInventory.haveWeapon[1])
        {
            PlayerPrefs.SetInt("weaponPistolSlotHaveWeapon", 1);
            PlayerPrefs.SetInt("weaponPistolSlotId", (int)playerInventory.weapons[1].GetComponent<Weapon>().weaponId);
            PlayerPrefs.SetInt("weaponPistolSlotInMag", playerInventory.weapons[1].GetComponent<Weapon>().magCurrent);
            PlayerPrefs.SetInt("weaponPistolSlotAmmoCarried", playerInventory.weapons[1].GetComponent<Weapon>().ammoCarried);
        }
        else {
            PlayerPrefs.SetInt("weaponPistolSlotHaveWeapon", 0);
        }
        //rifleSlot
        if (playerInventory.haveWeapon[2])
        {
            PlayerPrefs.SetInt("weaponRifleSlotHaveWeapon", 1);
            Debug.Log(PlayerPrefs.GetInt("weaponRifleSlotHaveWeapon"));
            PlayerPrefs.SetInt("weaponRifleSlotId",  (int)playerInventory.weapons[2].GetComponent<Weapon>().weaponId);
            PlayerPrefs.SetInt("weaponRifleSlotInMag", playerInventory.weapons[2].GetComponent<Weapon>().magCurrent);
            PlayerPrefs.SetInt("weaponRifleSlotAmmoCarried", playerInventory.weapons[2].GetComponent<Weapon>().ammoCarried);
        }
        else {
            PlayerPrefs.SetInt("weaponRifleSlotHaveWeapon", 0);
        }

        // Grenades
        PlayerPrefs.SetInt("fragGrenadesCount", playerInventory.fragCarried);

        Debug.Log("dataSaved");
    }


    public void LoadPlayerState()
    {
        if (PlayerPrefs.HasKey("playerHeadIndex"))
        {
            playerSkin.headId = PlayerPrefs.GetInt("playerHeadIndex");
            //print("mam");
        }
        else {
            playerSkin.headId = 2;
            //print("nemam");
        }

        if (PlayerPrefs.HasKey("playerTorsoIndex"))
        {
            playerSkin.torsoId = PlayerPrefs.GetInt("playerTorsoIndex");
            //print("mam");
        }
        else
        {
            playerSkin.torsoId = 0;
            //print("nemam");
        }

        if (PlayerPrefs.HasKey("playerShoeIndex"))
        {
            playerSkin.shoeId = PlayerPrefs.GetInt("playerShoeIndex");
            //print("mam");
        }
        else
        {
            playerSkin.shoeId = 1;
            //print("nemam");
        }

        playerSkin.SetBodyParts();


        if (SceneManager.GetActiveScene().name == "Tutorial1" || SceneManager.GetActiveScene().name == "Club1" || SceneManager.GetActiveScene().name == "PlayerRoom" || SceneManager.GetActiveScene().name == "PlayerRoom1" || SceneManager.GetActiveScene().name == "PlayerRoom2" || SceneManager.GetActiveScene().name == "Trade2" || SceneManager.GetActiveScene().name == "Farm2") return;

        // PlayerControler script data
        playerController.health = PlayerPrefs.GetFloat("maxHealth");
        playerController.maxHealth = PlayerPrefs.GetFloat("maxHealth");

        // PlayerInventory script data
        // equipid owned weapons
        if (PlayerPrefs.GetInt("weaponMeleeSlotHaveWeapon") == 1)
        {
            //Debug.Log("load Pickup melee");
            Invoke("CallMelee", 0.25f);
           
        }
        if (PlayerPrefs.GetInt("weaponPistolSlotHaveWeapon") == 1) {
            //Debug.Log("load Pickup pistol");
            playerInventory.LoadWeapon(PlayerPrefs.GetInt("weaponPistolSlotId"), 1, PlayerPrefs.GetInt("weaponPistolSlotAmmoCarried"), PlayerPrefs.GetInt("weaponPistolSlotInMag"));
        }
        //Debug.Log("from Load" + PlayerPrefs.GetInt("weaponRifleSlotHaveWeapon").ToString());
        if(PlayerPrefs.GetInt("weaponRifleSlotHaveWeapon") == 1)
        {
            //Debug.Log("load Pickup rifle");
            playerInventory.LoadWeapon(PlayerPrefs.GetInt("weaponRifleSlotId"), 2, PlayerPrefs.GetInt("weaponRifleSlotAmmoCarried"), PlayerPrefs.GetInt("weaponRifleSlotInMag"));
        }
        Invoke("Equip", 0.05f);

        // Grenades
        playerInventory.fragCarried = PlayerPrefs.GetInt("fragGrenadesCount");

        //Debug.Log("dataLoaded");
    }

    private void Equip() {
        playerInventory.EquipWeapon(0);
    }

    private void CallMelee() {
        playerInventory.PickUpWeapon(PlayerPrefs.GetInt("weaponMeleeSlotId"), 0);
    }
}
