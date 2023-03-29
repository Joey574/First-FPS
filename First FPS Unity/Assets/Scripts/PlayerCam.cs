using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public float verticalRecoil = 0;
    public float horizontalRecoil = 0;

    public Transform orientation;
    public Transform player;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        

        yRotation += mouseX - horizontalRecoil;

        xRotation -= mouseY + verticalRecoil;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        horizontalRecoil = 0;
        verticalRecoil = 0;

        transform.rotation = UnityEngine.Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = UnityEngine.Quaternion.Euler(0, yRotation, 0);
        player.rotation = UnityEngine.Quaternion.Euler(0, yRotation, 0);
    }

    public void addRecoil(float v, float h)
    {
        verticalRecoil += v;
        horizontalRecoil += h;
    }

    public UnityEngine.Vector3 getPos()
    {
        return transform.position;
    }

    public void setFOV (float f)
    {
        Camera.main.fieldOfView = f;
    }
}
