using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScriptMeleeHands: MonoBehaviour
{
    [SerializeField] private Door doorsToUnlock;

    [SerializeField] private GameObject punchTutorial;
    [SerializeField] private GameObject grabTutorial;
    [SerializeField] private GameObject throwTutorial;
    [SerializeField] private GameObject kickTutorial;

    [SerializeField] private bool stagePunch = false;
    [SerializeField] private bool stageGrab = false;
    [SerializeField] private bool stageThrow = false;
    [SerializeField] private bool stageKick = false;

    [SerializeField] private GameObject mouseLookUI1;
    [SerializeField] private GameObject mouseLookUI2;
    [SerializeField] private GameObject mouseLookUI3;
    [SerializeField] private GameObject keyKickUI;


    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float delayBeforeHide = 1.0f;

    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject dummyObj;

    void Start()
    {


        mouseLookUI1 = punchTutorial.transform.Find("MouseRight").gameObject;
        mouseLookUI2 = grabTutorial.transform.Find("MouseRight").gameObject;
        mouseLookUI3 = throwTutorial.transform.Find("MouseRight").gameObject;
        keyKickUI = kickTutorial.transform.Find("F").gameObject;

        playerObj = GameObject.FindWithTag("Player");

        punchTutorial.SetActive(false);
        grabTutorial.SetActive(false);
        throwTutorial.SetActive(false);
        kickTutorial.SetActive(false);

    }

    void Update()
    {
        if (stagePunch)
        {
            if (dummyObj.GetComponent<Enemy>().wasPunched) {
                stagePunch = false;
                stageGrab = true;
                KeyComplete(mouseLookUI1);
                HideTutorial(punchTutorial, grabTutorial);
            }

        }
        if (stageGrab)
        {
            if (dummyObj.GetComponent<Enemy>().isGrabbed)
            {
                stageGrab = false;
                stageThrow = true;
                KeyComplete(mouseLookUI2);
                HideTutorial(grabTutorial, throwTutorial);
            }

        }
        if (stageThrow)
        {
            if (!dummyObj.GetComponent<Enemy>().isGrabbed)
            {
                stageThrow = false;
                stageKick = true;
                KeyComplete(mouseLookUI3);
                HideTutorial(throwTutorial, kickTutorial);
            }

        }
        if (stageKick)
        {
            if (dummyObj.GetComponent<Enemy>().wasKicked)
            {
                stageKick = false;
                
                KeyComplete(keyKickUI);
                HideTutorial(kickTutorial, null);
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
            if (stagePunch == false)
            {
                stagePunch = true;
                ShowTutorial(punchTutorial);
            }
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

    }
}
