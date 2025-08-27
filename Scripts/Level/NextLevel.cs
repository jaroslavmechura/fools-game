using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    public string nextLevelName;

    [Header("--- Scaling ---")]
    [SerializeField] private float scaleSpeed = 1.0f; 
    [SerializeField] private float maxScale = 1.2f;   
    [SerializeField] private float minScale = 0.8f;   
    private Vector3 originalScale;
    private float timeCounter = 0.0f;
    private float scaleFactor;

    [Header("--- References ---")]
    private LevelManager levelManager;

    [Header("--- Dabing Reset ---")]
    [SerializeField] private List<string> playerPreffReset;


    void Start()
    {
        levelManager = GameObject.FindWithTag("LevelController").GetComponent<LevelManager>();
        if (levelManager == null) Debug.LogError("levelManager not set.");

        originalScale = transform.localScale;
    }


    void Update()
    {
        timeCounter += Time.deltaTime * scaleSpeed;
        scaleFactor = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(timeCounter) + 1) / 2);

        transform.localScale = originalScale * scaleFactor;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerInput>().EndBulletTimeLevelReset();

            foreach (string str in playerPreffReset) 
            {
                PlayerPrefs.SetInt(str, 0);
            }

            levelManager.LoadNextRoom(nextLevelName);
        }
    }
}
