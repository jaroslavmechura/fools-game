using System.Collections.Generic;

using UnityEngine;

public class BloodDestructor : EffectDestructor
{
    public List<GameObject> bloodPudle;

    private float targetRotationX;


    public void RotateSelf(bool isShotgun)
    {
        Vector3 toWeapon = GameObject.FindWithTag("Player").transform.position - transform.position;
        targetRotationX = Mathf.Atan2(toWeapon.y, -toWeapon.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(targetRotationX, 90.0f, 0.0f);

        SpawnPuddle(isShotgun, GameObject.FindWithTag("Player").transform.position);

    }

    public void RotateSelf(Vector3 source)
    {
        Vector3 toWeapon = source - transform.position;
        targetRotationX = Mathf.Atan2(toWeapon.y, -toWeapon.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(targetRotationX, 90.0f, 0.0f);

        SpawnPuddle(false, source);

    }


    private void SpawnPuddle(bool isShotgun, Vector3 source)
    {

        int first = Random.Range(0, bloodPudle.Count);
        int second = Random.Range(0, bloodPudle.Count);
        while (second == first) // Ensure second is different from first
        {
            second = Random.Range(0, bloodPudle.Count);
        }

        if (isShotgun)
        {
           
            Puddle(first, source);
            
        }
        else
        {
            Puddle(first, source);
            Puddle(second, source);
        }
    }


    private void Puddle(int index, Vector3 source)
    {
        // Instantiate the blood puddle
        GameObject puddleInstance = Instantiate(bloodPudle[index], transform.position, Quaternion.identity);

        // Calculate the direction vector from the source to the puddle's position
        Vector3 direction = source - puddleInstance.transform.position;

        // Calculate the angle in radians
        float angleRad = Mathf.Atan2(direction.y, direction.x);

        // Convert the angle to degrees
        float angleDeg = Mathf.Rad2Deg * angleRad;

        // Apply rotation only on the z-axis
        puddleInstance.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg + 90f);
    }
}
