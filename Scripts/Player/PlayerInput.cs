using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    [Header("--- Movement ---")]
    [SerializeField] float moveSpeed;

    [Header("--- References ---")]
    public Rigidbody2D rb;
    public Weapon currWeapon;

    private PlayerInventory playerInventory;

    [Header("--- Dash ---")]
    public float dashTimeDuration;
    

    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    public bool isDashing;

    [SerializeField] private TrailRenderer trailRenderer;

    [Header("--- KickController ---")]
    public float kickTimeDuration;

    [SerializeField] private GameObject bodyLeg;
    private bool isKicking;

    [Header("--- DodgeController ---")]
    public bool isDodging;
    public float dodgeForce;
    public float dodgeCooldown;
    public float dodgeDuration;

    private float dodgeNextTime;

    [Header("--- BulletTime ---")] 
    public float bulletTimeDuration = 1f; 
    public float bulletTimeCooldown = 5f;
    public bool isBulletTime;
    public float bulletTimeEndTime;
    public float bulletTimeNext;

    [SerializeField] private float bulletTimeRatio = 0.5f;
    [SerializeField] private float timeTransitionSpeed = 1f; 
    [SerializeField] private float audioPitchDuringBulletTime = 0.75f; 
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioSource bulletTimeIn;
    [SerializeField] private AudioSource bulletTimeOut;
    private float originalTimeScale;
    private float originalAudioPitch;

    private float bulletTimeStart;
    

    [Header("--- UiInfo ---")]
    public float tillNextBulletTime;
    public float tillNextDash;
    public float tillNextDodge;
    public float tillNextKick;

    [Header("--- References ---")]
    private PlayerControler playerControler;

    [Header("--- Animations ---")]
    [SerializeField] private GameObject legsObject;
    public Animator legsAnimator;

    [Header("--- PostProcess ---")]
    private VolumeManager volumeManager;

    [Header("--- Controlls ---")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private float originalTimeScalePauseMenu;
    [SerializeField] private bool isPauseMenu = false;
    [SerializeField] private bool isControlsMenu = false;

    

    private void Start()
    {
        tillNextBulletTime = 0;
        tillNextDash = 0;
        tillNextDodge = 0;
        
        bool isClubDanceScene = SceneManager.GetActiveScene().name == "ClubBoss";

        if (isClubDanceScene)
        {
            audioSource = GameObject.FindWithTag("BossMusic").GetComponent<AudioSource>();
        }
        else
        {
            audioSource = GameObject.FindWithTag("LevelController").GetComponent<AudioSource>();
        }
       
        playerInventory = GetComponent<PlayerInventory>();

        bodyLeg.SetActive(false);
        isKicking = false;
        isDashing = false;

        playerControler = gameObject.GetComponent<PlayerControler>();

        dodgeNextTime = Time.time + dodgeCooldown;

        isBulletTime = false;
        originalTimeScale = Time.timeScale;
        originalAudioPitch = audioSource.pitch;

        bulletTimeNext = 0f;

        legsAnimator = legsObject.GetComponent<Animator>();

        volumeManager = GameObject.FindWithTag("GlobalVolume").GetComponent<VolumeManager>();

        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);

        trailRenderer.emitting = false;
    }

    public void TogglePauseMenu()
    {
        if (isPauseMenu)
        {
            isPauseMenu = false;
            pauseMenu.SetActive(false);
          
            Time.timeScale = originalTimeScalePauseMenu;
            originalTimeScale = -1f;

        }
        else {
            isPauseMenu = true;
            pauseMenu.SetActive(true);

            if (Time.timeScale != 0.0) {
                originalTimeScalePauseMenu = Time.timeScale;
            }
            
            Time.timeScale = 0.0f;
        }
    }

    private void PauseMenu() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
        

            if (isControlsMenu)
            {
                ToggleControlsMenu();
            }

            TogglePauseMenu();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPauseMenu) TogglePauseMenu();

            ToggleControlsMenu();
        }
        

    }


    public void ToggleControlsMenu()
    {
        if (isControlsMenu)
        {
            isControlsMenu = false;
            controlsMenu.SetActive(false);
          

            Time.timeScale = originalTimeScalePauseMenu;
            originalTimeScale = -1f;

        }
        else
        {
            isControlsMenu = true;
            controlsMenu.SetActive(true);

            if (Time.timeScale != 0.0)
            {
                originalTimeScalePauseMenu = Time.timeScale;
            }
            Time.timeScale = 0.0f;
        }
    }

  

    private void Update()
    {
        if (playerControler.isDead) return;

        PauseMenu();

        if (isPauseMenu || isControlsMenu) return;

        Movement();

        if (playerControler.isInPlayerRoom) return;

        WeaponSwitch();


        BodyActions();
        currWeapon.WeaponInputActions();

        HandleBulletTime();
    }

    private void HandleBulletTime()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isBulletTime && Time.time > bulletTimeNext)
        {
            StartCoroutine(StartBulletTime());
        }

        if (isBulletTime && Time.time > bulletTimeEndTime)
        {
            StartCoroutine(EndBulletTime());
        }

        if (bulletTimeStart > 0)
        {
            bulletTimeStart -= Time.deltaTime;
        }
        else { bulletTimeStart = 0f; }
    }


    private void Movement()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.A))
            horizontalInput = -1f;
        if (Input.GetKey(KeyCode.D))
            horizontalInput = 1f;
        if (Input.GetKey(KeyCode.W))
            verticalInput = 1f;
        if (Input.GetKey(KeyCode.S))
            verticalInput = -1f;


        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput).normalized;
        Vector2 movement = movementDirection * moveSpeed;
        rb.velocity = movement;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 directionToMouse = mousePosition - transform.position;
        float angle = (Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg) - 90f;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));


        // legs animation
        if (rb.velocity.magnitude == 0)
        {
            legsAnimator.SetBool("IsWalking", false);
        }
        else
        {
            legsAnimator.SetBool("IsWalking", true);

            float legsRotationAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.y) * Mathf.Rad2Deg;
            legsObject.transform.rotation = Quaternion.Euler(0f, 0f, -legsRotationAngle);
        }


        if (Input.GetKey(KeyCode.LeftShift) && !isDashing)
        {
            Dash();
        }

        if (Input.GetKey(KeyCode.Q) && dodgeNextTime <= Time.time)
        {
            Dodge(0);
        }
        else if (Input.GetKey(KeyCode.E) && dodgeNextTime <= Time.time)
        {
            Dodge(1);
        }
    }

    private void Dodge(int side)
    {
        tillNextDodge = dodgeCooldown;
        StartCoroutine(DecreaseValueOverTime(tillNextDodge, 0f, dodgeCooldown, tillNextDodge, value => tillNextDodge = value));

        isDodging = true;

        dodgeNextTime = Time.time + dodgeCooldown;

        Vector2 dodgeVector;
        if (side == 1)
        {
            dodgeVector = Vector2.right;
        }
        else
        {
            dodgeVector = Vector2.left;
        }


        float dodgeDistance = dodgeForce * dodgeDuration;

        StartCoroutine(PerformDodge(dodgeVector, dodgeDistance));
        StartCoroutine(DodgeCooldown());

    }

    private IEnumerator PerformDodge(Vector2 dodgeVector, float dodgeDistance)
    {
        trailRenderer.emitting = true;
        float distanceMoved = 0;

        while (distanceMoved < dodgeDistance)
        {
            float step = dashForce * Time.deltaTime;
            transform.Translate(dodgeVector * step);
            distanceMoved += step;
            yield return null;
        }

        trailRenderer.emitting = false;
    }

    private IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeDuration);

        isDodging = false;
    }

    private void WeaponSwitch()
    {
        if (currWeapon.isReloading || (currWeapon.isMelee && currWeapon.nextFireTime > Time.time)) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) playerInventory.EquipWeapon((int)WeaponSlot.Melee);
        if (Input.GetKeyDown(KeyCode.Alpha2)) playerInventory.EquipWeapon((int)WeaponSlot.HandGun);
        if (Input.GetKeyDown(KeyCode.Alpha3)) playerInventory.EquipWeapon((int)WeaponSlot.Rifle);
        if (Input.GetKeyDown(KeyCode.G) && playerInventory.fragCarried > 0)      playerInventory.EquipWeapon((int)WeaponSlot.Granade);
    }

    private void Dash()
    {
        tillNextDash = dashTimeDuration;
        StartCoroutine(DecreaseValueOverTime(tillNextDash, 0f, dashTimeDuration, tillNextDash, value => tillNextDash = value));

        isDashing = true;

        Vector2 dashVector = Vector2.up;

        float dashDistance = dashForce * dashDuration;

        StartCoroutine(PerformDash(dashVector, dashDistance));
        StartCoroutine(DashCooldown());
    }

    private IEnumerator PerformDash(Vector2 dashVector, float dashDistance)
    {
        trailRenderer.emitting = true;
        float distanceMoved = 0;

        while (distanceMoved < dashDistance)
        {
            float step = dashForce * Time.deltaTime;
            transform.Translate(dashVector * step);
            distanceMoved += step;
            yield return null;
        }

        trailRenderer.emitting = false;
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashTimeDuration);

        isDashing = false;
    }

    private void BodyActions()
    {
        if (Input.GetKey(KeyCode.F) && !isKicking)
        {
            Kick();
        }
        if (Input.GetMouseButtonDown(1) && (playerInventory.equipedId != 0 || !playerInventory.haveFists) && currWeapon)
        {
            ThrowWeapon();
        }
    }

    private void ThrowWeapon()
    {
        if (currWeapon.weaponId == WeaponID.Fists) return;
        GameObject thrownWeapon = Instantiate(playerInventory.throwWeaponsPrefabs[(int)currWeapon.weaponId], playerInventory.weaponSpawnPoint.position + playerInventory.weaponSpawnPoint.transform.up, playerInventory.weaponSpawnPoint.rotation);
        thrownWeapon.GetComponent<ThrowableObject>().Throw(transform.position);

        playerInventory.ThrowWeapon();
        playerInventory.EquipWeapon(0);
    }

    private void Kick()
    {
        tillNextKick = kickTimeDuration;
        StartCoroutine(DecreaseValueOverTime(tillNextKick, 0f, kickTimeDuration, tillNextKick, value => tillNextKick = value));

        isKicking = true;
        bodyLeg.SetActive(true);
        StartCoroutine(KickStop());
    }

    private IEnumerator KickStop()
    {
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(kickTimeDuration / 5f);

        bodyLeg.SetActive(false);

        yield return new WaitForSeconds((kickTimeDuration / 5f) * 4f);

        isKicking = false;
    }

    private IEnumerator StartBulletTime()
    {
        tillNextBulletTime = bulletTimeCooldown;
        bulletTimeIn.Play();
        isBulletTime = true;
        bulletTimeEndTime = Time.time + bulletTimeDuration;

       

        float elapsedTime = 0f;
        float initialTimeScale = Time.timeScale;

        while (elapsedTime < 1f)
        {
            Time.timeScale = Mathf.Lerp(initialTimeScale, bulletTimeRatio, elapsedTime);
            audioSource.pitch = Mathf.Lerp(originalAudioPitch, audioPitchDuringBulletTime, elapsedTime);
            elapsedTime += Time.deltaTime / (bulletTimeDuration * timeTransitionSpeed);
            yield return null;
        }

        Time.timeScale = bulletTimeRatio;
        audioSource.pitch = audioPitchDuringBulletTime;

        volumeManager.SetVolume("bulletTime", true);

        StartCoroutine(BulletTimeCooldown());
    }

    private IEnumerator EndBulletTime()
    {
        if (isBulletTime) {
            bulletTimeOut.Play();
            float elapsedTime = 0f;
            float initialTimeScale = Time.timeScale;

            while (elapsedTime < 1f)
            {
                Time.timeScale = Mathf.Lerp(initialTimeScale, originalTimeScale, elapsedTime);
                audioSource.pitch = Mathf.Lerp(audioPitchDuringBulletTime, originalAudioPitch, elapsedTime);
                elapsedTime += Time.deltaTime / (bulletTimeDuration * timeTransitionSpeed);
                yield return null;
            }

            Time.timeScale = originalTimeScale;
            audioSource.pitch = originalAudioPitch;
            isBulletTime = false;

            volumeManager.SetVolume("basic", true);

            bulletTimeNext = Time.time + bulletTimeCooldown;
            
            StartCoroutine(DecreaseValueOverTime(tillNextBulletTime, 0f, bulletTimeCooldown, tillNextBulletTime, value => tillNextBulletTime = value));

        }


    }

    public void EndBulletTimeLevelReset() {
        Time.timeScale = originalTimeScale;
        audioSource.pitch = originalAudioPitch;
        isBulletTime = false;

        volumeManager.SetVolume("basic", true);
        bulletTimeNext = Time.time + bulletTimeCooldown;
        tillNextBulletTime = bulletTimeCooldown;
    }

    private IEnumerator BulletTimeCooldown()
    {
        yield return new WaitForSeconds(bulletTimeDuration);

        StartCoroutine(EndBulletTime());
    }

    private IEnumerator DecreaseValueOverTime(float originalValue, float targetValue, float duration, float initialValue, Action<float> setValueCallback)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float lerpedValue = Mathf.Lerp(originalValue, targetValue, elapsedTime / duration);
            setValueCallback(lerpedValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        setValueCallback(targetValue);
    }

}