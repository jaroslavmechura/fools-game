using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HairStation : MonoBehaviour
{
    [Header("--- Interactivity ---")]
    [SerializeField] private GameObject openText;
    [SerializeField] private bool isOpened = false;

    [Header("--- UI ---")]
    [SerializeField] private GameObject hairstationMenu;

    private void Start()
    {
        openText.SetActive(false);
        hairstationMenu.SetActive(false);
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
                    openText.GetComponent<TextMeshPro>().text = "Hairstation [SPACE]";

                    hairstationMenu.SetActive(true);
                    Time.timeScale = 1.0f;

                }
            }
        }

        else
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {

                isOpened = false;
                openText.GetComponent<TextMeshPro>().text = "Hairstation [SPACE]";

                hairstationMenu.SetActive(false);

                Time.timeScale = 1.0f;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            openText.SetActive(true);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            openText.SetActive(false);

            isOpened = false;
            openText.GetComponent<TextMeshPro>().text = "Hairstation [SPACE]";

            hairstationMenu.SetActive(false);

            Time.timeScale = 1.0f;
        }
    }

}
