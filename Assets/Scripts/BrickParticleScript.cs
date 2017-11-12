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
        Invoke("Destory", .3f);
    }

    private void Destory()
    {
        if (!particle.isStopped) particle.Stop();
        gameObject.SetActive(false);
    }
}