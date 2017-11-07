using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickParticleScript : MonoBehaviour
{
    private Color color;
    private ParticleSystem particle;
    private ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule;

    public void SetColor(Color particleColor)
    {
        color = particleColor;
    }

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        colorOverLifetimeModule = particle.colorOverLifetime;
    }

    private void OnEnable()
    {
        colorOverLifetimeModule.color = color;
        particle.Play();
        Invoke("Destory", .3f);
    }

    private void Destory()
    {
        if (!particle.isStopped) particle.Stop();
        gameObject.SetActive(false);
    }
}