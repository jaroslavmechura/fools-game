using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [Header("--- Stats ---")]
    public float health;
    public float maxHealth;

    public bool isInPlayerRoom = false;

    public GameObject deadBody;

    [Header("--- Attributes ---")]
    public bool isDead = false;

    [Header("--- References ---")]
    private PlayerInput playerInput;
    private PlayerSkin playerSkin;
    public List<GameObject> toDissable;

    [Header("--- Visuals ---")]
    [SerializeField] private GameObject blood;

    [Header("--- Debug ---")]
    [SerializeField] private bool isInvincible = false;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerSkin = GetComponent<PlayerSkin>();
    }


    void Update()
    {
        if (isInPlayerRoom) return;

        if (Input.GetKeyDown(KeyCode.H)) {
            GetComponent<PlayerInput>().EndBulletTimeLevelReset();
            RestartScene();
        }
    }


    public void TakeDamage(float dmg, Vector3 puddleDirection)
    {
        if (isInvincible) return;

        GameObject bloodObject = Instantiate(blood, transform.position, Quaternion.identity);
        bloodObject.GetComponent<BloodDestructor>().RotateSelf(puddleDirection);

        health -= dmg;
        CheckHealth();
    }



    public void CheckHealth()
    {
        if (health <= 0f && !isDead)
        {
            Death();
        }
    }


    public void RestartScene()
    {
        GameObject.FindWithTag("LevelController").GetComponent<LevelManager>().RestartCurrentLevel();
    }


    public void Death() 
    {
        if (gameObject.GetComponent<PlayerInput>().isBulletTime )
        {
            gameObject.GetComponent<PlayerInput>().StartCoroutine("EndBulletTime",0f);
        }

        isDead = true;

        foreach (GameObject bodyPart in toDissable) {
            Destroy(bodyPart);
        }

        GameObject body = Instantiate(deadBody, transform.position, Quaternion.identity);
        body.GetComponent<DeadBody>().totalBulletHoles = Random.Range(2, 5);
        body.GetComponent<DeadBody>().InitDeadBody(playerSkin.torsoId, playerSkin.headId);

        playerInput.rb.isKinematic = true;
        playerInput.rb.velocity = new Vector2(0f, 0f);

        GetComponent<CircleCollider2D>().enabled = false;

        GameObject[] enemiesToStop = GameObject.FindGameObjectsWithTag("Enemy");// i want to find all enemies in level
        foreach (GameObject enemyObj in enemiesToStop) {
            enemyObj.GetComponent<Enemy>().Dissalert();
        }

        Invoke("RestartScene", 5f);
    }


    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.collider.CompareTag("Enemy")) 
        {
            collision.gameObject.GetComponent<Enemy>().StartCoroutine(collision.gameObject.GetComponent<Enemy>().StopMovement(0.1f));
        }
        
        if (collision.collider.CompareTag("MeleeWeapon"))
        {
            
            if (playerInput.currWeapon.isMelee && playerInput.currWeapon.isAttacking)
            {
                collision.collider.GetComponentInParent<MeleeWeapon>().PlaySoundCoverEffect();
            }
            else 
            {
                collision.collider.GetComponentInParent<MeleeWeapon>().PlaySoundHitEffect();
                TakeDamage(collision.collider.GetComponentInParent<MeleeWeapon>().damage, collision.collider.transform.position);
            }
        }
    }


    public void HealthPickUp(float healthIn) {
        health += healthIn;

        if (health > maxHealth) health = maxHealth;
    }
}
