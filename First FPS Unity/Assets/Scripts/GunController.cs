using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
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
    public float verticalInaccuracy;
    public float horizontalInaccuracy;
    public float vSway;
    public float hSway;
    public float reloadTime;
    public float volume;
    public float zoomMult;
    public bool hasScope;
    public bool canAuto;

    private bool equipped;

    [Header("Objects")]
    public GameObject gun;
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public Transform bulletSpawn;
    public Transform casingSpawn;

    [Header("Adjustments")]
    public float defaultFOV;
    public float defaultAimZoom = 1.5f;
    public float chargeOpenTime;
    public float xAimAdjust;
    public float yAimAdjust;
    public float zAimAdjust;
    public float lowerRandFB;
    public float lowerRandLR;
    public float lowerRandUD;
    public float upperRandFB;
    public float upperRandLR;
    public float upperRandUD;
    public float aimTime = 5;

    private int animLayer = 0;
    private float aimT = 0;
    private GameObject hand;
    private GameObject camera;
    private PlayerCam cameraScript;
    private GunSwayController gunSway;
    private Animator anim;

    private GameObject gameManager;
    private KeybindsController keybinds;

    // UI Elements
    private GameObject UI;
    private GameObject crosshairObject;
    private GameObject ammoDisplayObject;

    private void Awake()
    {
        // Grabbing gameManager + keybinds
        gameManager = GameObject.Find("GameManager");
        keybinds = gameManager.GetComponent<KeybindsController>();

        // Grabbing hand object + setting default pos
        hand = GameObject.Find("RightHand");
        defaultPos = hand.transform.localPosition;

        // Grabbing playerCam script from playerCam
        camera = GameObject.Find("PlayerCam");
        cameraScript = camera.GetComponent <PlayerCam> ();

        // Grabbing animator
        anim = gun.GetComponent<Animator>();

        // Grab handMovement script
        gunSway = hand.GetComponent<GunSwayController>();
    }

    private void Update()
    {
        if (equipped) // only run if object is equipped
        {
            gunSway.setHSway(hSway);
            gunSway.setVSway(vSway);

            if (Input.GetKeyDown(keybinds.ToggleFire()) && canAuto) // toggle fire mode
            {
                auto = !auto;
            }

            if (Input.GetKeyDown(keybinds.Reload())) // start reload
            {
                startReload();
            }

            aimController(); // handles aiming

            if (!reloading) // don't fire while reloading
            {
                fireHandler();
            }
        }
    }
    
    private void aimHandler() // handles aim zoom
    {
        if (!reloading)
        {
            hand.transform.localPosition = new UnityEngine.Vector3(Mathf.Lerp(defaultPos.x, defaultPos.x - xAimAdjust, aimT), Mathf.Lerp(defaultPos.y, defaultPos.y - yAimAdjust, aimT), Mathf.Lerp(defaultPos.z, defaultPos.z - zAimAdjust, aimT));

            if (aimT < 1) // clamp aimT to 1
            {
                aimT += aimTime * Time.deltaTime;
            }

            if (hasScope) // talk to scope for zoomMult
            {
                cameraScript.setFOV(Mathf.Lerp(defaultFOV, defaultFOV / zoomMult, aimT));
            }
            else // default zoom mult
            {
                cameraScript.setFOV(Mathf.Lerp(defaultFOV, defaultFOV / defaultAimZoom, aimT));
            }
        }
    }

    private void aimController() // handles when to aim and when to reset pos of objects
    {

        if (Input.GetKeyUp(keybinds.Aim())) // reset aimT
        {
            aimT = 0;
            aiming = false;
            resetAim();
        }

        if (Input.GetKey(keybinds.Aim()))
        {
            aiming = true;
            aimHandler();
        }
    }

    private void fireHandler() // handles shooting
    {
        if (canAuto && auto)
        {
            if (Input.GetKey(keybinds.Shoot()) && readyToShoot && auto)
            {
                readyToShoot = false;
                spawnBullet();
                Invoke(nameof(resetShoot), (60f / fireRate));
            }
        }
        else
        {
            if (Input.GetKeyUp(keybinds.Shoot()))
            {
                resetShoot();
            }
            
            if (Input.GetKeyDown(keybinds.Shoot()) && readyToShoot)
            {
                readyToShoot = false;
                spawnBullet();
            }
        }
    }

    private void spawnBullet() // spawn bullet + add velocity
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

    private void spawnCasing() // spawn casing + add torque
    {
        float randomLR = Random.Range(lowerRandLR, upperRandLR);
        float randomFB = Random.Range(lowerRandFB, upperRandFB);
        float randomUD = Random.Range(lowerRandUD, upperRandUD);
        var casing = Instantiate(casingPrefab, casingSpawn.position, casingSpawn.rotation);
        casing.GetComponent<Rigidbody>().velocity = casingSpawn.up * ejectVelocity;

        // adding directional torque
        casing.GetComponent<Rigidbody>().AddTorque(transform.right * randomLR);
        casing.GetComponent<Rigidbody>().AddTorque(transform.up * randomUD);
        casing.GetComponent<Rigidbody>().AddTorque(transform.forward * randomFB);

        // add recoil
        cameraScript.addRecoil(verticalRecoil, horizontalRecoil);
    }

    private void resetShoot()
    {
        readyToShoot = true;
    }

    private void startReload() // initiate reload
    {
        reloading = true;
        hand.transform.localPosition = defaultPos;
        Invoke(nameof(setAmmoToMax), reloadTime);
    }

    private void setAmmoToMax() // set ammo to max + play close action if empty
    {
        if (ammo == 0)
        {
            anim.Play("closeAction", animLayer);
        }
        ammo = maxAmmo;
        StartCoroutine(WaitForCloseActionAnimation());
    }

    public void setEquipped(bool equipped) // set equipped to true
    {
        this.equipped = equipped;
    }

    private void resetAim() // reset aim and move hand back to default pos
    {
        cameraScript.setFOV(defaultFOV);
        hand.transform.localPosition = defaultPos;
    }

    private IEnumerator WaitForCloseActionAnimation() // wait for close action on reload if needed
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

    public int getAmmo()
    {
        return ammo;
    }

    public bool getAiming()
    {
        return aiming;
    }
}
