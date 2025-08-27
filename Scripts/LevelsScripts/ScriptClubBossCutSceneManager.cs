using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScriptClubBossCutSceneManager : MonoBehaviour
{
    public AudioClip clip;

    public bool isOneTimer = true;
    public string eventName;

    [SerializeField] private List<Light2D> lightsCutScene;

    [Header("--- References ---")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject fightManager;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource talkSource;




    private void Start()
    {
        musicSource = GameObject.FindWithTag("BossMusic").GetComponent<AudioSource>();

        if (isOneTimer && PlayerPrefs.HasKey(eventName))
        {
            if (PlayerPrefs.GetInt(eventName) == 1)
            {
                foreach (Light2D l in lightsCutScene) {
                    Destroy(l.gameObject);
                }

                Destroy(gameObject);
            }
        }
        foreach (Light2D l in lightsCutScene)
        {
            l.enabled = false;
        }
        player = GameObject.FindWithTag("Player");

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isOneTimer)
            {
                PlayerPrefs.SetInt(eventName, 1);

                fightManager.GetComponent<Collider2D>().enabled = false;

                StartCoroutine(SceneScript());
            }
        }
    }



    private IEnumerator SceneScript() {

        musicSource.volume = 0.25f;
        musicSource.Play();
        talkSource.Play();

        // stop player controlls and UI
        player.GetComponent<PlayerInput>().enabled = false;
        player.transform.Find("Canvas").gameObject.SetActive(false);
        player.GetComponent<Rigidbody2D>().isKinematic = true;
        player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        float zoom = player.GetComponent<PlayerInput>().currWeapon.cameraWiew;
        player.GetComponent<PlayerInput>().currWeapon.cameraWiew = 15f;

        // rotation
        player.transform.rotation = Quaternion.identity;
        player.transform.Find("Legs").transform.rotation = Quaternion.identity;
        boss.transform.rotation = Quaternion.identity;
        boss.transform.Rotate(0f, 0f, 180f);

        // animators
        player.GetComponent<PlayerInput>().legsAnimator.SetBool("IsWalking", true);
        boss.GetComponent<ClubBoss>().legsAnimator.SetBool("IsWalking", false);
        boss.GetComponent<ClubBoss>().legsAnimator.SetBool("IsStepping", true);

        // move both of them 
        float timer = 0f;
        while (timer < 3f) {
            player.transform.Translate(transform.up * Time.deltaTime * 5.5f);
            boss.transform.Translate(transform.up * Time.deltaTime * 3f);

            timer += Time.deltaTime * 1f;
            yield return null;
        }

        // stop them
        player.GetComponent<PlayerInput>().legsAnimator.SetBool("IsWalking", false);
        boss.GetComponent<ClubBoss>().legsAnimator.SetBool("IsWalking", false);
        boss.GetComponent<ClubBoss>().legsAnimator.SetBool("IsStepping", false);

        //chat 
        yield return new WaitForSeconds(1f);
        // lights on
        foreach (Light2D l in lightsCutScene)
        {
            l.enabled = true;
        }
        yield return new WaitForSeconds(6f);
        boss.GetComponent<ClubBoss>().weapon.animator.SetTrigger("MagReload");
        boss.GetComponent<ClubBoss>().weapon.reloadSound.clip = boss.GetComponent<ClubBoss>().weapon.reloadClips[boss.GetComponent<ClubBoss>().weapon.reloadClipsIndex];
        boss.GetComponent<ClubBoss>().weapon.reloadSound.Play();
        yield return new WaitForSeconds(5f);
        

        // startFight
        player.GetComponent<PlayerInput>().enabled = true;
        player.transform.Find("Canvas").gameObject.SetActive(true);
        player.GetComponent<Rigidbody2D>().isKinematic = false;
        player.GetComponent<PlayerInput>().currWeapon.cameraWiew = zoom;

        fightManager.GetComponent<ClubBossFightManager>().StartFight();

        musicSource.volume = 0.5f;

        // lights off
        foreach (Light2D l in lightsCutScene)
        {
            Destroy(l.gameObject);
        }

        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

}

