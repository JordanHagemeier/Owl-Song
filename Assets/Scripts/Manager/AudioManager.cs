using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSrc[] sources;
    [SerializeField]
    public static AudioManager audiomanager;
    [SerializeField] private GameObject m_MainCamera;


    private void Awake()
    {

        if (audiomanager == null)
        {
            audiomanager = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        

        
    }


   //manager for all three? 
   //

 
   


    //public void AddAudioSource(AudioSource newAudioSrc)
    //{
    //    audioSources.Add(newAudioSrc);
    //}



    public void PlaySound(string name)
    {
        //AudioSource source = audioSources.Find(r => r.clip.ToString() == name + " (UnityEngine.AudioSource)");
        AudioSource source = m_MainCamera.GetComponent<AudioSource>();
        AudioClip oldClip = source.clip;

        AudioSrc newSource = new AudioSrc(); 
        for(int i = 0; i < sources.Length; i++)
        {
            if(sources[i].audioSrcName == name)
            {
                newSource = sources[i];
            }
        }
        
        if(newSource == null)
        {
            Debug.LogWarning("Sound >>" + name + "<< was not found");
        }
        else
        {
            source.volume   = newSource.volume;
            source.clip     = newSource.soundClip;
            source.loop     = newSource.loop;
            source.Play();
        }

    }

    //public void AdjustOverallVolume(System.Single newVolume)
    //{
    //    foreach (AudioSource s in audioSources)
    //    {
    //        s.volume += newVolume;
    //    }
    //}
}
