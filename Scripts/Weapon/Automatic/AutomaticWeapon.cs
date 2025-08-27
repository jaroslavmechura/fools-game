 using UnityEngine;

public class AutomaticWeapon : Weapon
{
    private void LateUpdate()
    {
        animator.SetBool("isWallClip", isWallClip);
    }

    public override void WeaponInputActions()
    {
        if (Input.GetMouseButton(0))
        {
            if (!isWallClip)Fire(false);

        }
        else if (Input.GetKey(KeyCode.R))
        {
            Reload();
        }
    }
}
