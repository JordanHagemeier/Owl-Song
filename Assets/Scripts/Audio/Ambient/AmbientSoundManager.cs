using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AudioTransition
{
    FadeIn,
    FadeOut,
    CrossFade,
    FadeToPreferredVolume,
    None
}




public class AmbientSoundManager : MonoBehaviour
{

    //TODO:
    //Original & New Audio source durch Array mit zwei Einträgen ersetzen, dann immer rummodulon 
    [SerializeField]
    private GameObject m_AmbientSoundSourceGameObject;
    [SerializeField]
    private AudioSrc[]  m_AmbientSoundtrackSources;
    [SerializeField]
    private AudioSource[] m_AmbientSoundAudioSources = new AudioSource[2];
    private int m_CurrentAudioSource = 0;

    [SerializeField]
    private float           m_SwitchingTreshold;
    [SerializeField]
    private AudioTransition m_CurrentTransition;
    private bool m_TransitionIsDone = true;

    private float           m_FadeTime;
    private float           m_PreferredVolume;
    private float[]         m_CurrentFadeVelocity = new float[2];

    [SerializeField] AudioTransitionInfo m_InitialAmbient;

    [SerializeField] private GameObject m_AmbientAudioParent;
    [SerializeField] private AudioSource[] m_AmbientAudioSources; 


    void Awake()
    {
        m_AmbientSoundAudioSources[0] = m_AmbientSoundSourceGameObject.AddComponent<AudioSource>();
        m_AmbientSoundAudioSources[1] = m_AmbientSoundSourceGameObject.AddComponent<AudioSource>();

        m_AmbientSoundAudioSources[0].spatialBlend   = 1.0f;
        m_AmbientSoundAudioSources[(m_CurrentAudioSource +1) % 2].spatialBlend        = 1.0f;

        m_AmbientSoundAudioSources[0].loop   = true;
        m_AmbientSoundAudioSources[(m_CurrentAudioSource +1) % 2].loop        = true;

        m_AmbientSoundAudioSources[0].volume = 0.0f;
        m_AmbientSoundAudioSources[(m_CurrentAudioSource +1) % 2].volume      = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        //TryStartingTransition(m_AmbientSoundtrackSources[0].soundClip, 3.0f, AudioTransition.FadeIn, 0.0f, 1.0f);
        //TryStartingTransition(m_InitialAmbient);
        PlayCurrentAmbientSounds();

        m_AmbientAudioSources = m_AmbientAudioParent.GetComponentsInChildren<AudioSource>();
    }

    public void StartAllAmbientSounds()
    {
        for (int i = 0; i < m_AmbientAudioSources.Length; i++)
        {
            m_AmbientAudioSources[i].Play();
        }
    }

