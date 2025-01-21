using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : SingletonMonoBehaviour<ObjectPooler>
{
    [System.Serializable]
    public class ObjectPool
    {
        //were it so easy
        [Tooltip("Friendly name to call pool easily")]
        public string tag;

        //[AllowNesting]
        [Tooltip("Prefab to load")]
        public GameObject prefab;

        [HideInInspector]
        [Tooltip("List of all objects spawned for the pool.")]
        public List<GameObject> currentlySpawnedObjs = new List<GameObject>();

        [Tooltip("Max number of objects to be loaded")]
        public int size;

        [Tooltip("Create pool of objects immediatley on start.")]
        public bool instantiateOnAwake = true;

        //Audio Pooliing Specific Items

        [HideInInspector]
        public bool audioPool = false;

        [HideInInspector]
        public SoundClip poolSound;

    }

    #region Fields
    public List<ObjectPool> pools;

    private List<ObjectPool> currentPools = new List<ObjectPool>();

    #endregion

    #region Properties

    public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();


    #endregion

    #region Unity Methods

    new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        foreach (ObjectPool pool in pools)
        {
            InstantiatePool(pool, this.gameObject);
        }
    }


    #endregion

    #region Utility Methods

    /// <summary>Instantiates a pool of objects and adds it to the pool dictionary.
    /// </summary>
    /// <param name="pool"> Object pool to add </param>
    public void InstantiatePool(ObjectPool pool, GameObject parent)
    {
        Queue<GameObject> objectPool;

        if (ObjectPooler.Instance.poolDictionary.ContainsKey(pool.tag))
        {
            objectPool = ObjectPooler.Instance.poolDictionary[pool.tag];
        }
        else
        {
            objectPool = new Queue<GameObject>();
        }

        for (int i = 0; i < pool.size; i++)
        {
            //Debug.Log("spawning " + pool.prefab.name);
            GameObject obj = Instantiate(pool.prefab);

            obj.name = pool.prefab.name;

            obj.transform.parent = parent.transform;

            obj.SetActive(false);
            objectPool.Enqueue(obj);
            pool.currentlySpawnedObjs.Add(obj);
        }

        //pools.Add(pool);

        currentPools.Add(pool);

        //Debug.Log(pool.tag + "added to pools");

        if (!ObjectPooler.Instance.poolDictionary.ContainsKey(pool.tag))
        {

            ObjectPooler.Instance.poolDictionary.Add(pool.tag, objectPool);
        }
    }

    /// <summary>Removes object pool from the list of pools and removes all objects instantiated for the pool.
    /// </summary>
    /// <param name="pool"> Object pool to find </param>
    /// <returns> Object spawned from pool </returns>
    public void RemovePool(ObjectPool pool)
    {
        foreach (GameObject obj in pool.currentlySpawnedObjs)
        {
            Destroy(obj.gameObject);
        }

        poolDictionary.Remove(pool.tag);
    }

    public ObjectPool GetPoolByName(string name)
    {
        ObjectPool poolReturn = null;


        foreach (ObjectPool pool in currentPools)
        {
            if (name == pool.prefab.name)
            {
                poolReturn = pool;
                return poolReturn;
            }
        }

        Debug.LogWarning("no pool could be found with name: " + name);


        return poolReturn;
    }


    public void ReorderQueue(string tag)
    {
        Queue<GameObject> queue = poolDictionary[tag];

        List<GameObject> objs = new List<GameObject>();

        foreach (GameObject obj in queue)
        {
            if (!obj.activeSelf)
            {
                objs.Add(obj);
            }
        }
        foreach (GameObject obj in queue)
        {
            if (obj.activeSelf)
            {
                objs.Add(obj);
            }
        }

        poolDictionary[tag].Clear();

        foreach (GameObject obj in objs)
        {
            poolDictionary[tag].Enqueue(obj);
        }


    }

    /// <summary>Spawns an object from inside of an instantiated object pool.
    /// </summary>
    /// <param name="tag"> Object pool to find </param>
    /// <param name="position"> Position to place object. </param>
    /// <param name="rotation"> Object rotation. </param>
    /// <returns> Object spawned from pool </returns>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Peek();

        if (objectToSpawn.activeSelf)
        {
            // object is already in use, we need to grow the pool
            ObjectPool pool = GetPoolByName(objectToSpawn.name);

            if (pool != null)
            {

                for (int i = 0; i < pool.size / 2; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);


                    obj.name = pool.prefab.name;

                    obj.transform.parent = objectToSpawn.transform.parent;

                    obj.SetActive(false);
                    poolDictionary[tag].Enqueue(obj);
                    pool.currentlySpawnedObjs.Add(obj);

                    ReorderQueue(tag);

                }

                pool.size += pool.size / 2;
            }


            objectToSpawn = poolDictionary[tag].Dequeue();
        }
        else
        {
            objectToSpawn = poolDictionary[tag].Dequeue();
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        PooledObject pooledObj = objectToSpawn.GetComponent<PooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawned();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    #endregion
}
