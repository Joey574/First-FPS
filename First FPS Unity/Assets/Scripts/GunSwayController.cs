using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwayController : MonoBehaviour
{
    public Transform playerCam;

    private float horizontalSway;
    private float verticalSway;

    private float horizontalSwayT;
    private float verticalSwayT;

    void Update()
    {
        horizontalSwayT += horizontalSway * Time.deltaTime;
        verticalSwayT += verticalSway * Time.deltaTime;

        transform.rotation = UnityEngine.Quaternion.Euler(Mathf.LerpUnclamped(playerCam.rotation.x, transform.rotation.x, horizontalSwayT), Mathf.LerpUnclamped(playerCam.rotation.y, transform.rotation.y, verticalSwayT), playerCam.rotation.z);
    }

    private void resetTime()
    {
        horizontalSwayT = 0;
        verticalSwayT = 0;
    }

    public void setHSway(float s)
    {
        horizontalSway = s;
    }

    public void setVSway(float s)
    {
        verticalSway = s;
    }
}
