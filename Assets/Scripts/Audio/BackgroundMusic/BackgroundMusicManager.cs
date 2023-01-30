using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{

    [SerializeField]
    private string m_BackgroundMusicName; 
    [SerializeField]
    private GameObject m_BackgroundSoundSourceGameObject;
    [SerializeField]
    private AudioSrc[] m_BackgroundSoundtrackSources;
    [SerializeField]
    private AudioSource[] m_BackgroundSoundAudioSources = new AudioSource[2];
    private int m_CurrentAudioSource = 0;

    [SerializeField] private float m_SwitchingTreshold;
    [SerializeField] private AudioTransition m_CurrentTransition;
    [SerializeField] private bool m_TransitionIsDone = true;

    private float m_FadeTime;
    private float m_PreferredVolume;
    private float[] m_CurrentFadeVelocity = new float[2];

    [SerializeField] private AudioTransitionInfo m_NightMusicTransitionInfo;
    [SerializeField] private AudioTransitionInfo m_DayMusicTransitionInfo;
    [SerializeField] private AudioTransitionInfo m_CurrentAudioTransitionInfo;

    [SerializeField] private bool m_StopPlaying = false;

    public float GetCurrentMusicVolume() 
    { 
        if (m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2] != null) 
        { 
            return m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2].volume; 
        }
        return 0.0f; 
    }

    private void Awake()
    {
        m_BackgroundSoundAudioSources[0] = m_BackgroundSoundSourceGameObject.AddComponent<AudioSource>();
        m_BackgroundSoundAudioSources[1] = m_BackgroundSoundSourceGameObject.AddComponent<AudioSource>();

        m_BackgroundSoundAudioSources[0].spatialBlend = 1.0f;
        m_BackgroundSoundAudioSources[(m_CurrentAudioSource + 1) % 2].spatialBlend = 1.0f;

        m_BackgroundSoundAudioSources[0].loop = true;
        m_BackgroundSoundAudioSources[(m_CurrentAudioSource + 1) % 2].loop = true;

        m_BackgroundSoundAudioSources[0].volume = 1.0f;
        m_BackgroundSoundAudioSources[(m_CurrentAudioSource + 1) % 2].volume = 0.0f;
    }
    // Start is called before the first frame update
    void Start()
    {
        //PlaySound(m_BackgroundMusicName);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFading();
    }

    

    public void TryStartingTransition(Daytime daytime)
    {
        
        switch (daytime)
        {
            case Daytime.Evening:
                m_CurrentAudioTransitionInfo = m_NightMusicTransitionInfo;
                break;
            case Daytime.Night:
                m_CurrentAudioTransitionInfo = m_DayMusicTransitionInfo;
                break;
        }


        if (!m_TransitionIsDone)
        {
            Debug.LogWarning("New transition cannot be started while other transition is still in progress!");
            return;
        }

        if (m_CurrentAudioTransitionInfo.clip == m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2].clip && m_CurrentAudioTransitionInfo.transition == AudioTransition.CrossFade)
        {
            Debug.Log("New Clip is same as old clip. No Crossfade Transition.");
            return;
        }

        //the current sound should always play on the first audio source
        //copy the new source to the second audio source
        m_BackgroundSoundAudioSources[(m_CurrentAudioSource + 1) % 2].clip = m_CurrentAudioTransitionInfo.clip;




        m_PreferredVolume   = m_CurrentAudioTransitionInfo.targetVolume;
        m_FadeTime          = m_CurrentAudioTransitionInfo.fadeDuration;
        m_CurrentTransition = m_CurrentAudioTransitionInfo.transition;
        m_CurrentFadeVelocity[(m_CurrentAudioSource + 1) % 2] = 0.0f;

        m_TransitionIsDone = false;
    }

    private void UpdateFading()
    {
        if(m_CurrentTransition == AudioTransition.CrossFade)
        {
            AudioSource currentlyPlayingAudioSource = m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2];
            AudioSource nextPlayingAudioSource = m_BackgroundSoundAudioSources[(m_CurrentAudioSource + 1) % 2];

            bool crossFadeAFinished = AudioHelper.SingleFade(currentlyPlayingAudioSource, 0.0f, ref m_CurrentFadeVelocity[m_CurrentAudioSource % 2], m_FadeTime, m_SwitchingTreshold);
            bool crossFadeBFinished = AudioHelper.SingleFade(nextPlayingAudioSource, m_PreferredVolume, ref m_CurrentFadeVelocity[(m_CurrentAudioSource + 1) % 2], m_FadeTime, m_SwitchingTreshold);

            if (crossFadeAFinished && crossFadeBFinished)
            {
                currentlyPlayingAudioSource.Stop();
                m_CurrentAudioSource++;
                m_CurrentTransition = AudioTransition.None;
                m_TransitionIsDone = true;
            }
        }

        if(m_CurrentTransition == AudioTransition.FadeOut)
        {
            bool transitionFinished = AudioHelper.SingleFade(m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2], m_PreferredVolume, ref m_CurrentFadeVelocity[m_CurrentAudioSource % 2], m_FadeTime, m_SwitchingTreshold);

            if (transitionFinished)
            {
                m_CurrentTransition = AudioTransition.None;
                m_TransitionIsDone = true;
                m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2].volume = m_PreferredVolume;
            }
        }
        
    }

    public void SetNewVolume(float musicVolume)
    {
        if(m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2]!= null)
        {
            m_CurrentTransition = AudioTransition.FadeOut;
            //m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2].volume = musicVolume;
            AudioHelper.SingleFade(m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2], musicVolume, ref m_CurrentFadeVelocity[m_CurrentAudioSource % 2], m_FadeTime, m_SwitchingTreshold);
            m_PreferredVolume = musicVolume;
        }
        
    }

    public void PlaySound(string name)
    {

        //BAU HIER NEN FADE EIN

        AudioSrc newSource = new AudioSrc();
        for (int i = 0; i < m_BackgroundSoundtrackSources.Length; i++)
        {
            if (m_BackgroundSoundtrackSources[i].audioSrcName == name)
            {
                newSource = m_BackgroundSoundtrackSources[i];
            }
        }

        if (newSource == null)
        {
            Debug.LogWarning("Sound >>" + name + "<< was not found");
        }
        else
        {
            m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2].volume = newSource.volume;
            m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2].clip = newSource.soundClip;
            m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2].loop = newSource.loop;
            m_BackgroundSoundAudioSources[m_CurrentAudioSource % 2].Play();
        }

    }
}
