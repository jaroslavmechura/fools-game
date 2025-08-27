using UnityEngine;


public class MeleeWeapon : Weapon
{
    [Header("--- Melee Audio ---")]
    public AudioSource meleeHitSound;
    public AudioSource meleeCoverSound;

    [Header("--- Special ---")]
    public float damage;
    public float forceSeconds;

    [SerializeField] private float attackLength;
    [SerializeField] private bool isAttack1 = false;
    [SerializeField] private GameObject wallHitParticles;

    [SerializeField] private BoxCollider2D weaponHitBox;


    public void Start()
    {
        shotClipsIndex = 0;
        cameraScript = Camera.main.gameObject.GetComponent<CameraFollow>();
    }

    public override void WeaponInputActions()
    {
        if (Input.GetMouseButton(0))
        {
            Slash(false);
        }
    }

    public void PlaySoundHitEffect() {
        meleeHitSound.Play();
    }

    public void PlaySoundCoverEffect()
    {
        meleeCoverSound.Play();
    }

    public void Slash(bool byEnemy)
    {
        if (Time.time > nextFireTime)
        {
            if (animator != null && !isAttack1)
            {
                animator.SetTrigger("Attack1");
                animator.SetBool("Attack1bool", true);

                isAttack1 = true;
                weaponHitBox.enabled = true;
                isAttacking = true;
                Invoke("TurnOffHitBox", attackLength);
            }
            else if (animator != null && isAttack1)
            {
                animator.SetTrigger("Attack2");
                animator.SetBool("Attack1bool", false);

                isAttack1 = false;
                weaponHitBox.enabled = true;
                isAttacking = true;
                Invoke("TurnOffHitBox", attackLength);
            }

            fireSound.clip = shotClips[shotClipsIndex];
            fireSound.Play();

            if (byEnemy)
            {
                nextFireTime = Time.time + 1f / (fireRate / 2.0f);
            }
            else
            {
                nextFireTime = Time.time + 1f / fireRate;
            }

            shotClipsIndex++;
            if (shotClipsIndex >= shotClips.Count) shotClipsIndex = 0;

            if (!byEnemy)
            {
                cameraScript.CameraShake(cameraShakeForce);
            }
        }
    }

    private void TurnOffHitBox()
    {
        isAttacking = false;
        weaponHitBox.enabled = false;
    }
}
