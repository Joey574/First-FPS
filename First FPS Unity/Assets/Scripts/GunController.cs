using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;

    private bool readyToShoot = true;

    [Header("Gun Data")]
    public float fireRate;
    public int bulletSpeed;


    private void Update()
    {
        if (Input.GetKey(shoot) && readyToShoot)
        {
            readyToShoot = false;

            spawnBullet();

            Invoke(nameof(resetShoot), (60f / fireRate));
        }
    }

    private void spawnBullet()
    {

    }

    private void resetShoot()
    {
        readyToShoot = true;
    }
}
