using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MeleeWeaponsRoom : MonoBehaviour
{
    [SerializeField] private Door doorsToUnlock;

    [SerializeField] private GameObject hitTutorial;
    [SerializeField] private GameObject counterTutorial;
    [SerializeField] private GameObject throwTutorial;

    [SerializeField] private bool stageHit = false;
    [SerializeField] private bool stageCounter = false;
    [SerializeField] private bool stageThrow = false;

    [SerializeField] private GameObject mouseUI;
    [SerializeField] private GameObject throwUI;

    [SerializeField] private float currCounterCount = 0;
    [SerializeField] private float neededCounterCount = 3;

    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float delayBeforeHide = 1.0f;

    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject dummyObj;
    [SerializeField] private GameObject katanaDummyObj;
    [SerializeField] private Transform katanaSpawn;

    void Start()
    {
        mouseUI = hitTutorial.transform.Find("MouseButton").gameObject;
        throwUI = throwTutorial.transform.Find("X").gameObject;

        playerObj = GameObject.FindWithTag("Player");

        hitTutorial.SetActive(false);
        counterTutorial.SetActive(false);
        throwTutorial.SetActive(false);
    }

 
    void Update()
    {
        if (stageHit)
        {
            if (dummyObj.GetComponent<Enemy>().wasSliced)
            {
                stageHit = false;
                stageThrow = true;
                KeyComplete(mouseUI);
                HideTutorial(hitTutorial, throwTutorial);
            }

        }
        if (stageCounter)
        {
            if (currCounterCount >= neededCounterCount)
            {
                stageCounter = false;
                stageThrow = true;
                HideTutorial(counterTutorial, throwTutorial);
                dummyObj.SetActive(false);
            }
            counterTutorial.GetComponent<TextMeshProUGUI>().text = "Attack enemy at the same time as he attacks. Succesfull counters: " + ((int)currCounterCount).ToString() + " / " + ((int)neededCounterCount).ToString();

        }
        if (stageThrow) {
            if (Input.GetKey(KeyCode.X)) {
                stageThrow = false;
                KeyComplete(throwUI);
                HideTutorial(throwTutorial, null);
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

    public void AddMeleeCounter() {
        if (!stageCounter) return;
        currCounterCount += 0.5f;
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
            if (stageHit == false)
            {
                stageHit = true;
                ShowTutorial(hitTutorial);
            }
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

    }
}
