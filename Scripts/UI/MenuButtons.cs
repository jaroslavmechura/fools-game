using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{

    [SerializeField] private AudioSource audioButtonClick;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject sceneButton1;
    [SerializeField] private GameObject sceneButton2;
    [SerializeField] private float transitionDuration = 1.0f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float fadeInDuration = 0.5f;

    public GameObject controlsMenu;


    private bool controlsOpened = false;
    private void Start()
    {
        Time.timeScale = 1.0f;

        if (sceneButton2 != null)
        {
            sceneButton2.SetActive(!sceneButton2.activeSelf);
        }

        controlsMenu.SetActive(false);

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && controlsOpened)
        {
            ToggleControlsMenu();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleControlsMenu();
        }
    }


    public void ToggleControlsMenu()
    {
        if (controlsOpened)
        {
            controlsOpened = false;
            controlsMenu.SetActive(false);
        }
        else
        {
            controlsOpened = true;
            controlsMenu.SetActive(true);
        }
    }

    public void ToggleCameraAndSceneButtons()
    {
        audioButtonClick.Play();
        StartCoroutine(TransitionCoroutine());
        foreach (GameObject button in sceneButton1.transform) {
            ButtonHower script = button.GetComponent<ButtonHower>();

            if (script != null) {
                script.HowerOff();
            }
        }
    }

    private IEnumerator TransitionCoroutine()
    {
        StartCoroutine(FadeOutObject(sceneButton1, fadeOutDuration));
        StartCoroutine(FadeOutObject(sceneButton2, fadeOutDuration));

        yield return new WaitForSeconds(fadeOutDuration);

        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPosition = mainCamera.transform.position;
        targetPosition.y = (startPosition.y == 0) ? -23f : 0f;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;

        sceneButton1.SetActive(!sceneButton1.activeSelf);
        sceneButton2.SetActive(!sceneButton2.activeSelf);

        StartCoroutine(FadeInObject(sceneButton1, fadeInDuration));
        StartCoroutine(FadeInObject(sceneButton2, fadeInDuration));
    }

    private IEnumerator FadeOutObject(GameObject obj, float duration)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        float startAlpha = canvasGroup.alpha;
        float targetAlpha = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeInObject(GameObject obj, float duration)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        float startAlpha = canvasGroup.alpha;
        float targetAlpha = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }


    // MAIN MENU SCREEN

    public void LoadTutorialScene()
    {
        audioButtonClick.Play();
        //Invoke("Tutorial", 1f);

        controlsMenu.SetActive(true);
        controlsOpened = true;

    }

    public void EndGame()
    {
        audioButtonClick.Play();

        float delay = audioButtonClick.clip.length;
        StartCoroutine(QuitAfterDelay(delay));
        
    }



    // LEVEL SELECT SCREEN


    public void LoadClub1Scene()
    {
        audioButtonClick.Play();
        Invoke("Club", 1f);
    }

    public void LoadAfterClubScene()
    {
        audioButtonClick.Play();
        Invoke("PlayerRoom1", 1f);
    }
    public void LoadEmergencyRoom()
    {
        audioButtonClick.Play();
        Invoke("PlayerRoom2", 1f);
    }

    private IEnumerator QuitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Application.Quit();
    }

    void Club()
    {
        SceneManager.LoadScene("Club0New");
    }
    void PlayerRoom1()
    {
        SceneManager.LoadScene("PlayerRoom1");
    }
    void PlayerRoom2()
    {
        SceneManager.LoadScene("PlayerRoom2");
    }
    void Tutorial()
    {
        SceneManager.LoadScene("TMain");
    }
}
