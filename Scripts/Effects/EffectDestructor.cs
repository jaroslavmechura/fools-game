using UnityEngine;

public class EffectDestructor : MonoBehaviour
{
    [SerializeField] private float destroyTimer = 1f;


    private void Start()
    {
        Destroy(gameObject, destroyTimer);
    }


    public void RotateSelf()
    {
        Vector3 toWeapon = GameObject.FindWithTag("Player").transform.position - transform.position;
        float targetRotationX = Mathf.Atan2(toWeapon.y, -toWeapon.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(targetRotationX, 90.0f, 0.0f) * transform.rotation;

        transform.Translate(Vector3.forward * 0.75f);
    }
}
