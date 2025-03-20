using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool _instance;
    private List<GameObject> objects = new List<GameObject>();
    [SerializeField] private int amountToPool = 150;
    [SerializeField] private GameObject objectPrefab;
    private void Awake()
    {
        _instance = this;
    }

    public static ObjectPool instance
    {
        get
        {
            return _instance;
        }
    }

    private void Start()
    {
        for (int i = 0; i < amountToPool; i++) //Instantiate objects into pool
        {
            CreateObject();
        }
    }

    public GameObject GetPooledObject() //Used by scripts to access an inactive pooled object
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (!objects[i].activeInHierarchy)
            {
                return objects[i];
            }
        }

        GameObject obj = CreateObject(); //If it couldnt find an inactive object it will instantiate a new one for the pool and return that
        return obj;
    }

    private GameObject CreateObject()
    {
        GameObject obj = Instantiate(objectPrefab);
        obj.transform.parent = transform;
        obj.SetActive(false);
        objects.Add(obj);
        return obj;
    }
}
