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
    private bool aiming = false;

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
    public float verticalInaccuracy;
    public float horizontalInaccuracy;
    public float reloadTime;
    public float volume;
    public float zoomMult;
    public bool hasScope;
    public bool canAuto;
    public bool aimToggle;
    public bool left = false;

    [Header("Objects")]
    public GameObject gun;
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public Transform bulletSpawn;
    public Transform casingSpawn;

    [Header("Adjustments")]
    public float defaultFOV;
    public float reloadZoom = 5.0f;
    public float chargeOpenTime;
    public float horizontalAimAdjust;
    public float verticalAimAdjust;
    public float zAimAdjust;
    public float lowerRand;
    public float upperRand;

    private GameObject hand;
    private GameObject camera;
    private PlayerCam cameraScript;
    private Animation anim;

    private void Awake()
    {
        if (left)
        {
            hand = GameObject.Find("LeftHand");
        }
        else
        {
            hand = GameObject.Find("RightHand");
        }
        camera = GameObject.Find("PlayerCam");
        cameraScript = camera.GetComponent <PlayerCam> ();
        anim = gun.GetComponent<Animation>();
        defaultPos = hand.transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleFire))
        {
            auto = !auto;
        }

        if (Input.GetKeyDown(reload))
        {
            startReload();
        }
        
        if (aimToggle)
        {
            if (Input.GetKeyDown(aim) && !aiming)
            {
                aiming = true;
                aimHandler();
            }
            
            if (Input.GetKeyDown(aim) && aiming)
            {
                aiming = false;
                resetAim(); 
            }
        }
        else
        {
             if (Input.GetKey(aim))
            {
                aiming = true;
                aimHandler();
            }
            
            if (Input.GetKeyUp(aim))
            {
                aiming = false;
                resetAim();
            }
        } 
        
        if (!reloading)
        {
            fireHandler();
        } 
        else if (reloading && Input.GetKey(shoot)) // when reload anim added, swith to coroutine to wait before setting ammo, then stop coroutine
        {
        
        }
       
    }
    
    private void aimHandler()
    {
         if (!reloading)
            {
                hand.transform.localPosition = new UnityEngine.Vector3(0 - horizontalAimAdjust, 0 - verticalAimAdjust, defaultPos.z - zAimAdjust);
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
            
            if (Input.GetKeyDown(shoot) && readyToShoot)
            {
                spawnBullet();
                readyToShoot = false;
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
                Invoke(nameof(pauseSlideAction), chargeOpenTime);
            }

            cameraScript.addRecoil(verticalRecoil, horizontalRecoil);

            Invoke(nameof(spawnCasing), chargeOpenTime);
        } 
        else // play click sound 
        {
            
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
        reloading = true;
        hand.transform.localPosition = defaultPos;
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
        hand.transform.localPosition = defaultPos;
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

        if (!aiming)
        {
            cameraScript.setFOV(defaultFOV);
        }
        else
        {
            aimHandler();
        }
    }
}
