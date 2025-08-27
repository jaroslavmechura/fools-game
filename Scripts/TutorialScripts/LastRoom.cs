using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LastRoom : MonoBehaviour
{

    [SerializeField] private Door doorsToUnlock1;
    [SerializeField] private Door doorsToUnlock2;

    [SerializeField] private GameObject bulletTimeTutorial;

    [SerializeField] private bool stageBulletTime;

    [SerializeField] private GameObject ctrlUI;


    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float delayBeforeHide = 1.0f;

    [SerializeField] private GameObject playerObj;
    
    void Start()
    {


        ctrlUI = bulletTimeTutorial.transform.Find("Button").gameObject;

        playerObj = GameObject.FindWithTag("Player");

        bulletTimeTutorial.SetActive(false);
    }

    void Update()
    {


        if (stageBulletTime)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                stageBulletTime = false;

                KeyComplete(ctrlUI);
                HideTutorial(bulletTimeTutorial, null);
                UnlockProgress();
            }

            if (Input.GetKey(KeyCode.F))
            {
                UnlockArena();
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

    public void UnlockArena() {
        doorsToUnlock1.isLocked = false;
    }

    public void UnlockProgress()
    {
        doorsToUnlock2.isLocked = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (stageBulletTime == false)
            {
                stageBulletTime = true;
                ShowTutorial(bulletTimeTutorial);
            }
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

    }
}