    public void StopAllAmbientSounds()
    {
        for(int i = 0; i < m_AmbientAudioSources.Length; i++)
        {
            m_AmbientAudioSources[i].Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInput();
        UpdateFading();

    }

    private void CheckForInput()
    {
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    TryStartingTransition(m_AmbientSoundtrackSources[(m_CurrentAudioSource + 1) % 2].soundClip, 3.0f, AudioTransition.CrossFade, 0.0f, 1.0f);
        //}

        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    TryStartingTransition(m_FadeOutInfoTest);
        //}
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    TryStartingTransition(m_AmbientSoundtrackSources[m_CurrentAudioSource % 2].soundClip, 3.0f, AudioTransition.FadeToPreferredVolume, 1.0f, 0.5f);
        //}
    }


    //Fades:
    //Fade in: no original soundtrack, only new soundtrack, starts at 0 and goes to target volume
    //Fade out: original soundtrack, no new soundtrack, starts at original volume and goes to 0
    //Cross Fade: original soundtrack, new soundtrack, orignal starts at original volume and goes to 0, new soundtrack starts at 0 and goes to target volume 
    //Fade To preferred Volume: original soundtrack, no new soundtrack, starts at original volume and goes to target volume

    public void TryStartingTransition(AudioTransitionInfo info)
    {

        if (!m_TransitionIsDone)
        {
            Debug.LogWarning("New transition cannot be started while other transition is still in progress!");
            return;
        }

        if(info.clip == m_AmbientSoundAudioSources[m_CurrentAudioSource % 2].clip && info.transition == AudioTransition.CrossFade)
        {
            Debug.Log("New Clip is same as old clip. No Crossfade Transition.");
            return;
        }

        //the current sound should always play on the first audio source
        //copy the new source to the second audio source
        m_AmbientSoundAudioSources[(m_CurrentAudioSource +1) % 2].clip      = info.clip;
        if(info.transition == AudioTransition.FadeIn)
        {
            m_AmbientSoundAudioSources[(m_CurrentAudioSource + 1) % 2].volume = 0.0f;
        }
        
       
        
        m_PreferredVolume                   = info.targetVolume;
        m_FadeTime                          = info.fadeDuration;
        m_CurrentTransition                 = info.transition;
        m_CurrentFadeVelocity[(m_CurrentAudioSource + 1) % 2] = 0.0f;

        m_TransitionIsDone = false;
    }

    private void UpdateFading()
    {
        //if there is no original soundtrack, but a new one, its a fade in
        //if there is an original soundtrack, but no new one, its a fade out or fade to preferred volume -> bool for preferred volume fade?
        //if there is an original soundtrack and a new one, its a cross fade 
        AudioSource currentlyPlayingAudioSource = m_AmbientSoundAudioSources[m_CurrentAudioSource % 2];
        AudioSource nextPlayingAudioSource      = m_AmbientSoundAudioSources[(m_CurrentAudioSource + 1) % 2];
        bool finishedTransition = false;
        switch (m_CurrentTransition)
        {
            case AudioTransition.None:

                break;
            case AudioTransition.FadeIn:
                //fade in
                //raise volume over time to preferred Volume

                finishedTransition = AudioHelper.SingleFade(nextPlayingAudioSource, m_PreferredVolume, ref m_CurrentFadeVelocity[(m_CurrentAudioSource + 1) % 2], m_FadeTime, m_SwitchingTreshold);
                if (finishedTransition)
                {
                    m_CurrentAudioSource++;
                    m_CurrentTransition = AudioTransition.None;
                    m_TransitionIsDone = true;
                }
                
                    
                   
                
                break;

            case AudioTransition.FadeOut:
                finishedTransition = AudioHelper.SingleFade(currentlyPlayingAudioSource, m_PreferredVolume, ref m_CurrentFadeVelocity[m_CurrentAudioSource % 2], m_FadeTime, m_SwitchingTreshold);
                if (finishedTransition)
                {
                    currentlyPlayingAudioSource.Stop();
                    m_CurrentAudioSource++;
                    m_CurrentTransition = AudioTransition.None;
                    m_TransitionIsDone = true;
                }

                
                break;

            case AudioTransition.CrossFade:

                bool crossFadeAFinished = AudioHelper.SingleFade(currentlyPlayingAudioSource, 0.0f, ref m_CurrentFadeVelocity[m_CurrentAudioSource % 2], m_FadeTime, m_SwitchingTreshold);
                bool crossFadeBFinished = AudioHelper.SingleFade(nextPlayingAudioSource, m_PreferredVolume, ref m_CurrentFadeVelocity[(m_CurrentAudioSource + 1) % 2], m_FadeTime, m_SwitchingTreshold);

                if(crossFadeAFinished && crossFadeBFinished)
                {
                    currentlyPlayingAudioSource.Stop();
                    m_CurrentAudioSource++;
                    m_CurrentTransition = AudioTransition.None;
                    m_TransitionIsDone = true;
                }
               
                break;

            case AudioTransition.FadeToPreferredVolume:

                finishedTransition = AudioHelper.SingleFade(currentlyPlayingAudioSource, m_PreferredVolume, ref m_CurrentFadeVelocity[m_CurrentAudioSource % 2], m_FadeTime, m_SwitchingTreshold);
                if (finishedTransition)
                {
                    m_CurrentTransition = AudioTransition.None;
                    m_TransitionIsDone = true;
                }
                
                break;
        }
           
         

    }


    private void PlayCurrentAmbientSounds()
    {
       m_AmbientSoundAudioSources[m_CurrentAudioSource % 2].Play();
        m_AmbientSoundAudioSources[(m_CurrentAudioSource +1) % 2].Play();
    }

    public void PlaySound(string name)
    {
     
        AudioSrc newSource = new AudioSrc();
        for (int i = 0; i < m_AmbientSoundtrackSources.Length; i++)
        {
            if (m_AmbientSoundtrackSources[i].audioSrcName == name)
            {
                newSource = m_AmbientSoundtrackSources[i];
            }
        }

        if (newSource == null)
        {
            Debug.LogWarning("Sound >>" + name + "<< was not found");
        }
        else
        {
           m_AmbientSoundAudioSources[m_CurrentAudioSource % 2].volume = newSource.volume;
           m_AmbientSoundAudioSources[m_CurrentAudioSource % 2].clip = newSource.soundClip;
           m_AmbientSoundAudioSources[m_CurrentAudioSource % 2].loop = newSource.loop;
           m_AmbientSoundAudioSources[m_CurrentAudioSource % 2].Play();
        }

    }
}
