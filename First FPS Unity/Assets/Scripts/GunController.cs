using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private bool resetAimingToggle = false;

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

    private bool equipped;

    [Header("Objects")]
    public GameObject gun;
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public Transform bulletSpawn;
    public Transform casingSpawn;

    [Header("Adjustments")]
    public float defaultFOV;
    public float reloadZoom = 5.0f;
    public float defaultAimZoom = 1.5f;
    public float chargeOpenTime;
    public float hammerDelay;
    public float slideDelay;
    public float horizontalAimAdjust;
    public float verticalAimAdjust;
    public float zAimAdjust;
    public float lowerRandFB;
    public float lowerRandLR;
    public float lowerRandUD;
    public float upperRandFB;
    public float upperRandLR;
    public float upperRandUD;

    private int animLayer = 0;
    private GameObject hand;
    private GameObject camera;
    private PlayerCam cameraScript;
    private Animator anim;

    private void Awake()
    {
        hand = GameObject.Find("RightHand");
        camera = GameObject.Find("PlayerCam");
        cameraScript = camera.GetComponent <PlayerCam> ();
        anim = gun.GetComponent<Animator>();
        anim.SetFloat("hammerDelay", hammerDelay);
        anim.SetFloat("slideDelay", slideDelay);
        defaultPos = hand.transform.localPosition;
    }

    private void Update()
    {
        if (equipped)
        {
            if (Input.GetKeyDown(toggleFire))
            {
                auto = !auto;
            }

            if (Input.GetKeyDown(reload))
            {
                startReload();
            }

            aimController();

            if (!reloading)
            {
                fireHandler();
            }
            else if (reloading && Input.GetKey(shoot)) // when reload anim added, swith to coroutine to wait before setting ammo, then stop coroutine
            {

            }
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
            else
            {
                cameraScript.setFOV(defaultFOV / defaultAimZoom);
            }
        }
        else
        {
            cameraScript.setFOV(defaultFOV / reloadZoom);
        }
    }

    private void aimController()
    {
        if (aimToggle)
        {
            if (Input.GetKeyUp(aim) && !resetAimingToggle)
            {
                resetAimingToggle = true;
            }

            if (Input.GetKeyDown(aim) && !aiming && resetAimingToggle)
            {
                aiming = true;
                resetAimingToggle = false;
                aimHandler();
            }

            if (Input.GetKeyDown(aim) && aiming && resetAimingToggle)
            {
                aiming = false;
                resetAimingToggle = false;
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
                readyToShoot = false;
                spawnBullet();
            }
        }
    }

    private void spawnBullet()
    {
        if (ammo > 0) {
            ammo--;
            var bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawn.forward * bulletSpeed;

            if (ammo == 0)
            {
                anim.Play("fireActionIncomplete", animLayer);
            }
            else
            {
                anim.Play("fireActionComplete", animLayer);
            }

            Invoke(nameof(spawnCasing), chargeOpenTime);
        } 
        else // play click sound 
        {

        }
    }

    private void spawnCasing()
    {
        float randomLR = Random.Range(lowerRandLR, upperRandLR);
        float randomFB = Random.Range(lowerRandFB, upperRandFB);
        float randomUD = Random.Range(lowerRandUD, upperRandUD);
        var casing = Instantiate(casingPrefab, casingSpawn.position, casingSpawn.rotation);
        casing.GetComponent<Rigidbody>().velocity = casingSpawn.up * ejectVelocity;
        casing.GetComponent<Rigidbody>().AddTorque(transform.right * randomLR);
        casing.GetComponent<Rigidbody>().AddTorque(transform.up * randomUD);
        casing.GetComponent<Rigidbody>().AddTorque(transform.forward * randomFB);
        cameraScript.addRecoil(verticalRecoil, horizontalRecoil);
    }

    private void resetShoot()
    {
        StartCoroutine(WaitForKickBackAnimation());
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
            anim.Play("closeAction", animLayer);
        }
        ammo = maxAmmo;
        StartCoroutine(WaitForCloseActionAnimation());
    }

    public void setEquipped(bool equipped)
    {
        this.equipped = equipped;
    }

    private void resetAim()
    {
        cameraScript.setFOV(defaultFOV);
        hand.transform.localPosition = defaultPos;
    }

    private IEnumerator WaitForKickBackAnimation()
    {
        while(!anim.GetCurrentAnimatorStateInfo(animLayer).IsTag("Idle"))
        {
            yield return null;
        }
        readyToShoot = true;
    }

    private IEnumerator WaitForCloseActionAnimation()
    {
        while (anim.GetCurrentAnimatorStateInfo(animLayer).IsName("closeAction"))
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
