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

    // Screen Partioning

    // Blip State specifics
    const float MAX_EJECT_DIST = 0.5f;
    const float EJECT_SPEED = MAX_EJECT_DIST/2;
    const float TRANSIT_SPEED = EJECT_SPEED * 3;
    const float ORBIT_RADIUS = 0.7f;
    private float distTraveled = 0.0f;

    // Blip characteristics
    public State BlipState;
    public int Player;
    public GameObject[] OriginDest;

    // On select sprite
    private Sprite selectSprite;
    private Sprite normalSprite;

    private void Awake()
    {
        selectSprite = Resources.Load<Sprite>("Sprites/GreenBlipSelected");
        normalSprite = Resources.Load<Sprite>("Sprites/GreenBlip");
    }

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
                
                if (distTraveled <= MAX_EJECT_DIST)
                {
                    distTraveled += Translate(EJECT_SPEED, transform.up);
                }
                else
                {
                    BlipState = State.Idle;
                    distTraveled = 0;
                }
                break;
            case State.Idle:
                Vector3 origin = OriginDest[0].transform.position;
                
                if (Vector3.Distance(origin, transform.position) <= ORBIT_RADIUS)
                {
                    transform.RotateAround(origin, Vector3.forward, 20 * Time.deltaTime);
                }
                break;
            case State.Transit:
                if(Vector3.Magnitude(transform.position - OriginDest[1].transform.position) > MAX_EJECT_DIST) 
                {
                    Translate(TRANSIT_SPEED, transform.up);
                }
                else
                {
                    OriginDest[0] = OriginDest[1];
                    OriginDest[1] = null;
                    BlipState = State.Idle;
                }
                break;
            case State.Attack:

            default:
                break;
        }
    }

    private float Translate(float speed, Vector3 dir)
    {
        dir = dir.normalized;
        transform.Translate(dir.x * speed * Time.deltaTime, dir.y * speed * Time.deltaTime, 0, Space.World);

        return speed * Time.deltaTime;
    }

    public bool ChangeSprite(bool selected)
    {
        Debug.Log("Tried to change sprite");
        if (selected)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = selectSprite;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = normalSprite;
        }
        return selected;
    }

    public override string ToString()
    {
        return $"{transform.up.ToString()}";
    }

    private void AdjustOnProximity()
    {
        Selector.Selected.ForEach(e => {
            if (!gameObject.Equals(e))
            {
                if (Vector3.Distance(e.transform.position, transform.position) < 0.1f)
                {
                    Vector3 dir = e.transform.position - transform.position;
                    dir = Quaternion.LookRotation(Vector3.forward, Vector3.down) * dir;
                }
            }
        });
    }

}
