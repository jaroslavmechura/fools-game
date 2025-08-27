using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScriptItemTutorial : MonoBehaviour
{
    [SerializeField] private Door doorsToUnlock;

    [SerializeField] private GameObject pickTutorial;
    [SerializeField] private GameObject throwTutorial;

    [SerializeField] private bool stagePick = false;
    [SerializeField] private bool stageThrow = false;

    [SerializeField] private GameObject mouseLookUI1;
    [SerializeField] private GameObject mouseLookUI2;


    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float delayBeforeHide = 1.0f;

    [SerializeField] private GameObject playerObj;

    void Start()
    {
        mouseLookUI1 = pickTutorial.transform.Find("MouseRight").gameObject;
        mouseLookUI2 = throwTutorial.transform.Find("MouseRight").gameObject;

        playerObj = GameObject.FindWithTag("Player");

        pickTutorial.SetActive(false);
        throwTutorial.SetActive(false);

    }

    void Update()
    {
        if (stagePick)
        {
            FistsWeapon fists =  playerObj.GetComponent<PlayerInput>().currWeapon.GetComponent<FistsWeapon>();

            if (fists != null) {
                if (fists.isHoldingSomething) {
                    stagePick = false;
                    stageThrow = true;
                    KeyComplete(mouseLookUI1);
                    HideTutorial(pickTutorial, throwTutorial);
                }
            }

        }
        if (stageThrow)
        {
            FistsWeapon fists = playerObj.GetComponent<PlayerInput>().currWeapon.GetComponent<FistsWeapon>();

            if (fists != null)
            {
                if (!fists.isHoldingSomething)
                {
                    stagePick = false;
                    stageThrow = false;
                    KeyComplete(mouseLookUI2);
                    HideTutorial(throwTutorial, null);
                    UnlockProgress();
                }
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
        if (collision.CompareTag("Player")) {
            if (stagePick == false) {
                stagePick = true;
                ShowTutorial(pickTutorial);
            }
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
       
    }
}
