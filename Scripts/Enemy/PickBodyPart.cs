using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickBodyPart : MonoBehaviour
{
    [Header("Functionality")]
    [SerializeField] private bool isHeadSetter;
    [SerializeField] private bool isTorsoSetter;
    [SerializeField] private bool isHandSetter;
    [SerializeField] private bool isFootSetter;

    [Header("Heads")]
    [SerializeField] private List<Sprite> headSprites;
    [SerializeField] private List<float> headOffsets;

    [Header("Torsos")]
    [SerializeField] private List<Sprite> torsoSprites;
    [SerializeField] private List<float> torsoOffsets;

    [Header("Arms")]
    [SerializeField] private List<Sprite> armPartSprite;

    [Header("Foot")]
    [SerializeField] private List<Sprite> footPartSprite;


    [Header("Passing Values")]
    [SerializeField] int torsoIndex;

    private SpriteRenderer spriteRenderer;

    public int headID;

    [Header("--- Boss ---")]
    public bool isBoss = false;
    public int bossHeadId;
    public int bossTorsoId;

    public int SetHead()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        // pick random head
        int index = UnityEngine.Random.Range(0, headSprites.Count);

        if (isBoss) index = bossHeadId;

        // set head
        spriteRenderer.sprite = headSprites[index];

        // handle local y offset
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + headOffsets[index], transform.localPosition.z);

        return index;
    }

    public int SetTorso()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        // pick torso, set hands based on this index
        torsoIndex = UnityEngine.Random.Range(0, torsoSprites.Count);

        if (isBoss) torsoIndex = bossTorsoId;

        // set torso
        spriteRenderer.sprite = torsoSprites[torsoIndex];

        // handle local y offset
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + torsoOffsets[torsoIndex], transform.localPosition.z);

        gameObject.GetComponentInParent<HandSkinManger>().SetHandsParts(torsoIndex);
        return torsoIndex;
    }

    public int SetTorsoSpecifiedEnemy(int index)
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        // pick torso, set hands based on this index
        torsoIndex = index;

        if (isBoss) torsoIndex = bossTorsoId;

        // set torso
        spriteRenderer.sprite = torsoSprites[torsoIndex];

        // handle local y offset
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + torsoOffsets[torsoIndex], transform.localPosition.z);

        gameObject.GetComponentInParent<HandSkinManger>().SetHandsParts(torsoIndex);
        return torsoIndex;
    }

    public void SetArmPart(int index) {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        
        spriteRenderer.sprite = armPartSprite[index];
    }

    public void SetFootPart(int index)
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = footPartSprite[index];
    }

    public void SetHead(int index)
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = headSprites[index];

        transform.localPosition = new Vector3(transform.localPosition.x, headOffsets[index], transform.localPosition.z);
    }

    public void SetTorso(int index)
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = torsoSprites[index];

        transform.localPosition = new Vector3(transform.localPosition.x, -0.05f + torsoOffsets[torsoIndex], transform.localPosition.z);
    }
}
