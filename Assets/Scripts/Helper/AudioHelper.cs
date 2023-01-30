using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioHelper
{
    public static bool SingleFade(AudioSource audioSource, float preferredVolume, ref float currentFadeVelocity, float fadeTime, float fadeFinishTreshold)
    {
        bool finishedTransitioning = false;
        if (audioSource.isPlaying == false)
        {
            audioSource.Play();
        }
        audioSource.volume = Mathf.SmoothDamp(audioSource.volume, preferredVolume, ref currentFadeVelocity, fadeTime);
        if (Mathf.Abs(preferredVolume - audioSource.volume) < fadeFinishTreshold)
        {

            finishedTransitioning = true;
        }

        return finishedTransitioning;
    }
}
