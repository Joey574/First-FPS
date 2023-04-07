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
    public float upForce;
    public float forwardForce;
    
    [Header("Raycast Values")]
    public float radius;
    public float distance;

    private RaycastHit hit;
    private UniversalItemHandler item;

    private Vector3 p1;
    private Vector3 p2;

    void Update()
    {
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
        p1 = camera.position;
        p2 = camera.position;
        if (Physics.CapsuleCast(p1, p2, radius, camera.forward, out hit, distance, LayerMask.GetMask("Items")))
           {
               item = hit.transform.GetComponent<UniversalItemHandler>();
               item.setEquipped(true);
               hasObject = true;
        }
    }
}


