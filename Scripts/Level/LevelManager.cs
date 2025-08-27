using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Texture2D customCursor;

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
        ManageBackgroundMusic();
    }

    public void LoadNextRoom(string nextRoomName)
    {
        if (nextRoomName == "MainMenu" || nextRoomName == "Club1" || nextRoomName == "ClubBoss" || nextRoomName == "Trade2" || nextRoomName == "Farm2" || nextRoomName == "PlayerRoom" || nextRoomName == "PlayerRoom1")
        {
            Destroy(gameObject);
            Instance = null;
        }

        ManageBackgroundMusic();

        Cursor.visible = true;
        GameObject.FindWithTag("Player").GetComponent<PlayerLoader>().SavePlayerState();

        if (SceneManager.GetActiveScene().name == "PlayerRoom2") {
            Destroy(gameObject);
            Instance = null;
        }

        SceneManager.LoadScene(nextRoomName);
    }

    public void RestartCurrentLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void ManageBackgroundMusic()
    {
        // Check if the current scene is "ClubDance"
        bool isClubDanceScene = SceneManager.GetActiveScene().name == "ClubBoss";

        // If the current scene is "ClubDance", stop the background music
        if (isClubDanceScene)
        {
            if (backgroundMusic.isPlaying)
            {
                backgroundMusic.Stop();
            }
        }
        else
        {
            // If the current scene is not "ClubDance" and the background music is not playing, start playing it
            if (!backgroundMusic.isPlaying)
            {
                backgroundMusic.Play();
            }
        }

        if (SceneManager.GetActiveScene().name == "PlayerRoom") {
            backgroundMusic.Play();
        }
    }
}
