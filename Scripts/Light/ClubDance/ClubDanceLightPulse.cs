using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ClubDanceLightPulse : MonoBehaviour
{
    public List<Light2D> lightsReactingToBasses, lightsReactingToNB, lightsReactingToMiddles, lightsReactingToHighs;
    public float minRadius = 1f;
    public float maxRadius = 5f;
    public float lerpingSpeed = 1f; // Speed at which the radius changes

    public float innerDifference;

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
            float radius = Mathf.Lerp(maxRadius, minRadius, bassValue); // Interpolate radius between max and min based on bass frequency value
            SetLightRadius(light, radius);
        }

        // Pulsing effect for lights reacting to NB frequencies
        foreach (Light2D light in lightsReactingToNB)
        {
            float nbValue = ClubDanceLightsManager.instance.GetFrequenciesDiapason(7, 15, 100);
            float radius = Mathf.Lerp(maxRadius, minRadius, nbValue); // Interpolate radius between max and min based on NB frequency value
            SetLightRadius(light, radius);
        }

        // Pulsing effect for lights reacting to mid-range frequencies
        foreach (Light2D light in lightsReactingToMiddles)
        {
            float middleValue = ClubDanceLightsManager.instance.GetFrequenciesDiapason(15, 30, 200);
            float radius = Mathf.Lerp(maxRadius, minRadius, middleValue); // Interpolate radius between max and min based on middle frequency value
            SetLightRadius(light, radius );
        }

        // Pulsing effect for lights reacting to high frequencies
        foreach (Light2D light in lightsReactingToHighs)
        {
            float highValue = ClubDanceLightsManager.instance.GetFrequenciesDiapason(30, 32, 1000);
            float radius = Mathf.Lerp(maxRadius, minRadius, highValue); // Interpolate radius between max and min based on high frequency value
            SetLightRadius(light, radius );
        }
    }

    void SetLightRadius(Light2D light, float radius)
    {
        light.pointLightOuterRadius = Mathf.Lerp(light.pointLightOuterRadius, radius, lerpingSpeed * Time.deltaTime);
        light.pointLightInnerRadius = Mathf.Lerp(light.pointLightInnerRadius, radius * 0.5f, lerpingSpeed * Time.deltaTime) - innerDifference ; // Adjust the inner radius as well for a smoother transition
    }
}
