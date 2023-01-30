using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBookPool : MonoBehaviour
{
    //Two Arrays of Audio Clips that can be given to audiobook manager to play
    //Array 1 is from before the bridge building
    //Array 2 is from after the bridge building

    //function to set the bool "Bridge Built"
    //timer "audio entrance trigger countdown"
    //function to tell script that the player stepped into the area (is getting called by trigger) 
    // -> this function asks the audiobook manager if an audiopool is already active 
    //-> if no audiopool is active, check if pool should instant play: yes = play next audio clip immediately
    //no: 
    //if "audio entrance trigger countdown" = 0: 
    // -> calculates random percentage if the audio event happens
    //      ->yes: check for bridge bool, then take next audioclip from according array (count up)
    //              tell the audiomanager that you're active now, tell audioclip in how many seconds you will stop again (get clip length)
    [SerializeField] private AudioClip[] m_PreBridgeAudioClips;
    [SerializeField] private AudioClip[] m_PostBridgeAudioClips;
    private int m_CurrentAudioClipNumber = 0;
    [SerializeField] private bool m_BridgeHasBeenBuilt = false; public bool bridgeHasBeenBuilt { get { return m_BridgeHasBeenBuilt; } set { m_BridgeHasBeenBuilt = value; } }
    private AudioBookManager m_AudioBookManager; 
    private void Awake()
    {
        m_AudioBookManager = Singletons.audioBookManager;
        m_AudioBookManager.RegisterAudioBookPool(this);
    }

    public void TryPlayingNextAudioClip()
    {
        if (m_AudioBookManager.someAudioPoolIsCurrentlyPlaying)
        {
            Debug.LogWarning("Another Audio pool is currently playing!");
            return;
        }

        if (!m_AudioBookManager.playNextAudioClipInstantly)
        {
            Debug.LogWarning("Cannot play clip at this moment, try again later!");
            return;
        }
        
        GetAndPlayNextAudioClip();
        
    }

    private void GetAndPlayNextAudioClip()
    {
        if (m_BridgeHasBeenBuilt)
        {
            if(m_CurrentAudioClipNumber < m_PostBridgeAudioClips.Length)
            {
                m_AudioBookManager.PlayAudioClip(m_PostBridgeAudioClips[m_CurrentAudioClipNumber]);
                
                m_CurrentAudioClipNumber++;
            }

        }
        else
        {
            if (m_CurrentAudioClipNumber < m_PreBridgeAudioClips.Length)
            {
                m_AudioBookManager.PlayAudioClip(m_PreBridgeAudioClips[m_CurrentAudioClipNumber]);
                m_CurrentAudioClipNumber++;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
