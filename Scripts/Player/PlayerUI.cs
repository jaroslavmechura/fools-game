using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("--- Player Stats ---")]
    public GameObject canvas;
    public float lerpSpeed = 5f;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthSlider2;

    [SerializeField] private Slider dashCooldownSlider;
    [SerializeField] private Slider dashCooldownSlider2;

    [SerializeField] private Slider dodgeCooldownSlider;
    [SerializeField] private Slider dodgeCooldownSlider2;

    [SerializeField] private Slider bulletTimeCooldownSlider;
    [SerializeField] private Slider bulletTimeCooldownSlider2;

    [SerializeField] private Slider kickCooldownSlider;
    [SerializeField] private Slider kickCooldownSlider2;

    [Header("--- Weapons ---")]
    [SerializeField] private GameObject carringText;
    [SerializeField] private List<GameObject> weaponSlots;
    [SerializeField] private List<Sprite> weaponSlotsSprites;

    [SerializeField] private Transform bulletContainer;
    [SerializeField] private Transform bulletContainer1;
    [SerializeField] private Transform bulletContainer2;
    [SerializeField] private List<GameObject> bulletSprites = new List<GameObject>();
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletPrefabSmall;

    [Header("--- Grenates ---")]
    [SerializeField] private GameObject fragCarryText; 

    [Header("--- Level Stats ---")]
    [SerializeField] private SubLevelProgres subLevelProgresScript;

    [Header("--- References ---")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerControler playerController;

    [Header("Colors")]
    [SerializeField] private Color fullMagC;
    [SerializeField] private Color midMagC;
    [SerializeField] private Color emptyMagC;

    private Vector2 originalPos;


    [Header("--- Special ---")]
    [SerializeField] private bool isForTrailer;


    void Start()
    {
        originalPos = bulletContainer.GetComponent<RectTransform>().anchoredPosition;

        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null) Debug.LogError("playerInput not set.");
        playerInventory = GetComponent<PlayerInventory>();
        if (playerInventory == null) Debug.LogError("playerInventory not set.");
        playerController = GetComponent<PlayerControler>();
        if (playerController == null) Debug.LogError("playerController not set.");
        if (playerController.isInPlayerRoom) return;

        healthSlider.maxValue = playerController.maxHealth;
        healthSlider.minValue = 0;
        healthSlider.value = playerController.health;
        healthSlider2.maxValue = playerController.maxHealth;
        healthSlider2.minValue = 0;
        healthSlider2.value = playerController.health;

        dashCooldownSlider.maxValue = playerInput.dashTimeDuration;
        dashCooldownSlider2.maxValue = playerInput.dashTimeDuration;

        dodgeCooldownSlider.maxValue = playerInput.dodgeCooldown;
        dodgeCooldownSlider2.maxValue = playerInput.dodgeCooldown;

        bulletTimeCooldownSlider.maxValue = playerInput.bulletTimeCooldown;
        bulletTimeCooldownSlider.value = playerInput.bulletTimeCooldown;
        bulletTimeCooldownSlider2.maxValue = playerInput.bulletTimeCooldown;
        bulletTimeCooldownSlider2.value = playerInput.bulletTimeCooldown;

        kickCooldownSlider.maxValue = playerInput.kickTimeDuration;
        kickCooldownSlider.value = playerInput.kickTimeDuration;
        kickCooldownSlider2.maxValue = playerInput.kickTimeDuration;
        kickCooldownSlider2.value = playerInput.kickTimeDuration;


        subLevelProgresScript = GameObject.FindWithTag("SubLevelProgres").GetComponent<SubLevelProgres>();

        UpdateUI();

        if (isForTrailer) {
            canvas.SetActive(false);
            
        }
    }


    private void UpdateUI()
    {
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            if (playerInventory.weapons[i] != null)
            {
                weaponSlots[i].GetComponent<Image>().sprite = weaponSlotsSprites[(int)playerInventory.weapons[i].GetComponent<Weapon>().weaponId];
                
                if (i == playerInventory.equipedId)
                {
                    weaponSlots[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    weaponSlots[i].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    
                }

            }
            else
            {
                weaponSlots[i].GetComponent<Image>().sprite = null;
                weaponSlots[i].GetComponent<Image>().color = Color.clear;
            }
        }

        if (playerInput.currWeapon.weaponId == WeaponID.Granade) {
            fragCarryText.GetComponent<TextMeshProUGUI>().color = Color.white;
        } else {
            fragCarryText.GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }


    void Update()
    {
        if (playerController.isInPlayerRoom) return;

        UpdateBullets();
       
        carringText.GetComponent<TextMeshProUGUI>().text = playerInput.currWeapon.ammoCarried.ToString();
        fragCarryText.GetComponent<TextMeshProUGUI>().text = playerInventory.fragCarried.ToString();

        healthSlider.value = Mathf.Lerp(healthSlider.value, playerController.health, Time.deltaTime * lerpSpeed);
        healthSlider2.value = Mathf.Lerp(healthSlider2.value, playerController.health, Time.deltaTime * lerpSpeed);

        dashCooldownSlider.value = dashCooldownSlider.maxValue - playerInput.tillNextDash;
        dashCooldownSlider2.value = dashCooldownSlider2.maxValue - playerInput.tillNextDash;

        dodgeCooldownSlider.value = dodgeCooldownSlider.maxValue - playerInput.tillNextDodge;
        dodgeCooldownSlider2.value = dodgeCooldownSlider2.maxValue - playerInput.tillNextDodge;

        if (playerInput.isBulletTime)
        {
            bulletTimeCooldownSlider.maxValue = playerInput.bulletTimeDuration;
            bulletTimeCooldownSlider2.maxValue = playerInput.bulletTimeDuration;

            bulletTimeCooldownSlider.value = playerInput.bulletTimeEndTime - Time.time;
            bulletTimeCooldownSlider2.value = playerInput.bulletTimeEndTime - Time.time;
        }
        else {
            bulletTimeCooldownSlider.maxValue = playerInput.bulletTimeCooldown;
            bulletTimeCooldownSlider2.maxValue = playerInput.bulletTimeCooldown;

            bulletTimeCooldownSlider.value = bulletTimeCooldownSlider.maxValue - playerInput.tillNextBulletTime;
            bulletTimeCooldownSlider2.value = bulletTimeCooldownSlider2.maxValue - playerInput.tillNextBulletTime;
        }

 

        kickCooldownSlider.value = kickCooldownSlider.maxValue - playerInput.tillNextKick;
        kickCooldownSlider2.value = kickCooldownSlider.maxValue - playerInput.tillNextKick;

        UpdateUI();
    }
    private void UpdateBullets()
    {
        // Clear existing bullet sprites
        foreach (GameObject bullet in bulletSprites)
        {
            Destroy(bullet);
        }
        bulletSprites.Clear();

        bulletContainer.GetComponent<HorizontalLayoutGroup>().spacing = playerInput.currWeapon.ammoSpacing;
        bulletContainer1.GetComponent<HorizontalLayoutGroup>().spacing = playerInput.currWeapon.ammoSpacing;
        bulletContainer2.GetComponent<HorizontalLayoutGroup>().spacing = playerInput.currWeapon.ammoSpacing;

        float ammoRatio = (float)playerInput.currWeapon.magCurrent / playerInput.currWeapon.magCapacity;

        // Spawn bullet sprites based on the current ammo count of the player's current weapon
        if (playerInput.currWeapon != null)
        {

            if (playerInput.currWeapon.magCapacity > 30)
            {
                int i = 0;

                while (i < playerInput.currWeapon.magCapacity / 2 && i < playerInput.currWeapon.magCurrent)
                {
                    GameObject bulletSprite = Instantiate(bulletPrefabSmall, bulletContainer1.transform);

                    //bulletSprite.transform.localScale = new Vector3(bulletSprite.transform.localScale.x, bulletSprite.transform.localScale.y, bulletSprite.transform.localScale.z);

                    bulletSprite.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

                    // Set bullet color based on ammo ratio


                    if (ammoRatio >= 2f / 3f)
                    {
                        bulletSprite.GetComponent<Image>().color = fullMagC;
                    }
                    else if (ammoRatio >= 1f / 3f)
                    {
                        bulletSprite.GetComponent<Image>().color = midMagC;
                    }
                    else
                    {
                        bulletSprite.GetComponent<Image>().color = emptyMagC;
                    }


                    bulletSprites.Add(bulletSprite);
                    i++;

                }

                while (i < playerInput.currWeapon.magCurrent)
                {
                    GameObject bulletSprite = Instantiate(bulletPrefabSmall, bulletContainer2.transform);

                    //bulletSprite.transform.localScale = new Vector3(bulletSprite.transform.localScale.x, bulletSprite.transform.localScale.y, bulletSprite.transform.localScale.z);

                    bulletSprite.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

                    // Set bullet color based on ammo ratio


                    if (ammoRatio >= 2f / 3f)
                    {
                        bulletSprite.GetComponent<Image>().color = fullMagC;
                    }
                    else if (ammoRatio >= 1f / 3f)
                    {
                        bulletSprite.GetComponent<Image>().color = midMagC;
                    }
                    else
                    {
                        bulletSprite.GetComponent<Image>().color = emptyMagC;
                    }


                    bulletSprites.Add(bulletSprite);
                    i++;
                }


            }
            else {
                for (int i = 0; i < playerInput.currWeapon.magCurrent; i++)
                {
                    GameObject bulletSprite = Instantiate(bulletPrefab, bulletContainer.transform);

                    bulletSprite.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

                    // Set bullet color based on ammo ratio


                    if (ammoRatio >= 2f / 3f)
                    {
                        bulletSprite.GetComponent<Image>().color = fullMagC;
                    }
                    else if (ammoRatio >= 1f / 3f)
                    {
                        bulletSprite.GetComponent<Image>().color = midMagC;
                    }
                    else
                    {
                        bulletSprite.GetComponent<Image>().color = emptyMagC;
                    }


                    bulletSprites.Add(bulletSprite);
                }
            }

        }

        bulletContainer.GetComponent<RectTransform>().anchoredPosition = originalPos;

    }


}
