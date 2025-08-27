using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistSkin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lUpper;
    [SerializeField] private SpriteRenderer lLower;
    [SerializeField] private SpriteRenderer rUpper;
    [SerializeField] private SpriteRenderer rLower;

    [SerializeField] private List<Sprite> upperList;
    [SerializeField] private List<Sprite> lowerList;


    public void SetHands(int index) { 
        lUpper.sprite = upperList[index];
        rUpper.sprite = upperList[index];

        lLower.sprite = lowerList[index];
        rLower.sprite = lowerList[index];
    }
}
