﻿using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGraph : MonoBehaviour
{
    // Constants
    private const float MIN_EDGE_LEN = 0.25f;

    // Unity editor set slider variables
    public int numNodes;
    public GameObject nodeSprite;
    public Material material;
    private Camera cam;

    // State variables
    private int CurrID = 0;
    private List<GameObject> Nodes;
    private List<GameObject> edges;

    private GameObject GetNode(float xdist, float ydist)
    {
        GameObject node = Instantiate(nodeSprite) as GameObject;
        Node script = node.AddComponent<Node>();
        script.position = new Vector3(xdist, ydist, 1);
        node.transform.position = cam.ViewportToWorldPoint(script.position);
        script.Id = CurrID;
        script.currentBlips = 0;
        script.neighbors = new List<GameObject>();
        return node;
    }

    /// <summary>
    /// Checks to see if given coordinate pair isn't too close to any other sets of used coordinates. 
    /// <note>
    /// This algorithm runs in O(n). This isn't ideal as a self-balacing k-d tree could be used to check validity in O(log(n)) time.
    /// This would however require storing ranges of invalid x and y coordinate values in nodes separately from one another.
    /// </note>
    /// </summary>
    /// <param name="coords"></param>
    /// <param name="used"></param>
    /// <returns></returns>
    private bool isValid(Tuple<float, float> coords, List<Tuple<float, float>> used)
    {
        float minDistX = nodeSprite.GetComponent<SpriteRenderer>().bounds.size.x / (Screen.width - 0.1f * Screen.width);
        float minDistY = nodeSprite.GetComponent<SpriteRenderer>().bounds.size.y / (Screen.height - 0.1f * Screen.height);
        float minDist = Mathf.Sqrt(minDistX * minDistX + minDistY * minDistY);
        foreach (Tuple<float, float> e in used)
        {
            float dist = Mathf.Sqrt(Mathf.Pow(coords.Item1 - e.Item1, 2) + Mathf.Pow(coords.Item2 - e.Item2, 2));
            if (dist < (minDist + MIN_EDGE_LEN))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// This function would ideally use some kind of heuristic to make sure that nodes are not generated too close to one another. 
    /// Could eventually factor in mean and standard deviation of previous nodes x and y values in order to create a new distribution of 
    /// offsets to select from, which will be added to the new node's x and y values. This way previously unused space will have a greater
    /// chance of being filled in, so that the map isn't mostly empty if you're unlucky. 
    /// </summary>

    private void generateNodes()
    {
        List<GameObject> nodes = new List<GameObject>();
        List<Tuple<float, float>> used = new List<Tuple<float, float>>();
        float min = 0.05f;
        float max = 0.95f;
        int tries = 0;
        while (tries < 60 && CurrID < numNodes)
        {
            Tuple<float, float> coords = new Tuple<float, float>(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));
            if (isValid(coords, used))
            {
                nodes.Add(GetNode(coords.Item1, coords.Item2));
                tries = 0;
                CurrID++;
                used.Add(coords);
            }
            else
            {
                tries++;
            }
        }
        Nodes = nodes;
    }

    /// <summary>
    /// This method takes nLogn time to sort distances between the current node and all other nodes.
    /// </summary>
    /// <note>
    /// This method requries at elast N^(2)LogN time to process all nodes in the graph. 
    /// </note>
    private void MatchAdjacents(int maxAdjacents)
    {
        generateNodes();
        Queue<GameObject> unvisited = new Queue<GameObject>();
        foreach(GameObject n in Nodes)
        {
            unvisited.Enqueue(n);
        }
        while (unvisited.Count > 0)
        {
            GameObject curr = unvisited.Dequeue();
            if (curr.GetComponent<Node>().neighbors.Count < maxAdjacents)
            {
                IEnumerable<GameObject> closest = GetClosestUnvisited(Nodes, maxAdjacents, curr);
                foreach (GameObject e in closest)
                {
                    e.GetComponent<Node>().neighbors.Add(curr);
                    unvisited.Enqueue(e);
                }
                curr.GetComponent<Node>().neighbors.AddRange(closest);
            }
        }
    }

    private IEnumerable<GameObject> GetClosestUnvisited(List<GameObject> nodes, int v, GameObject curr)
    {
        List<GameObject> sol = new List<GameObject>();
        SortedDictionary<float, GameObject> dict = new SortedDictionary<float, GameObject>();
        // Adding to the sortedDict takes log(n) time.
        foreach (GameObject n in Nodes)
        {
            if (n.GetComponent<Node>().neighbors.Count < v && !n.GetComponent<Node>().neighbors.Contains(curr))
            {
                dict.Add(curr.GetComponent<Node>().getDist(n.GetComponent<Node>()), n);
            }
        }
        foreach (KeyValuePair<float, GameObject> pair in dict)
        {
            if (curr.GetComponent<Node>().neighbors.Count + sol.Count >= v)
            {
                break;
            }else if(pair.Value.GetComponent<Node>().Id != curr.GetComponent<Node>().Id)
            {
                sol.Add(pair.Value);
            }
        }
        return sol;
    }

    private void DrawGraph()
    {
        MatchAdjacents(4);
    }

    private void DrawEdges()
    {
        if (cam != null)
        {
            edges = new List<GameObject>();
            bool[] drawn = new bool[Nodes.Count];
            foreach (GameObject e in Nodes)
            {
                Vector3 vertex = e.GetComponent<Node>().position;
                foreach (GameObject n in e.GetComponent<Node>().neighbors)
                {
                    if (!drawn[n.GetComponent<Node>().Id])
                    {
                        GameObject empty = new GameObject();
                        empty.AddComponent<LineRenderer>();
                        LineRenderer lr = empty.GetComponent<LineRenderer>();
                        lr.material = material;
                        lr.startWidth = 0.05f;
                        lr.endWidth = 0.05f;
                        lr.startColor = Color.grey;
                        lr.endColor = Color.grey;
                        lr.positionCount = 2;
                        lr.SetPosition(0, cam.ViewportToWorldPoint(e.GetComponent<Node>().position));
                        lr.SetPosition(1, cam.ViewportToWorldPoint(n.GetComponent<Node>().position));
                        edges.Add(empty);
                    }
                }
                drawn[e.GetComponent<Node>().Id] = true;
            }
        }
        else
        {
            Debug.LogError("Camera not initialized");
        }
    }
    private void OnPostRender()
    {

    }
    
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        DrawGraph();
        DrawEdges();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
