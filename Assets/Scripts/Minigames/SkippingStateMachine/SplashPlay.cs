using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashPlay : MonoBehaviour
{
    [SerializeField] ParticleSystem splashParticles;

    // Start is called before the first frame update
    private void Awake()
    {
        SkippingMinigame.SkippingSystem.OnStoneSkipped += Splash;
    }

    // Update is called once per frame
    void Splash()
    {
        splashParticles.Play();
    }

    private void OnDestroy()
    {
        SkippingMinigame.SkippingSystem.OnStoneSkipped -= Splash;
    }
}
