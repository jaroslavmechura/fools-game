using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WardrobeMenu : MonoBehaviour
{
    [Header("--- Torso ---")]
    [SerializeField] private List<Sprite> torsoSprites;

    [Header("--- Shoe ---")]
    [SerializeField] private List<Sprite> rShoeSprites;
    [SerializeField] private List<Sprite> lShoeSprites;

    [Header("--- UI ---")]
    [SerializeField] private Image torsoImage;
    [SerializeField] private Image rShoeImage;
    [SerializeField] private Image lShoeImage;

    [Header("--- References ---")]
    [SerializeField] private PlayerSkin skin;

    

    private void Start()
    {
        skin = GameObject.FindWithTag("Player").GetComponent<PlayerSkin>();
        torsoImage.sprite = torsoSprites[skin.torsoId];
        rShoeImage.sprite = rShoeSprites[skin.shoeId];
        lShoeImage.sprite = lShoeSprites[skin.shoeId];
    }

    private void Update()
    {
        torsoImage.sprite = torsoSprites[skin.torsoId];
        rShoeImage.sprite = rShoeSprites[skin.shoeId];
        lShoeImage.sprite = lShoeSprites[skin.shoeId];
    }

    public void TorsoPlus() { skin.SetTorsoIndex(+1); }
    public void TorsoMinus(){ skin.SetTorsoIndex(-1); }
    public void ShoePlus()  { skin.SetShoeIndex(+1); }
    public void ShoeMinus() { skin.SetShoeIndex(-1); }



}
