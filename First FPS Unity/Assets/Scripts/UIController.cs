using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIController : MonoBehaviour
{
    private GunController activeGun;
    private GameObject player;
    private GameObject hand;
    private PickUpController pickUp;

    // UI Elements
    private GameObject crosshairObject;
    private GameObject ammoObject;
    private Image crosshair;
    private TMP_Text ammo;

    void Awake()
    {
        player = GameObject.Find("Player");
        hand = GameObject.Find("RightHand");
        crosshairObject = GameObject.Find("Crosshair");
        ammoObject = GameObject.Find("AmmoDisplay");

        crosshair = crosshairObject.GetComponent<Image>();
        ammo = ammoObject.GetComponent<TMP_Text>();

        pickUp = player.GetComponent<PickUpController>();


    }

    void Update()
    {
        if (pickUp.getHasObject())
        {
            activeGun = hand.GetComponentInChildren<GunController>();

            crosshairControl();

            ammo.SetText(activeGun.getAmmo().ToString());
        }
        else
        {
            activeGun = null;

            ammo.SetText("");
            crosshair.enabled = true;
        }
    }

    private void crosshairControl()
    {
        if (activeGun.getAiming())
        {
            crosshair.enabled = false;
        }
        else
        {
            crosshair.enabled = true;
        }
    }
}
