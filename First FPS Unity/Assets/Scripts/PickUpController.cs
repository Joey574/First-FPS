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
    
    [Header("Raycast Values"]
    public float p1;
    public float p2;
    public float radus;
    public float distance;

    private RayCastHit hit;
    private equipHandler item;

    void Update()
    {

        if (hasObject && Input.GetKeyDown(drop))
        {
            hasObject = false;
            Drop();
        }

        if (!hasObject && Input.GetKeyDown(pickUp))
        {
            hasObject = true;
            PickUp();
        }
    }
    
    private void Drop()
    {
        if (item != null) 
        {
            item.setEquipped(false);
        }
    }

    private void PickUp()
    {
        if (Physics.CapsuleCast(p1, p2, radius, camera.forward, out hit, distance) && hit.transform.tag == "Equippable") 
           {
               item = hit.getComponent<equipHandler>();
               item.setEquipped(true);
           }
    }
}
