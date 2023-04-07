using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalItemHandler : MonoBehaviour
{
    public string handName;
    public Rigidbody rb;

    [Header("Item Type")]
    public bool gun = false;

    private bool equipped = false;
    private GameObject hand;
    private GunController gunScript;

    void Start()
    {
        hand = GameObject.Find(handName);

        if (gun)
        {
            gunScript = transform.GetComponent<GunController>();
        }
    }

    public void setEquipped(bool equipped)
    {
        this.equipped = equipped;
        equipUpdate();
    }

    public void setEquipped(bool equipped, float upForce, float forwardForce)
    {
        this.equipped = equipped;
        equipUpdate();
        rb.AddForce(transform.forward * forwardForce, ForceMode.Impulse);
        rb.AddForce(transform.up * upForce, ForceMode.Impulse);
    }

    public void equipUpdate()
    {
        if (gun)
        {
            gunScript.setEquipped(equipped);
        }

        if (equipped)
        {
            transform.SetParent(hand.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            rb.isKinematic = true;
        }
        else
        {
            transform.SetParent(null, true);
            rb.isKinematic = false;
        }
    }
}
