using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCasingDespawn : MonoBehaviour
{

    public float life = 5;

    private void Awake()
    {
        Destroy(gameObject, life);
    }
}
