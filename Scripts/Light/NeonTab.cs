using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightColorWave : MonoBehaviour
{
    [SerializeField] private GameObject lightObject1;
    [SerializeField] private GameObject lightObject2;

    [SerializeField] private float waveSpeed = 1.0f;

    [SerializeField] private Light2D light1;
    [SerializeField] private Light2D light2;

    private void Start()
    {
        light1 = lightObject1.GetComponent<Light2D>();
        light2 = lightObject2.GetComponent<Light2D>();
    }

    private void Update()
    {
        float time = Time.time * waveSpeed;
        float redValue = Mathf.Sin(time) * 0.5f + 0.5f;
        float greenValue = Mathf.Sin(time + Mathf.PI * 2 / 3) * 0.5f + 0.5f;
        float blueValue = Mathf.Sin(time + Mathf.PI * 4 / 3) * 0.5f + 0.5f;

        Color newColor = new Color(redValue, greenValue, blueValue);
        light1.color = newColor;
        light2.color = newColor;
    }
}
