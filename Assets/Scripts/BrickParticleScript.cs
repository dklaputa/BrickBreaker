using System.Collections;
using UnityEngine;

public class BrickParticleScript : MonoBehaviour
{
    private Color color;
    private ParticleSystem particle;
    private ParticleSystem.MainModule mainModule;

    public void SetColor(Color particleColor)
    {
        color = particleColor;
    }

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        mainModule = particle.main;
    }

    private void OnEnable()
    {
        mainModule.startColor = color;
        particle.Play();
        StartCoroutine("Destory");
    }

    private IEnumerator Destory()
    {
        yield return new WaitForSeconds(.3f);
        if (!particle.isStopped) particle.Stop();
        gameObject.SetActive(false);
    }
}