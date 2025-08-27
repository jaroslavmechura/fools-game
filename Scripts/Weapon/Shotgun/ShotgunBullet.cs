using UnityEngine;

public class ShotgunBullet : Bullet
{
    private float initialSpeed;
    private Vector2 localUp = Vector2.up; // Local "up" direction

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialSpeed = speed;
        rb.velocity = transform.TransformDirection(localUp) * speed; // Initial velocity in local "up" direction
    }

    // Update is called once per frame
    void Update()
    {
        // Gradually slow down the bullet using linear drag
        rb.drag = 1.0f + (speed / initialSpeed);
        speed -= Time.deltaTime * 1f;

        if (speed <= initialSpeed - 0.7)
        {
            Destroy(gameObject);
        }
    }
}
