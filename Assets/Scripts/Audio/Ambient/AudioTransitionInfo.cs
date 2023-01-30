using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Audio / Audio Transition Info")]
public class AudioTransitionInfo : ScriptableObject
{

    public AudioClip clip;
    public float fadeDuration;
    public AudioTransition transition;
    public float targetVolume;



};
