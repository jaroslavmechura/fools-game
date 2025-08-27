using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class Weapon : MonoBehaviour
{
    public WeaponID weaponId;

    [Header("--- Stats ---")]
    public float fireRate;
    public float reloadTimeDuration;
    public bool isMelee = true;
    public bool isAttacking;
    public float cameraWiew;
    public float wallClipRange;
   

    public float cameraShakeForce;

    [SerializeField] private bool haveMagazine;


    [Header("--- Ammunition ---")]
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;

    public int magCurrent;
    public int magCapacity;
    public int ammoCarried;


    [Header("--- Attributes ---")]
    public float nextFireTime;
    public bool isReloading = false;
    public bool isWallClip = false;


    [Header("--- Visuals ---")]
    public GameObject muzzleLight;
    public GameObject muzzleFlash;
    public Animator animator;
    public GameObject casingSpawnPoint;

    [SerializeField] private BulletCasesSpawn casingSpawnScript;
    [SerializeField] private GameObject magazineSpawnPoint;
    [SerializeField] private MagazineDrop magazineSpawnScript;

    public Color ammoColor;
    public float ammoSpacing;


    [Header("--- Audio ---")]
    public AudioSource fireSound;
    public AudioSource reloadSound;
    public AudioSource emptyCockSource;

    public List<AudioClip> shotClips;
    public int shotClipsIndex;
    public List<AudioClip> reloadClips;
    public int reloadClipsIndex;


    [Header("--- Refs ---")]
    public CameraFollow cameraScript;

    public abstract void WeaponInputActions();


    private void Start()
    {
        muzzleLight.GetComponent<Light2D>().intensity = 0f;

        shotClipsIndex = 0;
        reloadClipsIndex = 0;

        cameraScript = Camera.main.gameObject.GetComponent<CameraFollow>();

        
       
    }


    public void Fire(bool byEnemy)
    {
        if (Time.time > nextFireTime && magCurrent > 0 && !isReloading)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null) {
                bulletScript.RotateSelf(byEnemy);
            }
           
            
            

            animator.SetTrigger("Shot");

            muzzleFlash.GetComponent<ParticleSystem>().Play();
            muzzleLight.GetComponent<Light2D>().intensity = 0.0f;
            Invoke("MuzzleFlashEnd", 1/fireRate);
            
            casingSpawnPoint.GetComponent<BulletCasesSpawn>().SpawnBulletCase();

            fireSound.clip = shotClips[shotClipsIndex];
            fireSound.Play();
            
            nextFireTime = Time.time + 1f / fireRate;
            magCurrent--;
            if (magCurrent <= 0) {
                animator.SetBool("EmptyMag", true);
            }
            
            shotClipsIndex++;
            if (shotClipsIndex >= shotClips.Count) shotClipsIndex = 0;

            if (!byEnemy)
            {
                cameraScript.CameraShake(cameraShakeForce);
            }

        }
        if (magCurrent <= 0 && Time.time > nextFireTime) { 
            
            emptyCockSource.Play();
            nextFireTime = Time.time + 1f / fireRate;
        }


        
    }


    public virtual void SpawnCasing()
    {
        magazineSpawnPoint.GetComponent<MagazineDrop>().SpawnMagazine();
    }


    public virtual void MuzzleFlashEnd()
    {
        if (muzzleLight == null) return;

        Light2D muzzleL = muzzleLight.GetComponent<Light2D>();
        if (muzzleL != null) muzzleL.intensity = 0f;
    }


    public virtual void Reload()
    {
        if (isReloading) return;

        if (ammoCarried <= 0) return;


/*
        if (ammoCarried >= magCapacity)
        {
            magCurrent = magCapacity;
            ammoCarried -= magCapacity;

           
           
        }
        else 
        {
            magCurrent = ammoCarried;
            ammoCarried = 0;

        }*/

        animator.SetTrigger("MagReload");
        animator.SetBool("EmptyMag", false);

        reloadSound.clip = reloadClips[reloadClipsIndex];
        reloadSound.Play();

        reloadClipsIndex++;
        if (reloadClipsIndex >= reloadClips.Count) reloadClipsIndex = 0;

        Invoke("SpawnCasing", 0.8f);

        StartCoroutine(Reloading());
    }


    public IEnumerator Reloading(){
        isReloading = true;
        Invoke("EndReload", reloadTimeDuration);

        magCurrent = 0;

        int stop = magCapacity;

        if (ammoCarried < magCapacity) stop = ammoCarried;
        

        for (int i = 0; i < stop; i++) {
            magCurrent ++;
            ammoCarried--;
            yield return new WaitForSeconds(reloadTimeDuration / magCapacity);
        }

    }


    public void DestroyOnThrow() {
        Destroy(gameObject);
    }

    public IEnumerator Reloading1()
    {
        isReloading = true;
        Invoke("EndReload", reloadTimeDuration);


        //magCurrent++;
        //ammoCarried--;
        yield return new WaitForSeconds(reloadTimeDuration / magCapacity);
        
       
    }

    private void EndReload() {
        isReloading = false;
    }

}
