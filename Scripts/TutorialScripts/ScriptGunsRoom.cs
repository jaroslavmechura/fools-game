using System.Collections;
using System.Data;

using UnityEngine;
using UnityEngine.UI;

public class ScriptGunsRoom : MonoBehaviour
{
    [SerializeField] private Door doorsToUnlock;

    [SerializeField] private GameObject pickTutorial;
    [SerializeField] private GameObject switchTutorial;
    [SerializeField] private GameObject reloadTutorial;
    [SerializeField] private GameObject shootTutorial;
    [SerializeField] private GameObject switchPickUpTutorial;
    [SerializeField] private GameObject throwTutorial;
    [SerializeField] private GameObject explainTutorial;

    [SerializeField] private bool stagePick = false;
    [SerializeField] private bool stageSwitch = false;
    [SerializeField] private bool stageReload = false;
    [SerializeField] private bool stageShoot = false;
    [SerializeField] private bool stageSwitchPickUp = false;
    [SerializeField] private bool stageThrow = false;

    [SerializeField] private GameObject key1UI;
    [SerializeField] private GameObject key2UI;
    [SerializeField] private GameObject key3UI;
    [SerializeField] private GameObject keyRUI;
    [SerializeField] private GameObject keyMouseUI;
    [SerializeField] private GameObject keyTUI;
    [SerializeField] private GameObject keyXUI;


    [SerializeField] private bool onePressed;
    [SerializeField] private bool twoPressed;
    [SerializeField] private bool threePressed;

    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float delayBeforeHide = 1.0f;

    [SerializeField] private GameObject playerObj;

    void Start()
    {

        key1UI = switchTutorial.transform.Find("One").gameObject; ;
        key2UI = switchTutorial.transform.Find("Two").gameObject; ;
        key3UI = switchTutorial.transform.Find("Three").gameObject; ;
        keyRUI = reloadTutorial.transform.Find("R").gameObject; ;
        keyMouseUI = shootTutorial.transform.Find("Mouse").gameObject; ;
        keyTUI = switchPickUpTutorial.transform.Find("T").gameObject; ;
        keyXUI = throwTutorial.transform.Find("X").gameObject; ;

        playerObj = GameObject.FindWithTag("Player");


        pickTutorial.SetActive(false);
        switchTutorial.SetActive(false);
        reloadTutorial.SetActive(false);
        shootTutorial.SetActive(false);
        switchPickUpTutorial.SetActive(false);
        throwTutorial.SetActive(false);
        explainTutorial.SetActive(false);


    }

    void Update()
    {
        if (stagePick)
        {
            PlayerInventory invent = playerObj.GetComponent<PlayerInventory>();
            if (invent.haveWeapon[1] && invent.haveWeapon[2])
            {
                stagePick = false;
                stageSwitch = true;
                
                HideTutorial(pickTutorial, switchTutorial);
            }

        }
        if (stageSwitch) {
            if (Input.GetKey(KeyCode.Alpha1)) {
                KeyComplete(key1UI);
                onePressed = true;
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                KeyComplete(key2UI);
                twoPressed = true;
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                KeyComplete(key3UI);
                threePressed = true;
            }

            if (onePressed && twoPressed && threePressed) {
                stageSwitch = false;
                stageReload = true;

                HideTutorial(switchTutorial, reloadTutorial);
            }
        }
        if (stageReload) {
            if (Input.GetKey(KeyCode.R) && playerObj.GetComponent<PlayerInventory>().equipedId != 0) {
                stageReload = false;
                stageShoot = true;
                KeyComplete(keyRUI);
                HideTutorial(reloadTutorial, shootTutorial);
            }
        }
        if (stageShoot) {
            if (Input.GetMouseButton(0) && playerObj.GetComponent<PlayerInventory>().equipedId != 0)
            {
                stageShoot = false;
                stageSwitchPickUp = true;
                KeyComplete(keyMouseUI);
                HideTutorial(shootTutorial, switchPickUpTutorial);
            }
        }
        if (stageSwitchPickUp) {
            if (Input.GetKey(KeyCode.T)) {
                stageSwitchPickUp = false;
                stageThrow = true;
                KeyComplete(keyTUI);
                HideTutorial(switchPickUpTutorial, throwTutorial);
            }
        }
        if (stageThrow)
        {
            if (Input.GetKey(KeyCode.X))
            {
                stageThrow = false;
                KeyComplete(keyXUI);
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
            if (stagePick == false)
            {
                stagePick = true;
                ShowTutorial(pickTutorial);
                ShowTutorial(explainTutorial);
            }
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

    }
}
