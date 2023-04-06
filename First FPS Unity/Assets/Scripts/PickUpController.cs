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
    public float upForce;
    public float forwardForce;

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
        Vector3 distanceToPlayer = player.position - transform.position;

        if (equipped && Input.GetKeyDown(drop))
        {
            equipped = false;
            gunScript.setEquipped(equipped);
            Drop();
        }

        if (!equipped && Input.GetKeyDown(pickUp) && distanceToPlayer.magnitude <= pickUpRange)
        {
            equipped = true;
            gunScript.setEquipped(equipped);
            PickUp();
        }

    }

    private void Drop()
    {
        gunRB.isKinematic = false;
        transform.SetParent(null, true);

        gunRB.velocity = player.GetComponent<Rigidbody>().velocity;

        gunRB.AddForce(camera.forward * forwardForce, ForceMode.Impulse);
        gunRB.AddForce(camera.up * upForce, ForceMode.Impulse);
    }

    private void PickUp()
    {
        gunRB.isKinematic = true;
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
