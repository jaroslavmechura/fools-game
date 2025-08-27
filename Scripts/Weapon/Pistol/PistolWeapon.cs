using UnityEngine;

public class PistolWeapon : Weapon
{ 


    private void LateUpdate()
    {
        animator.SetBool("isWallClip", isWallClip);
    }

    public override void WeaponInputActions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isWallClip) Fire(false);

        }
        else if (Input.GetKey(KeyCode.R))
        {
            Reload();
        }
    }

}
