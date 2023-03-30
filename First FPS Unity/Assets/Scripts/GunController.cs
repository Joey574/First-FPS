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

    private UnityEngine.Vector3 defaultPos;
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
    public float chargeOpenTime;
    public bool canAuto;
    public float volume;
    public bool hasScope;
    public float zoomMult;
    public float reloadZoom;
    public float defaultFOV;
    public float horizontalAimAdjust;
    public float verticalAimAdjust;
    public float lowerRand;
    public float upperRand;
    public float pauseSlideAdjust;

    [Header("Objects")]
    public GameObject gun;
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public Transform hand;
    public Transform bulletSpawn;
    public Transform casingSpawn;

    private GameObject camera;
    private PlayerCam cameraScript;
    private Animation anim;

    private void Awake()
    {
        camera = GameObject.Find("PlayerCam");
        cameraScript = camera.GetComponent <PlayerCam> ();
        anim = gun.GetComponent<Animation>();
        defaultPos = hand.localPosition;
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

        if (Input.GetKey(aim))
        {
           aimHandler();
        }

        if (Input.GetKeyUp(aim))
        {
            resetAim();
        }

        fireHandler();
       
    }
    
    private void aimHandler()
    {
         if (!reloading)
            {
                hand.localPosition = new UnityEngine.Vector3(0 - horizontalAimAdjust, 0 - verticalAimAdjust, defaultPos.z);
                if (hasScope)
                {
                    cameraScript.setFOV(defaultFOV / zoomMult);
                }
            } 
            else
            {
                cameraScript.setFOV(defaultFOV / reloadZoom);
            }
    }

    private void fireHandler()
    {
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
            if (ammo == 0)
            {
                Invoke(nameof(pauseSlideAction), chargeOpenTime + pauseSlideAdjust);
            }

            cameraScript.addRecoil(verticalRecoil, horizontalRecoil);

            Invoke(nameof(spawnCasing), chargeOpenTime);
        } else {
            
        }
    }

    private void spawnCasing()
    {
        float random = Random.Range(lowerRand, upperRand);   
        var casing = Instantiate(casingPrefab, casingSpawn.position, casingSpawn.rotation);
        casing.GetComponent<Rigidbody>().velocity = casingSpawn.up * ejectVelocity;
        casing.GetComponent<Rigidbody>().AddTorque(new Vector3(random, random, 0));
    }

    private void resetShoot()
    {
        StartCoroutine(WaitForKickBackAnimation(anim));
    }

    private void startReload()
    {
        Invoke(nameof(setAmmoToMax), reloadTime);

    }

    private void setAmmoToMax()
    {
        if (ammo == 0)
        {
            anim.Play("closeAction");
        }
        ammo = maxAmmo;
        StartCoroutine(WaitForReloadAnimation(anim));
    }

    private void pauseSlideAction()
    {
        anim.Stop("kickBack");
    }

    private void resetAim()
    {
        cameraScript.setFOV(defaultFOV);
        hand.localPosition = defaultPos;
    }

    private IEnumerator WaitForKickBackAnimation(Animation animation)
    {
        while(animation.isPlaying)
        {
            yield return null;
        }
        readyToShoot = true;
    }

    private IEnumerator WaitForReloadAnimation(Animation animaton)
    {
        while (animaton.isPlaying)
        {
            yield return null;
        }
        reloading = false;
    }
}
