using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public RectTransform selectorImage;

    private Vector3 start, end;

    private List<GameObject> Blips;
    public static List<GameObject> Selected;

    private void Awake()
    {
        Blips = new List<GameObject>();
        Selected = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        selectorImage.gameObject.SetActive(false);
        Blips.AddRange(ObjectPoolContainer.sharedInstance.GetAllObjectsOfTag("GreenBlip"));
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGUI();
        UpdateSelected();
    }

    private void UpdateGUI()
    {
        if (Input.GetMouseButtonDown(0))
        {
            start = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectorImage.gameObject.SetActive(false);
        }

        if (Input.GetMouseButton(0))
        {
            if (!selectorImage.gameObject.activeInHierarchy)
            {
                selectorImage.gameObject.SetActive(true);
            }

            end = Input.mousePosition;
            selectorImage.position = (start + end) / 2f;
            selectorImage.sizeDelta = new Vector2(Mathf.Abs(start.x - end.x), Mathf.Abs(start.y - end.y));
        }
    }

    private void UpdateSelected()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if(Vector3.Magnitude(end - start) < 19f)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, 100f))
                {
                    if(hit.transform != null)
                    {
                        Send(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                Selected.ForEach(e => { e.gameObject.GetComponent<Blip>().ChangeSprite(false); });
                Selected.Clear();
                foreach (GameObject obj in Blips)
                {
                    if (obj.activeInHierarchy && IsInSelector(obj))
                    {
                        Debug.Log("Change Sprite");
                        obj.GetComponent<Blip>().ChangeSprite(true);
                        Selected.Add(obj);
                    }
                }
            }
        }
    }

    private bool IsInSelector(GameObject o)
    {
        Debug.Log("IS IN SELECTOR");
        Vector3 pos1 = Camera.main.ScreenToViewportPoint(start);
        Vector3 pos2 = Camera.main.ScreenToViewportPoint(end);
        Rect rect = new Rect(pos1.x, pos1.y, pos2.x - pos1.x, pos2.y - pos1.y);
        if (rect.Contains(Camera.main.WorldToViewportPoint(o.transform.position), true))
        {
            return true;
        }
        return false;
    }

    private void Send(GameObject node)
    {
        Selected.ForEach(e =>
        {
            Blip script = e.GetComponent<Blip>();
            if (script.OriginDest[0].GetComponent<Node>().neighbors.Contains(node))
            {
                if(script.BlipState != Blip.State.Transit)
                {
                    script.OriginDest[1] = node;
                    script.BlipState = Blip.State.Transit;
                    e.transform.rotation = Quaternion.LookRotation(Vector3.forward, node.transform.position - e.transform.position);
                }
            }
            else if(script.OriginDest[0].Equals(node))
            {
                script.OriginDest[0] = script.OriginDest[1];
                script.OriginDest[1] = node;
                script.BlipState = Blip.State.Transit;
                e.transform.rotation = Quaternion.LookRotation(Vector3.forward, node.transform.position - e.transform.position);
            }
            else
            {

            }
        });
    }
}
