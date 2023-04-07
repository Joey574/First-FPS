using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybindsController : MonoBehaviour
{
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode aim = KeyCode.Mouse1;
    public KeyCode pickUp = KeyCode.F;
    public KeyCode drop = KeyCode.G;
    public KeyCode reload = KeyCode.R;
    public KeyCode toggleFire = KeyCode.V;
    
    private KeyCode [] fireKeys = new KeyCode [];
    private KeyCode [] itemInteractions = new KeyCode [];
    
    void Start()
    {
        // gun control keys
        fireKeys.add(shoot);
        fireKeys.add(aim);
        fireKeys.add(reload);
        fireKeys.add(toggleFire);
        
        // item interation keys
        itemInteractions.add(pickUp);
        itemInteractions.add(drop);
    }
    
    void Update()
    {
        
    }
    
    public KeyCode [] FireKeys()
    {
        return fireKeys;
    }
    
    public KeyCode [] ItemInterations()
    {
        return itemInteractions;
    }
}
