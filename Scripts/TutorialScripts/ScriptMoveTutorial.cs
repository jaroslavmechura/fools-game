using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScriptMoveTutorial : MonoBehaviour
{
    [SerializeField] private Door doorsToUnlock;

    [SerializeField] private GameObject moveTutorial;
    [SerializeField] private GameObject lookAroundTutorial;
    [SerializeField] private GameObject openDoorTutorial;

    [SerializeField] private bool wMoved = false;
    [SerializeField] private bool sMoved = false;
    [SerializeField] private bool aMoved = false;
    [SerializeField] private bool dMoved = false;

    [SerializeField] private float mouseDistance = 0.0f;
    [SerializeField] private float mouseDistanceCompleted = 50.0f;

    [SerializeField] private bool stageMove = false;
    [SerializeField] private bool stageLook = false;
    [SerializeField] private bool stageOpen = false;

    [SerializeField] private GameObject wKeyUI;
    [SerializeField] private GameObject sKeyUI;
    [SerializeField] private GameObject aKeyUI;
    [SerializeField] private GameObject dKeyUI;

    [SerializeField] private GameObject mouseLookUI;

    [SerializeField] private GameObject spaceKeyUI;

    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float delayBeforeHide = 1.0f;

    void Start()
    {
        wKeyUI = moveTutorial.transform.Find("W").gameObject;
        sKeyUI = moveTutorial.transform.Find("S").gameObject;
        aKeyUI = moveTutorial.transform.Find("A").gameObject;
        dKeyUI = moveTutorial.transform.Find("D").gameObject;

        mouseLookUI = lookAroundTutorial.transform.Find("Mouse").gameObject;

        spaceKeyUI = openDoorTutorial.transform.Find("Space").gameObject;

        moveTutorial.SetActive(false);
        lookAroundTutorial.SetActive(false);
        openDoorTutorial.SetActive(false);

        ShowTutorial(moveTutorial);
        stageMove = true;
    }

    void Update()
    {
        if (stageMove)
        {
            if (Input.GetKey(KeyCode.W) && !wMoved)
            {
                wMoved = true;
                KeyComplete(wKeyUI);
            }
            if (Input.GetKey(KeyCode.S) && !sMoved)
            {
                sMoved = true;
                KeyComplete(sKeyUI);
            }
            if (Input.GetKey(KeyCode.A) && !aMoved)
            {
                aMoved = true;
                KeyComplete(aKeyUI);
            }
            if (Input.GetKey(KeyCode.D) && !dMoved)
            {
                dMoved = true;
                KeyComplete(dKeyUI);
            }

            if (wMoved && sMoved && aMoved && dMoved)
            {
                stageMove = false;
                stageLook = true;
                HideTutorial(moveTutorial, lookAroundTutorial);
            }
        }
        if (stageLook)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            mouseDistance += Mathf.Abs(mouseX) + Mathf.Abs(mouseY);

            if (mouseDistance > mouseDistanceCompleted)
            {
                stageLook = false;
                stageOpen = true;
                KeyComplete(mouseLookUI);
                HideTutorial(lookAroundTutorial, openDoorTutorial);
            }
        }
        if (stageOpen)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                KeyComplete(spaceKeyUI);
                StartCoroutine(HideWithDelay(openDoorTutorial, null));
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
}
