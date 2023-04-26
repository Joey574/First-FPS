using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunController : MonoBehaviour
{
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

    [Header("UI Elements")]
    public Image crosshair;
    public TMP_Text ammoDisplay;

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
    public float vSway;
    public float hSway;

    private int animLayer = 0;
    private float aimT = 0;
    private GameObject hand;
    private GameObject camera;
    private PlayerCam cameraScript;
    private handMovement handScript;
    private Animator anim;

    private GameObject gameManager;
    private KeybindsController keybinds;

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
        handScript = hand.GetComponent<handMovement>();
    }

    private void Update()
    {
        if (equipped) // only run if object is equipped
        {
            handScript.setHSway(hSway);
            handScript.setVSway(vSway);

            if (Input.GetKeyDown(keybinds.ToggleFire())) // toggle fire mode
            {
                auto = !auto;
            }

            UIUpdater();

            if (Input.GetKeyDown(keybinds.Reload())) // start reload
            {
                startReload();
            }

            aimController();

            if (!reloading) // don't fire while reloading
            {
                fireHandler();
            }
        }
    }
    
    private void aimHandler()
    {
        if (!reloading)
        {
            hand.transform.localPosition = new UnityEngine.Vector3(Mathf.Lerp(defaultPos.x, defaultPos.x - xAimAdjust, aimT), Mathf.Lerp(defaultPos.y, defaultPos.y - yAimAdjust, aimT), Mathf.Lerp(defaultPos.z, defaultPos.z - zAimAdjust, aimT));

            if (aimT <= 1)
            {
                aimT += aimTime * Time.deltaTime;
            }

            if (hasScope)
            {
                cameraScript.setFOV(Mathf.Lerp(defaultFOV, defaultFOV / zoomMult, aimT));
            }
            else
            {
                cameraScript.setFOV(Mathf.Lerp(defaultFOV, defaultFOV / defaultAimZoom, aimT));
            }
        }
    }

    private void aimController()
    {

        if (Input.GetKeyUp(keybinds.Aim()))
        {
            aimT = 0;
        }

        if (aimToggle)
        {
            if (Input.GetKeyUp(keybinds.Aim()) && !resetAimingToggle)
            {
                resetAimingToggle = true;
            }

            if (Input.GetKeyDown(keybinds.Aim()) && !aiming && resetAimingToggle)
            {
                aiming = true;
                resetAimingToggle = false;
                aimHandler();
            }

            if (Input.GetKeyDown(keybinds.Aim()) && aiming && resetAimingToggle)
            {
                aiming = false;
                resetAimingToggle = false;
                resetAim();
            }
        }
        else
        {
            if (Input.GetKey(keybinds.Aim()))
            {
                aiming = true;
                aimHandler();
            }

            if (Input.GetKeyUp(keybinds.Aim()))
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

    private void UIUpdater()
    {
        ammoDisplay.text = ammo.ToString() + " / " + maxAmmo.ToString();

        if (aiming)
        {
            crosshair.enabled = false;
        }
        else if (!aiming)
        {
            crosshair.enabled = true;
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

        // adding directional torque
        casing.GetComponent<Rigidbody>().AddTorque(transform.right * randomLR);
        casing.GetComponent<Rigidbody>().AddTorque(transform.up * randomUD);
        casing.GetComponent<Rigidbody>().AddTorque(transform.forward * randomFB);

        // add recoil
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
