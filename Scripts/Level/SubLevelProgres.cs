using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubLevelProgres : MonoBehaviour
{
    [Header("--- EnemyTracker ---")]
    public List<GameObject> enemyList;
    public int count;

    [Header("--- EnemyDeathAudio ---")]
    public AudioSource enemyDeathSwearAudio;
    public List<AudioClip> enemyDeathSwearClips;

    [Header("--- Manager ---")]
    public GameObject nextLevel;

    [Header("--- PostProcess ---")]
    private VolumeManager volumeManager;
    public float killEffectLength;

    [Header("--- Refs ---")]
    public CameraFollow cameraScript;
    public GameObject player;
    public PlayerControler playerControler;

    [Header("--- Announcement ---")]
    public bool isScreenOn=false;
    public GameObject clearObject;
    public GameObject deathObject;
    public AudioSource clearAudioSource;
    public List<string> deathTexts;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerControler = player.GetComponent<PlayerControler>();


        cameraScript = Camera.main.gameObject.GetComponent<CameraFollow>();
        if (nextLevel != null) nextLevel.SetActive(false);

        for (int i = 0; i < enemyList.Count; i++) {
            enemyList[i].GetComponent<Enemy>().SetSubLevelId(i);
        }
        count = enemyList.Count;

        volumeManager = GameObject.FindWithTag("GlobalVolume").GetComponent<VolumeManager>();

        clearObject.SetActive(false);
        deathObject.SetActive(false);
    }

    void Update()
    {
        bool bolean = true;
        int subCount = 0;
        foreach(GameObject enemy in enemyList)
        {
            if (enemy != null) {
                bolean = false;
                subCount++;
            }
        }

        if (bolean && !isScreenOn) {
            if (nextLevel != null) nextLevel.SetActive(true);
            StartCoroutine(ClearedScreen());
            isScreenOn = true;
        }

        count = subCount;

        if (playerControler.isDead && !isScreenOn) {
            cameraScript.CameraShake(20f);
            deathObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = deathTexts[UnityEngine.Random.Range(0, deathTexts.Count)];
            deathObject.SetActive(true);
            clearAudioSource.Play();

            
            isScreenOn = true;
        }
    }

    public void SetEnemyDeath(int id) {
        enemyList[id] = null;

        if (UnityEngine.Random.Range(0, 4) == 0) {
            enemyDeathSwearAudio.clip = enemyDeathSwearClips[UnityEngine.Random.Range(0, enemyDeathSwearClips.Count)];
            enemyDeathSwearAudio.Play();
        }
        cameraScript.CameraShake(2f);
        StartCoroutine(KillVolume());

    }

    private IEnumerator KillVolume() {
        if (!playerControler.GetComponent<PlayerInput>().isBulletTime)
        {
            volumeManager.SetVolume("kill", false);
            yield return new WaitForSeconds(killEffectLength);
            volumeManager.SetVolume("basic", false);
        }


    }

    private IEnumerator ClearedScreen() {
        clearObject.SetActive(true);
        clearAudioSource.Play();

        cameraScript.CameraShake(10f);

        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(1f);
        Time.timeScale = 1f;

        TextMeshProUGUI textMesh = clearObject.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Color startColor = textMesh.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Set alpha to 0

        Image background = clearObject.transform.Find("Back").GetComponent<Image>();
        Color startColor2 = background.color;
        float origA = startColor2.a;
        Color endColor2 = new Color(startColor2.r, startColor2.g, startColor2.b, 0f); // Set alpha to 0

        float elapsedTime = 0f;
        float duration = 2f; // Duration for the transition

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            textMesh.color = Color.Lerp(startColor, endColor, t);
            background.color = Color.Lerp(startColor2, endColor2, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the text is completely transparent at the end
        textMesh.color = endColor;

        clearObject.SetActive(false);

    }


}
