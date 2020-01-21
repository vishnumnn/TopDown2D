using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ObjectPoolContainer : MonoBehaviour
{

    public static ObjectPoolContainer sharedInstance;

    /// <summary>
    /// Publicly exposed enumerable that allows for variable numbers of distinct prefabs to be pooled. The alternative to this would be to 
    /// hard code the different prefabs into the dictionary object within the InstantiateAllObjects method, which is not adaptable.
    /// </summary>
    [SerializeField]
    private List<PoolingObject> prefabs;
    private Dictionary<PoolingObject, List<GameObject>> ObjectPools { get; set; }
    public GameObject RetrieveObjectByTag(string tag)
    {
        foreach (PoolingObject obj in ObjectPools.Keys)
        {
            if (obj.objectToPool.tag.Equals(tag))
            {
                List<GameObject> pool = null;
                ObjectPools.TryGetValue(obj, out pool);
                foreach (GameObject gameObj in pool)
                {
                    if (!gameObj.activeInHierarchy)
                    {
                        gameObj.SetActive(true);
                        return gameObj;
                    }
                }
                if (obj.expandable)
                {
                    GameObject newGameObject = Instantiate(obj.objectToPool);
                    newGameObject.SetActive(true);
                    pool.Add(newGameObject);
                    return newGameObject;
                }
                else
                {
                    Debug.Log("Have activated all available gameObjects of this tag");
                }
                break;
            }
        }
        return null;
    }
    public List<GameObject> GetAllObjectsOfTag(string tag)
    {
        foreach(PoolingObject obj in ObjectPools.Keys)
        {
            if (obj.objectToPool.tag.Equals(tag))
            {
                List<GameObject> list;
                ObjectPools.TryGetValue(obj, out list);
                return list;
            }
        }
        return null;
    }
    public void InstantiateAllObjects()
    {
        ObjectPools = new Dictionary<PoolingObject, List<GameObject>>();
        foreach(PoolingObject obj in prefabs)
        {
            List<GameObject> pool = new List<GameObject>();
            for(int i = 0; i < obj.poolSize; i++)
            {
                GameObject toAdd = Instantiate(obj.objectToPool);
                toAdd.SetActive(false);
                pool.Add(toAdd);
            }
            ObjectPools.Add(obj, pool);
            Debug.Log(ObjectPools.Keys.ToList()[0].ToString());
        }
    }

    private void OnEnable()
    {
        
    }

    private void Awake()
    {
        sharedInstance = this;
        InstantiateAllObjects();
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
