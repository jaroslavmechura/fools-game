using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class ScriptAdvancedMovement : MonoBehaviour
{
    [SerializeField] private Door doorsToUnlock;

    [SerializeField] private GameObject dashTutorial;
    [SerializeField] private GameObject dodgeTutorial;
    [SerializeField] private GameObject explainTutorial;

    [SerializeField] private bool stageDash = false;
    [SerializeField] private bool stageDodge = false;

    [SerializeField] private GameObject keyDashUI;
    [SerializeField] private GameObject keyDodgeUI1;
    [SerializeField] private GameObject keyDodgeUI2;

    [SerializeField] private bool qPressed;
    [SerializeField] private bool ePressed;

    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float delayBeforeHide = 1.0f;

    [SerializeField] private GameObject playerObj;
     
    void Start()
    {
        keyDashUI = dashTutorial.transform.Find("Shift").gameObject;
        keyDodgeUI1 = dodgeTutorial.transform.Find("Q").gameObject;
        keyDodgeUI2 = dodgeTutorial.transform.Find("E").gameObject;

        playerObj = GameObject.FindWithTag("Player");



        dashTutorial.SetActive(false);
        dodgeTutorial.SetActive(false);
        explainTutorial.SetActive(false);


    }

    void Update()
    {
        if (stageDash)
        {
            if (Input.GetKey(KeyCode.LeftShift)) {
                stageDodge = true;
                stageDash = false;
                KeyComplete(keyDashUI);
                HideTutorial(dashTutorial, dodgeTutorial);
            }

        }
        if (stageDodge)
        {
            if (Input.GetKey(KeyCode.Q) && playerObj.GetComponent<PlayerInput>().isDodging) {
                KeyComplete(keyDodgeUI1);
                qPressed=true;
            }
            if (Input.GetKey(KeyCode.E) && playerObj.GetComponent<PlayerInput>().isDodging)
            {
                KeyComplete(keyDodgeUI2);
                ePressed=true;
            }

            if (ePressed && qPressed) {
                HideTutorial(dodgeTutorial, null);
                HideTutorial(explainTutorial, null);
                UnlockProgress();
            }

        }

    }

    private void KeyComplete(GameObject key)
    {
        StartCoroutine(KeyColorTransition(key));
    }

    IEnumerator KeyColorTransition(GameObject key)
    {
        float elapsedTime = 0.0f;
        Color startColor = key.GetComponent<Image>().color;
        Color endColor = Color.green;

        while (elapsedTime < fadeDuration)
        {
            key.GetComponent<Image>().color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void HideTutorial(GameObject hide, GameObject show)
    {
        StartCoroutine(HideWithDelay(hide, show));
    }

    IEnumerator ShowWithDelay(GameObject tutorial)
    {
        tutorial.SetActive(true);

        RectTransform rectTransform = tutorial.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = new Vector3(0.5f, 0.5f, 0.5f);

            float elapsedTime = 0.0f;
            while (elapsedTime < fadeDuration)
            {
                rectTransform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.localScale = endScale;
        }
    }

    IEnumerator HideWithDelay(GameObject hide, GameObject show)
    {
        RectTransform rectTransform = hide.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 endScale = Vector3.zero;

            float elapsedTime = 0.0f;
            while (elapsedTime < fadeDuration)
            {
                rectTransform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            hide.SetActive(false);

            rectTransform.localScale = endScale;

            if (show != null)
            {
                StartCoroutine(ShowWithDelay(show));
            }
        }
    }

    private void ShowTutorial(GameObject tutorial)
    {
        StartCoroutine(ShowWithDelay(tutorial));
    }

    public void UnlockProgress()
    {
        doorsToUnlock.isLocked = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (stageDash == false)
            {
                stageDash = true;
                ShowTutorial(dashTutorial);
                ShowTutorial(explainTutorial);
            }
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

    }
}
