using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwayController : MonoBehaviour
{

    [Header("Adjustments")]
    public float hSwayMultiplier;
    public float vSwayMultiplier;
    public float smooth;

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * hSwayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * vSwayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    public void setSmooth(float s)
    {
        smooth = s;
    }

    public void setHSway(float h)
    {
        hSwayMultiplier = h;
    }

    public void setVSway(float v)
    {
        vSwayMultiplier = v;
    }

}


