using System.Collections.Generic;
using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Object pool. Object will not be destroyed after using - We deactivate the object and cache it into the pool.
/// </summary>
public class ObjectPoolBehavior : MonoBehaviour
{
    public int initialPoolSize;

    protected List<GameObject> pool;
    public GameObject prefab;


    // Use this for initialization
    protected void Start()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < initialPoolSize; i++) pool.Add(Instantiate(prefab, transform));
    }


    protected GameObject GetAvailableObject()
    {
        foreach (GameObject go in pool)
            if (!go.activeInHierarchy)
                return go;
        GameObject newGameObject = Instantiate(prefab, transform);
        pool.Add(newGameObject);
        return newGameObject;
    }
}