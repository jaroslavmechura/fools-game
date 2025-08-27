using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    [SerializeField] private bool isOpened;
    [SerializeField] private float smoothSpeed = 2f; // Smooth opening/closing speed

    [SerializeField] private Vector3 initPos;
    [SerializeField] private Vector3 openedPos;

    public AudioClip openSound;
    public AudioClip closeSound;
    private AudioSource audioSource;

    private Transform selfTransform;

    [SerializeField] Transform objectHitTransform;

    private void Start()
    {
        selfTransform = transform;

        initPos = selfTransform.localPosition;
        openedPos = initPos - new Vector3(selfTransform.localScale.x * 4, 0, 0);

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isOpened)
        {
            selfTransform.localPosition = Vector3.Lerp(selfTransform.localPosition, openedPos, smoothSpeed * Time.deltaTime);
            objectHitTransform.localPosition = Vector3.Lerp(selfTransform.localPosition, openedPos, smoothSpeed * Time.deltaTime);
        }
        else
        {
            selfTransform.localPosition = Vector3.Lerp(selfTransform.localPosition, initPos, smoothSpeed * Time.deltaTime);
            objectHitTransform.localPosition = Vector3.Lerp(selfTransform.localPosition, initPos, smoothSpeed * Time.deltaTime);
        }
    }



    public void Open() {
        if (!isOpened && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        isOpened = true;
    }
    public void Close() {
        if (isOpened && closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }

        isOpened = false;
    }
}
