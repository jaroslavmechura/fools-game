using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class ClubBossFightManager : MonoBehaviour
{
    public ClubBoss boss;
    public List<Enemy> fromUpLeft;
    public List<Enemy> fromUpRight;
    public List<Enemy> fromDownLeft;
    public List<Enemy> fromDownRight;

    public float delayBetweenWaves;
    public int activatePerWave;

    private int activatedFromUpLeft;
    private int activatedFromUpRight;
    private int activatedFromDownLeft; 
    private int activatedFromDownRight;
    private bool allActivated;

    private bool isStarted = false;

    private void Start()
    {
        boss.transform.Find("Canvas").gameObject.SetActive(false);
        boss.GetComponent<ClubBoss>().isInCutScene = true;
        boss.GetComponent<NavMeshAgent>().enabled = false;

        foreach (Enemy enemy in fromUpLeft)
        {
            enemy.isDeactive = true;
            enemy.isAllknowing = true;
        }
        foreach (Enemy enemy in fromUpRight)
        {
            enemy.isDeactive = true;
            enemy.isAllknowing = true;
        }
        foreach (Enemy enemy in fromDownLeft)
        {
            enemy.isDeactive = true;
            enemy.isAllknowing = true;
        }
        foreach (Enemy enemy in fromDownRight)
        {
            enemy.isDeactive = true;
            enemy.isAllknowing = true;
        }

        isStarted = false;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartFight();
        }
    }

    public void StartFight() {
        boss.transform.Find("Canvas").gameObject.SetActive(true);
        boss.GetComponent<ClubBoss>().isInCutScene = false;
        boss.GetComponent<NavMeshAgent>().enabled = true;

        isStarted = true;

        StartCoroutine(ActivateEnemies());

        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.enabled = false;
        }
        
    }

    IEnumerator ActivateEnemies()
    {
        while (!allActivated)
        {
           

            // Activate enemies from fromUpLeft
            for (int i = activatedFromUpLeft; i < Mathf.Min(fromUpLeft.Count, activatedFromUpLeft + activatePerWave); i++)
            {
                fromUpLeft[i].isAllknowing = true;
                fromUpLeft[i].isDeactive = false;
            }
            activatedFromUpLeft += activatePerWave;

            // Activate enemies from fromUpRight
            for (int i = activatedFromUpRight; i < Mathf.Min(fromUpRight.Count, activatedFromUpRight + activatePerWave); i++)
            {
                fromUpRight[i].isAllknowing = true;
                fromUpRight[i].isDeactive = false;
            }
            activatedFromUpRight += activatePerWave;

            // Activate enemies from fromDownLeft
            for (int i = activatedFromDownLeft; i < Mathf.Min(fromDownLeft.Count, activatedFromDownLeft + activatePerWave); i++)
            {
                fromDownLeft[i].isAllknowing = true;
                fromDownLeft[i].isDeactive = false;
            }
            activatedFromDownLeft += activatePerWave;

            // Activate enemies from fromDownRight
            for (int i = activatedFromDownRight; i < Mathf.Min(fromDownRight.Count, activatedFromDownRight + activatePerWave); i++)
            {
                fromDownRight[i].isAllknowing = true;
                fromDownRight[i].isDeactive = false;
            }
            activatedFromDownRight += activatePerWave;

            // Check if all enemies are activated
            if (activatedFromUpLeft >= fromUpLeft.Count && activatedFromUpRight >= fromUpRight.Count && activatedFromDownLeft >= fromDownLeft.Count && activatedFromDownRight >= fromDownRight.Count)
            {
                allActivated = true;
            }

            yield return new WaitForSeconds(delayBetweenWaves);

        }
    }
}
