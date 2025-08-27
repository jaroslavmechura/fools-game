using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponWallClip : MonoBehaviour
{
    [Header("RaycastSettings")]
    [SerializeField] private float range;
    [SerializeField] private LayerMask layerMask;

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Weapon weapon;

    [Header("Debug")]
    [SerializeField] private bool debugDrawEnabled = true;

    public bool isWallClip = false;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        weapon = playerInput.currWeapon;
    }

    private void Update()
    {
        weapon = playerInput.currWeapon;
        range = weapon == null ? 0 : weapon.wallClipRange;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, range, layerMask);

        if (hit.collider != null)
        {
            isWallClip = true;
        }
        else
        {
            isWallClip = false;
        }

        // Set weapon's isWallClip bool
        if (weapon != null)
        {
            weapon.isWallClip = isWallClip;
        }

        if (debugDrawEnabled)
        {
            DrawRaycastDebug(); // Draw debug raycast
        }
    }

    private void DrawRaycastDebug()
    {
        // Draw debug raycast
        Debug.DrawRay(transform.position, transform.up * range, isWallClip ? Color.red : Color.white);
    }

}
