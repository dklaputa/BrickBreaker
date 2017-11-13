using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolBehavior : MonoBehaviour
{
    public int initialPoolSize;
    public GameObject prefab;

    protected List<GameObject> pool;


    // Use this for initialization
    protected virtual void Start()
    {
        pool = new List<GameObject>();
        for (var i = 0; i < initialPoolSize; i++)
        {
            pool.Add(Instantiate(prefab, transform));
        }
    }


    protected GameObject GetAvailableObject()
    {
        foreach (var go in pool)
        {
            if (!go.activeInHierarchy)
            {
                return go;
            }
        }
        var newGameObject = Instantiate(prefab, transform);
        pool.Add(newGameObject);
        return newGameObject;
    }
}