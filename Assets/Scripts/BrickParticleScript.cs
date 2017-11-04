using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickParticleScript : MonoBehaviour
{
    private Color color;
    private ParticleSystem particleSystem;
    private ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule;

    public void SetColor(Color particleColor)
    {
        color = particleColor;
    }

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        colorOverLifetimeModule = particleSystem.colorOverLifetime;
    }

    private void OnEnable()
    {
        colorOverLifetimeModule.color = color;
        particleSystem.Play();
        Invoke("Destory", .3f);
    }

    private void Destory()
    {
        if (!particleSystem.isStopped) particleSystem.Stop();
        gameObject.SetActive(false);
    }
}