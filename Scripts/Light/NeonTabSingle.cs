using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NeonTabSingle : MonoBehaviour
{


    [SerializeField] private float waveSpeed = 1.0f;

    [SerializeField] private Light2D light1;


    private void Start()
    {
        light1 = GetComponent<Light2D>();
        
    }

    private void Update()
    {
        float time = Time.time * waveSpeed;
        float redValue = Mathf.Sin(time) * 0.5f + 0.5f;
        float greenValue = Mathf.Sin(time + Mathf.PI * 2 / 3) * 0.5f + 0.5f;
        float blueValue = Mathf.Sin(time + Mathf.PI * 4 / 3) * 0.5f + 0.5f;

        Color newColor = new Color(redValue, greenValue, blueValue);
        light1.color = newColor;
   
    }
}
