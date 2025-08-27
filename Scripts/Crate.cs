using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    private LevelObject levelObject;
    private float originalHP;

    private Transform top;

    public bool topIsOn;

    private void Start()
    {
        levelObject = GetComponent<LevelObject>();
        originalHP = levelObject.health;

        top = transform.Find("Top");

        if (!topIsOn) {
            top.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void Update()
    {
        if (topIsOn && levelObject.health < originalHP *0.75f ) {
            topIsOn = false;
            top.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
