using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwayController : MonoBehaviour
{
    public Transform PlayerCam;
    public Transform PlayerObj;

    [Header("Adjustments")]
    public float horizontalTime = 0;
    public float verticalTime = 0;

    public float xRotTolerance = 0.02f;
    public float yRotTolerance = 0.02f;

    private float xRot;
    private float yRot;

    private float hSway = 0;
    private float vSway = 0;

    void Update()
    {
        xRot = Mathf.Lerp(transform.localRotation.x, PlayerCam.localRotation.x, horizontalTime);
        yRot = Mathf.Lerp(transform.localRotation.y, PlayerObj.localRotation.y, verticalTime);

        transform.localRotation = UnityEngine.Quaternion.Euler(xRot, yRot, 0);

        horizontalTime += hSway * Time.deltaTime;
        verticalTime += vSway * Time.deltaTime;
    }

    public void setVSway(float verticalSway)
    {
        vSway = verticalSway;
    }

    public void setHSway(float horizontalSway)
    {
        hSway = horizontalSway;
    }

}
