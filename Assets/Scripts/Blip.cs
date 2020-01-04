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
    const float MAX_EJECT_DIST = 0.5f;
    const float EJECT_SPEED = MAX_EJECT_DIST/2;
    const float ORBIT_RADIUS = 0.6f;
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

        switch (BlipState)
        {
            case State.Ejection:
                
                transform.rotation = rotate(Direction);
                if (distTraveled <= MAX_EJECT_DIST)
                {
                    distTraveled += translate(EJECT_SPEED, Direction);
                }
                else
                {
                    BlipState = State.Idle;
                }
                break;
            case State.Idle:
                Vector3 origin = OriginDest[0].transform.position;
                if(Vector3.Distance(origin, transform.position) >= ORBIT_RADIUS)
                {
                    Direction = rotateByAngle(100.0f) * Direction;
                    transform.rotation = rotate(Direction);
                }
                translate(ORBIT_RADIUS, Direction);
                break;
            case State.Transit:

            case State.Attack:

            default:
                break;
        }
    }

    private float translate(float speed, Vector3 dir)
    {
        transform.Translate(dir.x * speed * Time.deltaTime, dir.y * speed * Time.deltaTime, 0);
        return speed * Time.deltaTime;
    }

    private Quaternion rotate(Vector3 dir)
    {
        float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private Quaternion rotateByAngle(float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward); 
    }
}
