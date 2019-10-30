using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGraph : MonoBehaviour
{
    // Constants
    private const float MIN_EDGE_LEN = 0.1f;

    // Unity editor set slider variables
    public int numNodes;
    private int CurrID = 0;
    private List<Node> Nodes;
    public GameObject nodeSprite;
    public GameObject blipSprite;
    public Material material;
    private Camera cam;
    private List<GameObject> edges;
    private float ejectSpeed = 0.1f;

    // Others
    private List<Blip> blipsInScene = new List<Blip>();
    private Node GetNode(float xdist, float ydist)
    {
        Node n = new Node();
        n.x = xdist;
        n.y = ydist;
        n.Id = CurrID;
        n.BlipCount = 40;
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
        Queue<Node> unvisited = new Queue<Node>();
        foreach(Node n in Nodes)
        {
            unvisited.Enqueue(n);
        }
        while (unvisited.Count > 0)
        {
            Node curr = unvisited.Dequeue();
            if (curr.neighbors.Count < maxAdjacents)
            {
                IEnumerable<Node> closest = GetClosestUnvisited(Nodes, maxAdjacents, curr);
                foreach (Node e in closest)
                {
                    e.neighbors.Add(curr);
                    unvisited.Enqueue(e);
                }
                curr.neighbors.AddRange(closest);
            }
        }
    }

    private IEnumerable<Node> GetClosestUnvisited(List<Node> nodes, int v, Node curr)
    {
        List<Node> sol = new List<Node>();
        SortedDictionary<float, Node> dict = new SortedDictionary<float, Node>();
        // Adding to the sortedDict takes log(n) time.
        foreach (Node n in Nodes)
        {
            if (n.neighbors.Count < v && !n.neighbors.Contains(curr))
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
        MatchAdjacents(4);
        foreach (Node e in Nodes)
        {
            Vector3 vertex = new Vector3(e.x, e.y, 1);
            Instantiate(nodeSprite, cam.ViewportToWorldPoint(vertex), Quaternion.identity);
        }
    }

    private void DrawEdges()
    {
        if (cam != null)
        {
            edges = new List<GameObject>();
            bool[] drawn = new bool[Nodes.Count];
            foreach (Node e in Nodes)
            {
                Vector3 vertex = new Vector3(e.x, e.y, 1);
                foreach (Node n in e.neighbors)
                {
                    if (!drawn[n.Id])
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
                        lr.SetPosition(0, cam.ViewportToWorldPoint(new Vector3(e.x, e.y, 1)));
                        lr.SetPosition(1, cam.ViewportToWorldPoint(new Vector3(n.x, n.y, 1)));
                        edges.Add(empty);
                    }
                }
                drawn[e.Id] = true;
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
    
    private void GenerateBlips()
    {
        foreach(Node n in Nodes)
        {
            // Make random direction to eject node
            Vector2 x = (new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f))).normalized;
            Vector3 ejectDirection = new Vector3(x.x, x.y, 1);
            // Create Blip
            Blip toAdd = new Blip(Blip.State.Ejection, n.Player, ejectDirection);
            toAdd.OriginDest[0] = n;

            blipsInScene.Add(toAdd);
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
        DrawEdges();
        GenerateBlips();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
