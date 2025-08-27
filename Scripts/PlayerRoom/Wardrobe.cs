using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Wardrobe : MonoBehaviour
{
    [Header("--- Interactivity ---")]
    [SerializeField] private GameObject openText;
    [SerializeField] private GameObject doorObject;
    [SerializeField] private bool isOpened = false;

    [Header("--- Rotation ---")]
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float rotationSpeed = 5f;


    [Header("--- UI ---")]
    [SerializeField] private GameObject wardrobeMenu;


    private GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");

        openText.SetActive(false);
        wardrobeMenu.SetActive(false);

        initialRotation = doorObject.transform.rotation;
        targetRotation = doorObject.transform.rotation * Quaternion.Euler(0, 0, 30f);
    }

    private void Update()
    {
        if (!isOpened)
        {
            if (openText.active)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isOpened = true;
                    openText.GetComponent<TextMeshPro>().text = "Wardrobe [SPACE]";

                    wardrobeMenu.SetActive(true);
                    Time.timeScale = 0.0f;

                    PlayerInput inputScript = player.GetComponent<PlayerInput>();
                    if (inputScript != null) {
                        inputScript.enabled = false;
                    }
                    
                }
            }
            doorObject.transform.rotation = Quaternion.Lerp(doorObject.transform.rotation, initialRotation, Time.deltaTime * rotationSpeed);
        }

        else {

            if (Input.GetKeyDown(KeyCode.Space)) {

                isOpened = false;
                openText.GetComponent<TextMeshPro>().text = "Wardrobe [SPACE]";

                wardrobeMenu.SetActive(false);

                Time.timeScale = 1.0f;


                PlayerInput inputScript = player.GetComponent<PlayerInput>();
                if (inputScript != null)
                {
                    inputScript.enabled = true;
                }
            }
            doorObject.transform.rotation = Quaternion.Lerp(doorObject.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            openText.SetActive(true);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            openText.SetActive(false);

            isOpened = false;
            openText.GetComponent<TextMeshPro>().text = "Wardrobe [SPACE]";

            wardrobeMenu.SetActive(false);

            Time.timeScale = 1.0f;
        }
    }


}
