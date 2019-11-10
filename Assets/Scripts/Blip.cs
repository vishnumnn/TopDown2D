using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blip : MonoBehaviour
{
    public enum State{
        Ejection = 1,
        Attack = 2,
        Transit = 3,
        Idle = 4
    }
    // Blip State specifics
    // Ejection
    const float EJECT_SPEED = 1f;
    const float MAX_DIST = 20f;
    private float distTraveled = 0.0f;

    // Blip characteristics
    public State BlipState;
    public int Player;
    public GameObject[] OriginDest;
    public Vector3 Direction;
 
    public Blip(State state, int player, Vector3 direction)
    {
        BlipState = state;
        Player = player;
        Direction = direction;
        OriginDest = new GameObject[2];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // rotation to direction
        float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if(BlipState == State.Ejection)
        {
            while(distTraveled < MAX_DIST)
            {
                this.transform.Translate(EJECT_SPEED * Time.deltaTime * Direction);
                distTraveled += EJECT_SPEED * Time.deltaTime;
            }
        }
    }
}
