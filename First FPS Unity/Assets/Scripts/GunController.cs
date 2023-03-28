using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public KeyCode toggleFire = KeyCode.V;
    public KeyCode reload = KeyCode.r;

    private bool readyToShoot = true;
    private bool auto = true;

    [Header("Gun Data")]
    public float fireRate;
    public float bulletSpeed;
    public int ammo = 12;
    public float ejectVelocity;
    public float recoil;
    public float volume;
    
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public Transform bulletSpan;
    public Transform casingSpawn;


    private void Update()
    {
        if (Input.GetKeyDown(toggleFire))
        {
            auto = !auto;
        }

        if (Input.GetKeyUp(shoot))
        {
            resetShoot();
        }

        if (Input.GetKey(shoot) && readyToShoot && auto)
        {
            readyToShoot = false;
            spawnBullet();
            Invoke(nameof(resetShoot), (60f / fireRate));
            
        } else if (Input.GetKey(shoot) && readyToShoot && !auto) {
            spawnBullet();
            readyToShoot = false;
        }
    }

    private void spawnBullet()
    {
        if (ammo > 0) {
             var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawn.forward * bulletSpeed;
            var casing = Instantiate(casingPrefab, casingSpawn.position, casingSpawn.rotation);
            casing.GetComponent<Rigidbody>().velocity = casingSpawn.right * ejectVelocity;
            ammo--;
        } else {
            
        }
    }

    private void resetShoot()
    {
        readyToShoot = true;
    }
}
