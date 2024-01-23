using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    private static ObjectPool instance;
    public static ObjectPool Instance
    {
        get { return instance; }
    }

    [SerializeField]
    private GameObject poolableObject;

    [SerializeField]
    private int poolSize;

    [SerializeField]
    private bool willGrow = true;

    public List<GameObject> pooledObjects;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(poolableObject);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (willGrow)
        {
            GameObject obj = Instantiate(poolableObject);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pooledObjects.Add(obj);
            return obj;
        }

        Debug.LogError("Object pool for " + poolableObject.name + " is not of sufficient size.");

        return null;
    }
}