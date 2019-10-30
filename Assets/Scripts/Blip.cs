using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blip : MonoBehaviour
{
    public enum State{
        Ejection = 1,
        Attack = 2,
        Transit = 3
    }

    public State BlipState;
    public int Player;
    public Node[] OriginDest;
    public Vector3 EjectionDir;
    public GameObject BlipPrefab;

    public Blip(State ejection, int player, Vector3 ejectDirection)
    {
        BlipState = ejection;
        Player = player;
        EjectionDir = ejectDirection;
        OriginDest = new Node[2];
        BlipPrefab = Instantiate(BlipPrefab);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
