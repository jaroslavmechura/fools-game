
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("--- Smooth Player Follow ---")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;

    [Header("--- CameraShake ---")]
    // Variables for camera shake
    public float shakeDurationReset = 0f;
    private float shakeDuration = 0f;
    public float shakeMagnitude = 0.5f;
    public float dampingSpeed = 1.0f;
    public Vector3 initialPosition;

    [Header("--- CameraOffset ---")]

    public float maxOffsetDistance;

    // Store the initial Z position of the camera
    private float initialZPosition;
    private Transform playerTransform;

    private void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        if (target == null) Debug.LogError("Target not found.");

        // Save the initial position of the camera
        initialPosition = target.transform.position;
        initialZPosition = transform.position.z; // Store the initial Z position
    }

    private void FixedUpdate()
    {
        

        initialPosition = target.transform.position;

        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distanceToPlayer = Vector3.Distance(cursorPosition, target.position);


        // If the distance to the cursor exceeds the maximum offset distance, adjust it
       
        Vector3 directionToCursor = (cursorPosition - target.position).normalized;

        float ovalScaleFactor = 1.5f; // Adjust this factor to control the width of the oval on the x-axis
        directionToCursor.x *= ovalScaleFactor;

        cursorPosition = target.position + directionToCursor * (distanceToPlayer > maxOffsetDistance? maxOffsetDistance/2 : distanceToPlayer/2);

        Vector3 finalPosition;
        if (target.GetComponent<PlayerControler>().isDead)
        {
            finalPosition = new Vector3(target.position.x, target.position.y, initialZPosition);
        }
        else {
            finalPosition = new Vector3(cursorPosition.x, cursorPosition.y, initialZPosition);
        }
        // Smoothly follow the player
        


        transform.position = Vector3.Lerp(transform.position, finalPosition, smoothSpeed * Time.deltaTime);

        // Update camera shake effect
        if (shakeDuration > 0)
        {
            // Apply camera shake effect smoothly
            Vector3 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            Vector3 targetShakePosition = new Vector3(initialPosition.x + shakeOffset.x, initialPosition.y + shakeOffset.y, initialZPosition);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetShakePosition, Time.deltaTime * dampingSpeed);

            // Decrease shake duration
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            // Reset camera position if shake duration is over
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(initialPosition.x, initialPosition.y, initialZPosition), Time.deltaTime * dampingSpeed);
        }
    }

    // Public method for camera shake
    public void CameraShake(float shakeForce)
    {
        shakeDuration = shakeDurationReset;
        shakeMagnitude = shakeForce;
    }
}