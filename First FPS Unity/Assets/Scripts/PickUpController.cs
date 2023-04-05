using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public bool equipped = false;

    [Header("Objects")]
    public GameObject gun;
    public Transform player, hand, camera;

    [Header("Adjustments")]
    public float pickUpRange;

    [Header("Keybinds")]
    public KeyCode drop = KeyCode.G;
    public KeyCode pickUp = KeyCode.F;

    private GunController gunScript;
    private Rigidbody gunRB;

    // Start is called before the first frame update
    void Start()
    {
        gunScript = gun.GetComponent<GunController>();
        gunRB = gun.GetComponent<Rigidbody>();
        gunScript.setEquipped(equipped);
    }

    // Update is called once per frame
    void Update()
    {

        if (equipped && Input.GetKeyDown(drop))
        {
            equipped = false;
            gunScript.setEquipped(equipped);
        }

        if (equipped)
        {
            gunRB.isKinematic = true;
        }
        else
        {
            gunRB.isKinematic = false;
        }
    }
}
