using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwayController : MonoBehaviour
{
    public Transform PlayerCam;
    public Rigidbody cameraRigidBody;
    public Transform PlayerObj;
    public Rigidbody playerRigidBody;

    [Header("Adjustments")]
    public float horizontalTime = 0;
    public float verticalTime = 0;

    public float xRot;
    public float yRot;

    public float hSway = 0;
    public float vSway = 0;

    public float hSwayAdjust;
    public float vSwayAdjust;

    public bool hasRun = false;

    void Update()
    {
        Debug.Log(cameraRigidBody.angularVelocity.magnitude);

        if (cameraRigidBody.angularVelocity.magnitude != 0 || playerRigidBody.angularVelocity.magnitude != 0)
        {
            if (hasRun == false)
            {
                hasRun = true;
                hSwayAdjust = transform.parent.rotation.x - transform.rotation.x;
                vSwayAdjust = transform.parent.rotation.y - transform.rotation.y;
                horizontalTime = 1;
                verticalTime = 1;
            }
        }

        if (horizontalTime > 0)
        {
            horizontalTime -= hSway * Time.deltaTime;
        }
        if (verticalTime > 0)
        {
            verticalTime -= vSway * Time.deltaTime;
        }

        if (verticalTime <= 0 && horizontalTime <= 0)
        {
            hasRun = false;
        }

        xRot = Mathf.Lerp(transform.localRotation.x + hSwayAdjust, 0, horizontalTime);
        yRot = Mathf.Lerp(transform.localRotation.y + vSwayAdjust, 0, verticalTime);

        transform.localRotation = UnityEngine.Quaternion.Euler(xRot, yRot, 0);

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
