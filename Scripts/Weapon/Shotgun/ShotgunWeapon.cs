using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class ShotgunWeapon : Weapon
{
    [Header("--- Special ---")]
    [SerializeField] private float shotgunKickingBackDuration;
    [SerializeField] private float shotgunKickbackForce;
    [SerializeField] private GameObject player;


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

    public override void WeaponInputActions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isWallClip) return;

            if (FireShotgun(false)) {

                Dash();
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


    public bool FireShotgun(bool byEnemy)
    {
        if (Time.time > nextFireTime && magCurrent > 0 && !isReloading)
        {
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

            muzzleFlash.GetComponent<ParticleSystem>().Play();
            muzzleLight.GetComponent<Light2D>().intensity = 0.0f;
            Invoke("MuzzleFlashEnd", 1 / fireRate);

            if (animator != null)
            {
                animator.SetTrigger("Shot");
            }

            Invoke("SpawnCasing", 0.75f);

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


    public override void Reload()
    {
        if (isReloading) return;

        if (ammoCarried > 0 && magCurrent < magCapacity)
        {
            ammoCarried--;
            magCurrent++;

            if (magCapacity == magCurrent)
            {
                reloadSound.clip = reloadClips[0];
                animator.SetTrigger("LoadLast");

            }
            else
            {
                reloadSound.clip = reloadClips[1];
                animator.SetTrigger("LoadBullet");
            }

            reloadSound.Play();

            reloadClipsIndex++;
            if (reloadClipsIndex >= reloadClips.Count) reloadClipsIndex = 0;

            if (magCurrent == magCapacity) StartCoroutine(Reloading1());
            else StartCoroutine(ReloadingContinue());
        }
    }


    private IEnumerator ReloadingContinue()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTimeDuration);

        isReloading = false;
    }


    public override void SpawnCasing()
    { 
        casingSpawnPoint.GetComponent<BulletCasesSpawn>().SpawnBulletCase();
    }
}
