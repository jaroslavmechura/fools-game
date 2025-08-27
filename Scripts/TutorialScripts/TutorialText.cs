using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    public List<GameObject> texts;
    public List<bool> passedBools;
    public int index = 0;

    public float fadeInDuration = 1.0f;
    public float fadeOutDuration = 1.0f;

    private bool wPressed = false;
    private bool sPressed = false;
    private bool aPressed = false;
    private bool dPressed = false;

    private bool mouseMoved = false;
    private float mouseMoveStartTime = 0f;
    public float totalMouseMove = 0f;

    public float mouseMoveThreshold = 2.0f;

    void Start()
    {
        for (int i = 0; i < passedBools.Count; i++)
        {
            passedBools[i] = false;
        }

        foreach (GameObject text in texts)
        {
            text.SetActive(false);
        }

        ShowText();
    }
    void Update()
    {
        wPressed |= Input.GetKey(KeyCode.W);
        sPressed |= Input.GetKey(KeyCode.S);
        aPressed |= Input.GetKey(KeyCode.A);
        dPressed |= Input.GetKey(KeyCode.D);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(mouseX) > 0 || Mathf.Abs(mouseY) > 0)
        {
            totalMouseMove += Mathf.Sqrt(mouseX * mouseX + mouseY * mouseY);
            mouseMoveStartTime = Time.time;
        }

        mouseMoved = totalMouseMove > mouseMoveThreshold;

        if (index == 0 && (wPressed && sPressed && aPressed && dPressed))
        {
            Next();
            totalMouseMove = Mathf.Sqrt(mouseX * mouseX + mouseY * mouseY);
            mouseMoveThreshold = totalMouseMove + 50;
        }
        else if (index == 1 && mouseMoved)
        {
            Next();
        }
        else if (index == 2 && Input.GetKey(KeyCode.LeftShift))
        {
            Next();
        }
        else if (index == 3 && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)))
        {
            Next();
        }
        else if (index == 4 && (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.F)))
        {
            Next();
        }
        else if (index == 5 && Input.GetMouseButton(1))
        {
            Next();
        }
        else if (index == 6 && Input.GetMouseButton(0))
        {
            Next();
        }
        else if (index == 7 && Input.GetKey(KeyCode.R))
        {
            Next();
        }
        else if (index == 8 && Input.GetKey(KeyCode.LeftControl))
        {
            Next();
        }
    }

    void Next()
    {
        index++;

        if (index < texts.Count)
        {
            StartCoroutine(FadeTextOutAndIn());
        }
        else
        {
            Debug.Log("Tutorial Complete!");
        }
    }

    IEnumerator FadeTextOutAndIn()
    {
        StartCoroutine(FadeText(texts[index - 1].GetComponent<TextMeshProUGUI>(), 1f, 0f, fadeOutDuration));

        yield return new WaitForSeconds(fadeOutDuration);

        texts[index - 1].SetActive(false);

        ShowText();
    }

    void ShowText()
    {
        texts[index].SetActive(true);

        StartCoroutine(FadeText(texts[index].GetComponent<TextMeshProUGUI>(), 0f, 1f, fadeInDuration));

        passedBools[index] = true;
    }

    IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
    {
        Color color = text.color;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            text.color = color;

            yield return null;
        }
    }
}
