using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weed : MonoBehaviour
{
    [Header("--- Leaves ---")]
    [SerializeField] private List<Transform> leavesLevels;

    [Header("--- Physics ---")]
    [SerializeField] private List<Rigidbody2D> rbs;
    [SerializeField] private List<DistanceJoint2D> joints;
    [SerializeField] private float maxDistanceFromPlayer;

    [SerializeField] private Transform playerTrans;
    [SerializeField] private Transform selfTrans;
    [SerializeField] private float distanceToPlayer;

    [SerializeField] private bool physOn = false;

    private void Start()
    {
        foreach (Transform leaf in leavesLevels)
        {

            leaf.localRotation = Quaternion.Euler(new Vector3(leaf.localRotation.eulerAngles.x, leaf.localRotation.eulerAngles.y, Random.Range(0f, 360f)));
        }

        playerTrans = GameObject.FindWithTag("Player").transform;
        selfTrans = gameObject.transform;

        foreach (DistanceJoint2D dj in joints)
        {
            dj.enabled = false;
            
        }

        foreach (Rigidbody2D rb in rbs)
        {
            rb.simulated = false;
        }
    }

    private void Update()
    {
        distanceToPlayer = Vector3.Distance(playerTrans.position, selfTrans.position);

        if (distanceToPlayer < maxDistanceFromPlayer && !physOn)
        {
            physOn = true;

            foreach (DistanceJoint2D dj in joints)
            {
                dj.enabled = true;
            }

            foreach (Rigidbody2D rb in rbs)
            {
                rb.simulated = true;
            }
        }
        else if (distanceToPlayer >= maxDistanceFromPlayer && physOn)
        {
            physOn = false;

            foreach (DistanceJoint2D dj in joints)
            {
                dj.enabled = false;
            }

            foreach (Rigidbody2D rb in rbs)
            {
                rb.simulated = false;
            }
        }
    }
}

    

