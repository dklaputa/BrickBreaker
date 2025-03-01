using System.Collections;
using UnityEngine;

public class BrickParticleScript : MonoBehaviour
{
    private Color color;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem particle;

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
        StartCoroutine(Remove());
    }

    private IEnumerator Remove()
    {
        yield return new WaitForSeconds(.3f);
        if (!particle.isStopped) particle.Stop();
        gameObject.SetActive(false);
    }
}