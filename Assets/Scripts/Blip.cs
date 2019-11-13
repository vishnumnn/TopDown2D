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
    const float MAX_EJECT_DIST = 0.5f;
    const float EJECT_SPEED = MAX_EJECT_DIST/2;
    private float distTraveled = 0.0f;

    // Blip characteristics
    public State BlipState;
    public int Player;
    public GameObject[] OriginDest;
    public Vector3 Direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   // rotation to direction

        if(BlipState == State.Ejection)
        {
            float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if(distTraveled <= MAX_EJECT_DIST)
            {
                transform.Translate(Direction.x * EJECT_SPEED * Time.deltaTime, Direction.y * EJECT_SPEED * Time.deltaTime, 0);
                distTraveled += EJECT_SPEED * Time.deltaTime;
            }
        }
    }
}
