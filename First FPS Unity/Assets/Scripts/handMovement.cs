using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handMovement : MonoBehaviour
{
    public Transform player;

    [Header("Adjustments")]
    public float xLocPosTar;
    public float yLocPosTar;
    public float zLocPosTar;

    private float horizontalSway;
    private float verticalSway;

    private float horizontalSwayT;
    private float verticalSwayT;

    void Update()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.position.x - xLocPosTar, horizontalSwayT), Mathf.Lerp(transform.position.y, player.position.y - yLocPosTar, verticalSwayT), player.position.z - zLocPosTar);
        horizontalSwayT += horizontalSway * Time.deltaTime;
        verticalSwayT += verticalSway * Time.deltaTime;
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
