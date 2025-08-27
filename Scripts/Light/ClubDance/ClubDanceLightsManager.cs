using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ClubDanceLightsManager : MonoBehaviour
{
    public static ClubDanceLightsManager instance;

    public float[] spectrumWidth;

    public AudioSource audioSource;

    private void Awake()
    {
        spectrumWidth = new float[64];

        audioSource = GameObject.FindWithTag("BossMusic").GetComponent<AudioSource>();

        instance = this;
    }

    void FixedUpdate()
    {
        audioSource.GetSpectrumData(spectrumWidth, 0, FFTWindow.Blackman);
    }

    public float GetFrequenciesDiapason(int start, int end, int mult) {
        return spectrumWidth.ToList().GetRange(start, end).Average() * mult;
    }

}
