using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptBossAudio : MonoBehaviour
{


    public static ScriptBossAudio Instance;

    [SerializeField] private AudioSource backgroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the current scene is not "ClubBoss" and this object exists in the scene
        if (scene.name != "ClubBoss")
        {
            // If so, destroy the whole object
            Destroy(gameObject);
        }
    }

}
