using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBookManager : MonoBehaviour
{
    //___AUDIOBOOK IMPLEMENTATION____
    //List of all audio book pools
    //public function to self-register each audio book pool 
    //bool "some pool is playing" -> get, no set (only set by function)
    //function to tell the audiomanager that an audio pool is now playing and for how long, sets "some pool is playing" to yes
    //timer "countdown till audio pool stops playing"
    //-> timer = 0; set "some pool is playing" to no

    //timer "time countdown since last audio clip" -> timer = 0 = sets bool "instant play clip on next triggered audio pool" to true
    //public function to tell the audiomanager that the bridge was built
    //if bridge was built: call each audio pool and change their status to "bridge built"
    // Start is called before the first frame update
    [SerializeField]    private List<AudioBookPool> m_AudioBookPools;

    [SerializeField]    private bool                m_SomeAudioPoolIsCurrentlyPlaying = false; public bool someAudioPoolIsCurrentlyPlaying { get { return m_SomeAudioPoolIsCurrentlyPlaying; } }
                        private float               m_CurrentlyPlayingClipDuration = 0.0f;

    [SerializeField]    private bool                m_PlayNextAudioClipInstantly = false; public bool playNextAudioClipInstantly { get { return m_PlayNextAudioClipInstantly; } }
                        private float               m_TimeSinceLastClipPlayed = 0.0f;


    [Tooltip("Timer Threshold until Instant Clip Playing Enabled")]
    [SerializeField]    private float               m_TimerThresholdTillInstantClipPlaying;

    [SerializeField]    private bool                m_BridgeHasBeenBuilt = false; public bool bridgeHasBeenBuilt { get { return m_BridgeHasBeenBuilt; } set { m_BridgeHasBeenBuilt = value; } }

    // #jj: Persönlich würde ich die public getter mit Großbuchstaben machen :)


    //___SOUND IMPLEMENTATION___
    [SerializeField]    private GameObject          m_AudioSourceGameObject;
    [SerializeField]    private AudioSource         m_AudioSource;

    //Queuing of important milestone audios (bridge) 
                        private bool                m_MilestoneAudioWaitingInQueue = false;

    [SerializeField] [Tooltip("Countdown Time Between Deque and Play of Milestone Audio")]
                        private float               m_CountdownTimeBetweenDequeAndPlay;

                        private float               m_DequeTimer; // #jj de-queue or deque? FYI: deque often means double-ended queue :) No need for a change, just a remark.
                        private bool                m_QueuedMilestoneCanBePlayed                = true;
    [SerializeField]    private List<AudioClip>     m_QueuedMilestoneAudioClips;


    [SerializeField] private bool m_AudioClipsAreAllowedToPlay = true; public bool audioClipsAreAllowedToPlay { get { return m_AudioClipsAreAllowedToPlay; } set { m_AudioClipsAreAllowedToPlay = value; } }

    private void Awake()
    {
        m_AudioSource = m_AudioSourceGameObject.AddComponent<AudioSource>();
        m_QueuedMilestoneAudioClips = new List<AudioClip>();
    }

    // #jj remove empty function
    void Start()
    {
        
    }

    public void StopAudioClip()
    {
        m_AudioSource.Stop();
    }


    public void RegisterAudioBookPool(AudioBookPool audioBookPool)
    {
        m_AudioBookPools.Add(audioBookPool);
    }

    // #jj: usually, register & deregister functions both exist and are needed at some point

    private void UpdateAllAudioPoolsThatTheBridgeHasBeenBuilt()
    {
        // #jj: consider for each
        // #jj: if all audio book pools have the same bool value at all times, why do they even have it and not just ask the audio book manager for its value?
        for(int i = 0; i < m_AudioBookPools.Count; i++)
        {
            m_AudioBookPools[i].bridgeHasBeenBuilt = true;
        }
    }

    // Update is called once per frame // #jj: consider removing comments like this as they just bloat your file :)
    void Update()
    {
        CheckIfBridgeHasBeenBuilt(); // #jj: this would not be needed if you did not need to tell all audio pools.
        CheckIfSomeAudioPoolPlaysCurrently();
        CheckIfNextIncomingAudioClipCanBePlayedInstantly();
        CheckForQueuedMilestoneClip();
    }

    // #jj: consider renaming to UpdateAudoPoolsInfo()
    private void CheckIfBridgeHasBeenBuilt()
    {
        // #jj: this bool is used as both a signal that audio pools need to be made aware as well as the actual value of what they are made aware of.
        // as you never toggle off again, if you wanted to keep this, you could rename to "m_AudioPoolsBridgeInfoDirty" or similar, then perform this action once and then unset the bool.
        if (m_BridgeHasBeenBuilt) 
        {
            UpdateAllAudioPoolsThatTheBridgeHasBeenBuilt();
        }
    }

    // #jj: "UpdateRemainingClipDuration()", "UpdateCurrentClip()"
    private void CheckIfSomeAudioPoolPlaysCurrently() // #jj: consider renaming to UpdateRemainingClipDuration
    {
        // #jj: m_SomeAudioPoolIsCurrentlyPlaying can be removed and replaced by a function IsCurrentlyPlaying() { return m_CurrentlyPlayingClipDuration > 0.0f; }
        // q: why? a: because you now have less chance of having m_SomeAudioPoolIsCurrentlyPlaying and m_CurrentlyPlayingClipDuration become async.
        if (m_SomeAudioPoolIsCurrentlyPlaying) // #jj: consider inverting the if
        {
            m_CurrentlyPlayingClipDuration -= Time.deltaTime; // #jj: remaining duration?
            if (m_CurrentlyPlayingClipDuration <= 0.0f)
            {
                m_SomeAudioPoolIsCurrentlyPlaying = false;
            }
        }
    }

    // #jj: "UpdateReadyForNextClip()", "UpdateNextClip()", or similar
    private void CheckIfNextIncomingAudioClipCanBePlayedInstantly()
    {
        // #jj: general note regarding bools that determine whether some logic needs to happen:
        // prefer calculating the bool every tick or everytime something happens that changes the bool, instead of only doing so when the bool is false
        // reasoning: there will be times where a bool that is true will need to be set to false, e.g. for error handling. You might miss the case with this implementation.
        // #jj: similar to CheckIfSomeAudioPoolPlaysCurrently, you can get rid of m_PlayNextAudioClipInstantly and replace it with CanPlayInstantClip() or similar.
        // Also note, I do not quite understand the concept of m_TimerThresholdTillInstantClipPlaying, at least not in an instant. Might need to put more time into understanding this?

        if (!m_SomeAudioPoolIsCurrentlyPlaying && !m_PlayNextAudioClipInstantly)
        {
            m_TimeSinceLastClipPlayed += Time.deltaTime;
            if(m_TimeSinceLastClipPlayed >= m_TimerThresholdTillInstantClipPlaying) // #jj: If you want to get rid of m_PlayNextAudioClipInstantly, you can make the timer be a cooldown instead of a timer :)
            {
                m_PlayNextAudioClipInstantly    = true;
                m_TimeSinceLastClipPlayed       = 0.0f;
            }
        }


    }

    // #jj: -ing (TryPlayMilestoneClip)
    // #jj: AudioBookManager already contains "audio", so I would consider to remove the word audio from the function call
    public void TryPlayingMilestoneAudioClip(AudioClip clip)
    {
        if (m_SomeAudioPoolIsCurrentlyPlaying | !m_PlayNextAudioClipInstantly | !m_AudioClipsAreAllowedToPlay) // #jj: Warning! Be careful using bit-operators "|" for boolean expressions. Prefer "||", even for bools :)
        {
            m_QueuedMilestoneAudioClips.Add(clip);
            m_MilestoneAudioWaitingInQueue  = true;
            m_DequeTimer                    = m_CountdownTimeBetweenDequeAndPlay;
        }
        else
        {           
            PlayAudioClip(clip);
        }


    }

    //#jj: move this method up to the other "update" methods 
    // UpdateQueuedMilestoneClip(); [notice how you did not call it "AudioClip" here?]
    private void CheckForQueuedMilestoneClip()
    {
        if (m_MilestoneAudioWaitingInQueue) // #jj: Consider inverting if
        {
            //check if previous audio has stopped playing
            // #jj: consider something like 
            // bool tickDequeueTimr = !IsCurrentlyPlayingAudio();
            // if (tickDequeueTimer) ...
            // You can then spare the comments

            if (!m_SomeAudioPoolIsCurrentlyPlaying)
            {
                //count down time from last audio clip ending, to avoid back to back audio clips
                // #jj: I'd spare the what from the comment and just leave the why: "Avoid back to back audio clips"
                m_DequeTimer -= Time.deltaTime;
                if (m_DequeTimer <= 0.0f)
                {
                    m_QueuedMilestoneCanBePlayed = true;
                    // #jj: A good practice is to set the timer to exactly 0 here (same goes for above), because it can be slightly negative now
                    // and maybe you would later on write m_DequeTimer += 5s, instead of = 5s, and have a different result.
                }

            }

            // #jj: Both of your ifs check for !m_SomeAudioPoolIsCurrentlyPlaying, which can not change in between them.
            // I often take this as an indicator that i mixed up some concepts. While it makes snese here, logically, 
            // i think it would be easier to read if you would just move that condition into the condition in line 156.
            // Then, combined with the inverted if, you'd have 
            // if (!m_MilestoneAudioWaitingInQueue || m_SomeAudioPoolIsCurrentlyPlaying) return;
            //
            // m_DequeTimer -= Time.deltaTime;
            // if (m_DequeTimer < 0.0f) m_DequeTimer = 0.0f;
            // if (QueuedMilestoneCanBePlayed())
            //{
            // PlayAudioClip(m_QueuedMilestoneAudioClip);
            //}

            //if noone is playing and enough time has passed, play the queued audio
            if (!m_SomeAudioPoolIsCurrentlyPlaying && m_QueuedMilestoneCanBePlayed && m_AudioClipsAreAllowedToPlay)
            {
                

                PlayAudioClip(m_QueuedMilestoneAudioClips[0]);
                m_QueuedMilestoneAudioClips.RemoveAt(0);
                if(m_QueuedMilestoneAudioClips.Count == 0)
                {
                    m_MilestoneAudioWaitingInQueue  = false;
                    
                }
                
                
            }


        }
        
    }

    public void PlayAudioClip(AudioClip clip)
    {
        Singletons.subtitleManager.StopAllCoroutines();
        Singletons.subtitleManager.StartSubtitles(clip);

        //start new audio clip on audio source
        m_AudioSource.clip = clip;
        m_AudioSource.Play();
        
        //maybe change volume if volume change implementation is there
        //get clips length and start countdown

        // #jj: I would not put that into its own function, because you never want to call it from anywhere but here, and because it does not capsulate that much logic
        // of course, you can decide to keep the method. Here, I find the name too long for it to be worth, though ;)
        UpdateThatAudioIsNowPlayingAndHowLong(clip.length);
        //Singletons.subtitleManager.StartSubtitles(clip);
    }

    private void UpdateThatAudioIsNowPlayingAndHowLong(float duration)
    {
        // #jj: if you use the feedback if gave earlier, regarding making most of your bools functions that return whether the timers are 0 or not, you could remove both calls to the bools here.
        // also, i think this method is a good example of how easy it is for code that "duplicates" variables to grow in a way where variables are becoming async or at least hard to determine whether they remain sync or not :)
        // you can see that because you use m_PlayNextAudioClipInstantly in conjunction with m_TimeSinceLastClipPlayed, but you dont set m_TimeSinceLastClipPlayed here, which you probably would need to if you started to call this
        // method from other places.
        // So its better to just get rid of it if you already store the information in the form of a timer 
        m_SomeAudioPoolIsCurrentlyPlaying = true;
        m_CurrentlyPlayingClipDuration      = duration;
        m_PlayNextAudioClipInstantly        = false;
        
        
    }

}
