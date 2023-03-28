using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public KeyCode toggleFire = KeyCode.V;

    private bool readyToShoot = true;
    private bool auto = true;

    [Header("Gun Data")]
    public float fireRate;
    public float bulletSpeed;
    public float ejectVelocity;

    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public Transform spawnPoint;
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
        var bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = spawnPoint.forward * bulletSpeed;
        var casing = Instantiate(casingPrefab, casingSpawn.position, casingSpawn.rotation);
        casing.GetComponent<Rigidbody>().velocity = casingSpawn.right * ejectVelocity;
    }

    private void resetShoot()
    {
        readyToShoot = true;
    }
}
