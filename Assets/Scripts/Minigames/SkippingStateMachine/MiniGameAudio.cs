using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameAudio : MonoBehaviour
{
    [SerializeField] AudioClip[] missedSplashSound;
    [SerializeField] AudioClip[] hitSplashSound;
    [SerializeField] AudioSource source;


    // Start is called before the first frame update
    
    private void Awake()
    {
        SkippingMinigame.SkippingSystem.OnStoneMissed += MissedSound;
        SkippingMinigame.SkippingSystem.OnStoneSkipped += HitSound;
    }

    private void OnDestroy()
    {
        SkippingMinigame.SkippingSystem.OnStoneMissed -= MissedSound;
        SkippingMinigame.SkippingSystem.OnStoneSkipped -= HitSound;
    }

    void MissedSound()
    {
        int i = Random.Range(0, missedSplashSound.Length - 1);
        AudioClip clip = missedSplashSound[i];

        source.Stop();
        source.clip = clip;
        source.pitch = Random.Range(0.5f, 1.5f);

        source.Play();

    }

    void HitSound()
    {
        int i = Random.Range(0, missedSplashSound.Length - 1);
        AudioClip clip = hitSplashSound[i];

        source.Stop();
        source.clip = clip;
        source.pitch = Random.Range(0.5f, 1.5f);

        source.Play();
    }
}
