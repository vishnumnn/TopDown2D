using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGraph : MonoBehaviour
{
    // Constants
    private const float MIN_EDGE_LEN = 0.07f;
    // Unity editor set slider variables
    public int numNodes;
    private int CurrID = 0;
    private List<Node> Nodes;
    public GameObject nodeSprite;
    public Material material;
    private Camera cam;


    private Node GetNode(float xdist, float ydist)
    {
        Node n = new Node();
        n.x = xdist;
        n.y = ydist;
        n.Id = CurrID;
        n.neighbors = new List<Node>();
        return n;
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
        List<Node> nodes = new List<Node>();
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
        bool[] explored = new bool[Nodes.Count];
        Queue<Node> unvisited = new Queue<Node>();
        unvisited.Enqueue(Nodes[0]);
        while (unvisited.Count > 0)
        {
            Node curr = unvisited.Dequeue();
            if (curr.neighbors.Count < maxAdjacents)
            {
                IEnumerable<Node> closest = GetClosestUnvisited(explored, Nodes, maxAdjacents, curr);
                foreach (Node e in closest)
                {
                    e.neighbors.Add(curr);
                    unvisited.Enqueue(e);
                }
                curr.neighbors.AddRange(closest);
                explored[curr.Id] = true;
            }
        }
    }

    private IEnumerable<Node> GetClosestUnvisited(bool[] visited, List<Node> nodes, int v, Node curr)
    {
        List<Node> sol = new List<Node>();
        SortedDictionary<float, Node> dict = new SortedDictionary<float, Node>();
        // Adding to the sortedDict takes log(n) time.
        foreach (Node n in Nodes)
        {
            if (!visited[n.Id])
            {
                dict.Add(curr.getDist(n), n);
            }
        }
        foreach (KeyValuePair<float, Node> pair in dict)
        {
            if (curr.neighbors.Count + sol.Count >= v)
            {
                break;
            }else if(pair.Value.Id != curr.Id)
            {
                sol.Add(pair.Value);
            }
        }
        return sol;
    }

    private void DrawGraph()
    {
        MatchAdjacents(6);
        foreach (Node e in Nodes)
        {
            Vector3 vertex = new Vector3(e.x, e.y, 1);
            Instantiate(nodeSprite, cam.ViewportToWorldPoint(vertex), Quaternion.identity);
        }
    }

    private void OnPostRender()
    {
        if (cam != null)
        {
            GL.Begin(GL.LINES);
            material.SetPass(0);
            bool[] drawn = new bool[Nodes.Count];
            foreach (Node e in Nodes)
            {
                Vector3 vertex = new Vector3(e.x, e.y, 1);
                foreach (Node n in e.neighbors)
                {
                    if (!drawn[n.Id])
                    {
                        GL.Color(Color.white);
                        GL.Vertex(cam.ViewportToWorldPoint(vertex));
                        GL.Vertex(cam.ViewportToWorldPoint(new Vector3(n.x, n.y, 1)));
                    }
                }
                drawn[e.Id] = true;
            }
            GL.End();
        }
        else
        {
            Debug.LogError("Camera not initialized");
        }

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
        Nodes.ForEach(e => {
            Debug.Log($"Node{e.Id}");
            e.neighbors.ForEach(n => Debug.Log($"To Node: {n.Id}"));
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
