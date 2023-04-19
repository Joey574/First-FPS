using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybindsController : MonoBehaviour
{
    [Header("Movement Keybinds")]
    public KeyCode jump = KeyCode.Space;

    [Header("Fire Keybinds")]
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public KeyCode reload = KeyCode.R;
    public KeyCode toggleFire = KeyCode.V;
    public bool aimToggle = false;
    [Header("Item Keybinds")]
    public KeyCode pickUp = KeyCode.F;
    public KeyCode drop = KeyCode.G;

    public KeyCode Jump()
    {
        return jump;
    }

    public KeyCode Shoot()
    {
        return shoot;
    }

    public KeyCode Reload()
    {
        return reload;
    }

    public KeyCode Aim()
    {
        return aim;
    }

    public KeyCode ToggleFire()
    {
        return toggleFire;
    }

    public bool AimToggle()
    {
        return aimToggle;
    }

    public KeyCode PickUp()
    {
        return pickUp;
    }

    public KeyCode Drop()
    {
        return drop;
    }

}
