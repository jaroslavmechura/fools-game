using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClubBoss : Enemy
{
    public float lerpSpeed = 5f;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthSlider2;

    public bool isInCutScene;

    public bool isForTrailer;

    protected override void Start()
    {
        base.Start();
        agent.speed = chaseSpeed;

        healthSlider.maxValue = maxHealth;
        healthSlider.minValue = 0;
        healthSlider.value = health;
        healthSlider2.maxValue = maxHealth;
        healthSlider2.minValue = 0;
        healthSlider2.value = health;

        if (isForTrailer)
        {
            transform.Find("Canvas").gameObject.SetActive(false);
        }
        
    }
    // Update is called once per frame
    protected override void Update()
    {
        UpdateUI();
        if (isInCutScene) return;
        
        {
            if (isGrabbed) { enemyCollider.enabled = false; transform.position = grabPoint.transform.position; }
            else { enemyCollider.enabled = true; }
            if (isStunned || isPunched || isDeactive || isGrabbed || isDummy) return;
            enemyCollider.enabled = true;
        }

        chaseLetDownTimer = chaseLetDownLength;

        ChasePlayer();
        CheckForDetection();


        LegsAnim();

        UpdateUI();
    }

    private void UpdateUI() {

    

        healthSlider.value = Mathf.Lerp(healthSlider.value, health, Time.deltaTime * lerpSpeed);
        healthSlider2.value = Mathf.Lerp(healthSlider2.value, health, Time.deltaTime * lerpSpeed);
    }

    public override void EnterSearchMode() { }
}
