using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public AudioSource audioButtonClick;

    public void OnTogglePauseMenuButtonClick()
    {
 
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerInput playerController = player.GetComponent<PlayerInput>();
            if (playerController != null)
            {
                playerController.TogglePauseMenu();
            }
        }
    }

    public void OnLoadMainMenuButtonClick()
    {
        audioButtonClick.Play();
        GameObject levelManager = GameObject.FindWithTag("LevelController");
        if (levelManager != null)
        {
            LevelManager levelManagerScript = levelManager.GetComponent<LevelManager>();
            if (levelManagerScript != null)
            {
                Time.timeScale = 1f;
                levelManagerScript.LoadNextRoom("MainMenu");
            }
        }

    }

 
}
