using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class PulseLight : MonoBehaviour
{
    public Color color1;
    public Color color2;
    public float speed = 1f;
    public bool isSmooth = false;
    public float smoothTransitionTime = 1f;

    private Light2D lightComponent;
    private float timeElapsed = 0f;
    private bool transitioningToColor2 = false;

    private void Start()
    {
        lightComponent = GetComponent<Light2D>();
        lightComponent.color = color1;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime * speed;

        if (isSmooth)
        {
            float t = Mathf.PingPong(timeElapsed / smoothTransitionTime, 1f); // Ping-pong time within smooth transition time
            lightComponent.color = Color.Lerp(color1, color2, t);
        }
        else
        {
            if (timeElapsed >= speed)
            {
                if (transitioningToColor2)
                {
                    lightComponent.color = color2;
                }
                else
                {
                    lightComponent.color = color1;
                }

                transitioningToColor2 = !transitioningToColor2;
                timeElapsed = 0f;
            }
        }
    }
}
