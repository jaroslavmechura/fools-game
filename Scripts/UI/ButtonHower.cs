using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonHower : MonoBehaviour
{
    private Button button;
    private Vector3 originalScale;
    private float targetScaleFactor = 1.4f;
    private float transitionDuration = 0.2f;
    private bool isHovered = false;

    public AudioSource audio;

    void Start()
    {
        button = GetComponent<Button>();

        originalScale = transform.localScale;

        if (button.GetComponent<EventTrigger>() == null)
        {
            button.gameObject.AddComponent<EventTrigger>();
        }

        AddEventTrigger(OnPointerEnter, EventTriggerType.PointerEnter);
        AddEventTrigger(OnPointerExit, EventTriggerType.PointerExit);
    }

    private void AddEventTrigger(UnityAction<BaseEventData> action, EventTriggerType eventType)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener((data) => action.Invoke((BaseEventData)data));
        button.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    private void OnPointerEnter(BaseEventData eventData)
    {

        if (!isHovered)
        {
            StartCoroutine(SmoothScale(originalScale * targetScaleFactor));
            audio.Play();
            isHovered = true;
        }
    }

    private void OnPointerExit(BaseEventData eventData)
    {
        if (isHovered)
        {
            StartCoroutine(SmoothScale(originalScale));
            isHovered = false;
        }
    }

    private IEnumerator SmoothScale(Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < transitionDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / transitionDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    public void HowerOff() {
        if (isHovered)
        {
            StartCoroutine(SmoothScale(originalScale));
            isHovered = false;
        }
    }
}
