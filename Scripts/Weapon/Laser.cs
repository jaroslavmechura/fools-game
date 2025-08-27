using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("--- Parameters ---")]
    public float laserLength = 10f;
    public LayerMask obstacleLayer;


    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, laserLength, obstacleLayer);

        Vector3 endPosition;

        if (hit.collider != null)
        {
            endPosition = hit.point;
        }
        else
        {
            endPosition = transform.position + transform.up * laserLength;
        }

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);
    }
}
