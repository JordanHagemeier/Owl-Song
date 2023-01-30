using UnityEngine.Audio;
using UnityEngine;


[System.Serializable]
public class AudioSrc
{
   
    public string           audioSrcName;
    public AudioClip        soundClip;
    [Range(0.0f, 1.0f)]
    public float            volume;
    public bool             loop;




}
