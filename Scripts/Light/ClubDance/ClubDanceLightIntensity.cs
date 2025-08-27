using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ClubDanceLightIntensity : MonoBehaviour
{
    public List<Light2D> lightsReactingToBasses, lightsReactingToNB, lightsReactingToMiddles, lightsReactingToHighs;
    public float minIntensity = 5f;
    public float maxIntensity = 5f;
    public float lerpingSpeed = 1f; // Speed at which the intensity changes

    private void FixedUpdate()
    {
        LightPulsing();
    }

    void LightPulsing()
    {
        // Pulsing effect for lights reacting to bass frequencies
        foreach (Light2D light in lightsReactingToBasses)
        {
            float bassValue = ClubDanceLightsManager.instance.GetFrequenciesDiapason(0, 7, 10);
            float intensity = Mathf.Lerp(maxIntensity, minIntensity, bassValue); // Interpolate intensity between 0 and 2 based on bass frequency value
            SetLightIntensity(light, intensity);
        }

        // Pulsing effect for lights reacting to NB frequencies
        foreach (Light2D light in lightsReactingToNB)
        {
            float nbValue = ClubDanceLightsManager.instance.GetFrequenciesDiapason(7, 15, 100);
            float intensity = Mathf.Lerp(maxIntensity, minIntensity, nbValue); // Interpolate intensity between 0 and 2 based on NB frequency value
            SetLightIntensity(light, intensity);
        }

        // Pulsing effect for lights reacting to mid-range frequencies
        foreach (Light2D light in lightsReactingToMiddles)
        {
            float middleValue = ClubDanceLightsManager.instance.GetFrequenciesDiapason(15, 30, 200);
            float intensity = Mathf.Lerp(maxIntensity, minIntensity, middleValue); // Interpolate intensity between 0 and 2 based on middle frequency value
            SetLightIntensity(light, intensity);
        }

        // Pulsing effect for lights reacting to high frequencies
        foreach (Light2D light in lightsReactingToHighs)
        {
            float highValue = ClubDanceLightsManager.instance.GetFrequenciesDiapason(30, 32, 1000);
            float intensity = Mathf.Lerp(maxIntensity, minIntensity, highValue); // Interpolate intensity between 0 and 2 based on high frequency value
            SetLightIntensity(light, intensity * 0.75f);
        }
    }

    void SetLightIntensity(Light2D light, float intensity)
    {
        light.intensity = Mathf.Lerp(light.intensity, intensity, lerpingSpeed * Time.deltaTime);
    }
}
