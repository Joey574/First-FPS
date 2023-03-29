using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public KeyCode toggleFire = KeyCode.V;
    public KeyCode reload = KeyCode.R;

    private bool readyToShoot = true;
    private bool reloading = false;
    private bool auto = true;

    [Header("Gun Data")]
    public float fireRate;
    public float bulletSpeed;
    public int maxAmmo;
    public int ammo;
    public float ejectVelocity;
    public float verticalRecoil;
    public float horizontalRecoil;
    public float verticalSway;
    public float horizontalSway;
    public float reloadTime;
    public bool canAuto;
    public float volume;

    public GameObject gun;
    public GameObject camera;
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public Transform bulletSpawn;
    public Transform casingSpawn;

    private PlayerCam cameraRecoil;
    private Animation anim;

    private void Awake()
    {
        cameraRecoil = camera.GetComponent <PlayerCam> ();
        anim = gun.GetComponent<Animation>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleFire))
        {
            auto = !auto;
        }

        if (Input.GetKeyDown(reload))
        {
            reloading = true;
            startReload();
        }

        if (!reloading)
        {
            if (canAuto && auto)
            {
                if (Input.GetKey(shoot) && readyToShoot && auto)
                {
                    readyToShoot = false;
                    spawnBullet();
                    Invoke(nameof(resetShoot), (60f / fireRate));

                }
            }
            else
            {
                if (Input.GetKeyUp(shoot))
                {
                    resetShoot();
                }

                else if (Input.GetKey(shoot) && readyToShoot)
                {
                    spawnBullet();
                    readyToShoot = false;
                }
            }
        }
    }

    private void spawnBullet()
    {
        if (ammo > 0) {
            ammo--;
            var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawn.forward * bulletSpeed;
            anim.Play("kickBack");

            cameraRecoil.addRecoil(verticalRecoil, horizontalRecoil);

            Invoke(nameof(spawnCasing), 0.08f);
        } else {
            
        }
    }

    private void spawnCasing()
    {
        var casing = Instantiate(casingPrefab, casingSpawn.position, casingSpawn.rotation);
        casing.GetComponent<Rigidbody>().velocity = casingSpawn.up * ejectVelocity;
    }

    private void resetShoot()
    {
        StartCoroutine(WaitForAnimation(anim));
    }

    private void startReload()
    {
        Invoke(nameof(setAmmoToMax), reloadTime);
    }

    private void setAmmoToMax()
    {
        ammo = maxAmmo;
        reloading = false;
    }

    private IEnumerator WaitForAnimation(Animation animation)
    {
        while(animation.isPlaying)
        {
            yield return null;
        }
        readyToShoot = true;
    }
}
