using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HairStationMenu : MonoBehaviour
{
    [Header("--- Hair ---")]
    [SerializeField] private List<Sprite> hairSprites;

    [Header("--- UI ---")]
    [SerializeField] private Image hairImage;


    [Header("--- References ---")]
    [SerializeField] private PlayerSkin skin;



    private void Start()
    {
        skin = GameObject.FindWithTag("Player").GetComponent<PlayerSkin>();
        hairImage.sprite = hairSprites[skin.headId];
    }

    private void Update()
    {
        hairImage.sprite = hairSprites[skin.headId];
    }

    public void HairPlus() { skin.SetHairIndex(+1); }
    public void HairMinus() { skin.SetHairIndex(-1); }

    public void HairColorPlus() { skin.SetHairColorIndex(+1); }
    public void HairColorMinus() { skin.SetHairColorIndex(-1); }




}
