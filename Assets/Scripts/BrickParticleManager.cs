using System.Collections.Generic;
using UnityEngine;

public class BrickParticleManager : MonoBehaviour
{
    public int initialPoolSize;
    public GameObject brickParticlePrefab;
    public static BrickParticleManager instance;

    private List<GameObject> particlePool;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        particlePool = new List<GameObject>();
        for (var i = 0; i < initialPoolSize; i++)
        {
            particlePool.Add(Instantiate(brickParticlePrefab, transform));
        }
    }

    public void ShowParticle(Vector2 position, Color color)
    {
        var particle = GetAvailableParticle();
        particle.transform.position = position;
        particle.GetComponent<BrickParticleScript>().SetColor(color);
        particle.SetActive(true);
    }

    private GameObject GetAvailableParticle()
    {
        foreach (var particle in particlePool)
        {
            if (!particle.activeInHierarchy)
            {
                return particle;
            }
        }
        var newParticle = Instantiate(brickParticlePrefab, transform);
        particlePool.Add(newParticle);
        return newParticle;
    }
}