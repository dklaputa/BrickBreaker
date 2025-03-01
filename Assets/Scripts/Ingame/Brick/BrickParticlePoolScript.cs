using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Brick particle generator.
/// </summary>
public class BrickParticlePoolScript : ObjectPoolBehavior
{
    public static BrickParticlePoolScript instance;

    private void Awake()
    {
        instance = this;
    }
    
    private void OnDestroy()
    {
        instance = null;
    }

    public void ShowParticle(Vector2 position, Color color)
    {
        GameObject particle = GetAvailableObject();
        particle.transform.position = position;
        particle.GetComponent<BrickParticleScript>().SetColor(color);
        particle.SetActive(true);
    }
}