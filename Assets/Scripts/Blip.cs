using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blip : MonoBehaviour
{
    private enum State{
        Ejection = 1,
        Attack = 2,
        Transit = 3
    }

    private int BlipState { get; set; }
    private int Color { get; set; }
    private Node[] OriginDest { get; set; }
    private Vector3 EjectionDir { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
