using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{

    public bool hasObject = false;

    [Header("Keybinds")]
    public KeyCode drop = KeyCode.G;
    public KeyCode pickUp = KeyCode.F;

    [Header("Objects")]
    public Transform camera;

    [Header("Adjustments")]
    public Vector3 p1;
    public Vector3 p2;
    public float upForce;
    public float forwardForce;
    
    [Header("Raycast Values")]
    public float radius;
    public float distance;

    private RaycastHit hit;
    private UniversalItemHandler item;

    private Vector3 P1;
    private Vector3 P2;

    void Update()
    {
        P1 = camera.position;
        P2 = camera.position;

        if (hasObject && Input.GetKeyDown(drop))
        {
            Drop();
        }

        if (!hasObject && Input.GetKeyDown(pickUp))
        {
            PickUp();
        }
    }
    
    private void Drop()
    {
        if (item != null) 
        {
            item.setEquipped(false, upForce, forwardForce);
            hasObject = false;
        }
    }

    private void PickUp()
    {
        if (Physics.CapsuleCast(P1 + p1, P2 + p2, radius, camera.forward, out hit, distance, LayerMask.GetMask("Items")))
           {
               item = hit.transform.GetComponent<UniversalItemHandler>();
               item.setEquipped(true);
               hasObject = true;
        }
    }
}


