using System.Collections.Generic;
using UnityEngine;

public class BrickParticleManager : ObjectPoolBehavior
{ 
    public static BrickParticleManager instance; 

    private void Awake()
    {
        instance = this;
    } 

    public void ShowParticle(Vector2 position, Color color)
    {
        var particle = GetAvailableObject();
        particle.transform.position = position;
        particle.GetComponent<BrickParticleScript>().SetColor(color);
        particle.SetActive(true);
    } 
}