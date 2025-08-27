using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;



public class ShortyWeapon : Weapon
{

    [Header("--- Special ---")]
    [SerializeField] private float shotgunKickingBackDuration;
    [SerializeField] private float shotgunKickbackForce;
    [SerializeField] private GameObject player;
    [SerializeField] private bool first = false;

    [Header("--- Shorty---")]
    [SerializeField] private GameObject shortyBarrel1;
    [SerializeField] private GameObject shortyBarrel2;
    [SerializeField] private GameObject shortyMuzzleLight1;
    [SerializeField] private GameObject shortyMuzzleFlash1;
    [SerializeField] private GameObject shortyMuzzleLight2;
    [SerializeField] private GameObject shortyMuzzleFlash2;
    [SerializeField] private GameObject shortyCasingSpawnPoint1;
    [SerializeField] private GameObject shortyCasingSpawnPoint2;

    private void Start()
    {
        cameraScript = Camera.main.gameObject.GetComponent<CameraFollow>();

        player = GameObject.FindWithTag("Player");
        MuzzleFlashEnd();


    }

    private void LateUpdate()
    { 
        animator.SetBool("isWallClip", isWallClip);
    }

    public override void MuzzleFlashEnd()
    {
        shortyMuzzleLight1.GetComponent<Light2D>().intensity = 0f;
        shortyMuzzleLight2.GetComponent<Light2D>().intensity = 0f;
    }


    public override void WeaponInputActions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isWallClip) return;

            if (!first)
            {
                if (FireShorty(1, false))
                {
                    first = true;
                    Dash();
                }
            }
            else {
                if (FireShorty(2, false))
                {
                    first = false;
                    Dash();
                }
            }
        }
        else if (Input.GetKey(KeyCode.R))
        {
            Reload();
        }
    }


    private void Dash()
    {
        Vector2 dashVector = -Vector2.up;
        float dashDistance = shotgunKickbackForce * shotgunKickingBackDuration;

        StartCoroutine(PerformDash(dashVector, dashDistance));
    }


    private IEnumerator PerformDash(Vector2 dashVector, float dashDistance)
    {
        float distanceMoved = 0;

        while (distanceMoved < dashDistance)
        {
            float step = shotgunKickbackForce * Time.deltaTime;
            player.transform.Translate(dashVector * step);
            distanceMoved += step;
            yield return null;
        }
    }


    public override void Reload()
    {
        if (isReloading) return;

        if (ammoCarried > 0)
        {


            reloadSound.clip = reloadClips[reloadClipsIndex];
            reloadSound.Play();

            animator.SetTrigger("Reload");

            Invoke("SpawnCasing", 0.85f);
        }
        else { return; }


        reloadClipsIndex++;
        if (reloadClipsIndex >= reloadClips.Count) reloadClipsIndex = 0;

        StartCoroutine(Reloading());
    }

    public bool FireShorty(int i, bool byEnemy)
    {
        if (Time.time > nextFireTime && magCurrent > 0 && !isReloading)
        {
            if (i == 1)
            {
                Instantiate(bulletPrefab, shortyBarrel1.transform.position, shortyBarrel1.transform.rotation);
                shortyMuzzleFlash1.GetComponent<ParticleSystem>().Play();
                shortyMuzzleLight1.GetComponent<Light2D>().intensity = 0.0f;
                Invoke("MuzzleFlashEnd", 1 / fireRate);
            }
            else
            {
                Instantiate(bulletPrefab, shortyBarrel2.transform.position, shortyBarrel2.transform.rotation);
                shortyMuzzleFlash2.GetComponent<ParticleSystem>().Play();
                shortyMuzzleLight2.GetComponent<Light2D>().intensity = 0.0f;
                Invoke("MuzzleFlashEnd", 1 / fireRate);
            }

            if (animator != null)
            {
                animator.SetTrigger("Shot");
            }

            fireSound.clip = shotClips[shotClipsIndex];
            fireSound.Play();

            nextFireTime = Time.time + 1f / fireRate;
            magCurrent--;

            shotClipsIndex++;
            if (shotClipsIndex >= shotClips.Count) shotClipsIndex = 0;

            if (!byEnemy)
            {
                cameraScript.CameraShake(cameraShakeForce);
            }

            return true;

        }
        if (magCurrent <= 0 && emptyCockSource != null && Time.time > nextFireTime)
        {
            emptyCockSource.Play();
            nextFireTime = Time.time + 1f / fireRate;
        }
        return false;
    }
    

    public override void SpawnCasing()
    {
        shortyCasingSpawnPoint1.GetComponent<BulletCasesSpawn>().SpawnBulletCase();
        shortyCasingSpawnPoint2.GetComponent<BulletCasesSpawn>().SpawnBulletCase();
    }
}
