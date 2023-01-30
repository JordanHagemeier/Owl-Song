using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    [SerializeField]    private string                          m_CampAudioSoundName;
    [SerializeField]    private Camera                          m_Camera;                           public Camera camera { get { return m_Camera; } set { m_Camera = value; } }
                        public PlayerMovementController         m_PlayerMovementController;
                        public PlayerInteractionController      m_PlayerInteractionController;
                        public bool                             m_InMenu                        = false;

    //Game State Events
    [SerializeField]    private bool                            m_BridgeHasBeenBuilt            = false;
    [SerializeField]    private bool                            m_GasstationIsFinished          = false;
                        private int                             m_FinishedGasstationEvents      = 0;

    [SerializeField]    private GameObject                      m_BlackFadingPanel;
                        private float                           m_IntroLength;
                        private float                           m_CurrentIntroPlayingTime       = 0.0f;
    [SerializeField]    private float                           m_IntroIsBlackPercentage;
                        private bool                            m_IntroPlaying                  = false;

    [SerializeField]    private float                           m_OutroIsBlackPercentage;
    [SerializeField]    private float                           m_OutroIsTransparentPercentage;
                        private float                           m_OutroLength;
                        private float                           m_CurrentOutroPlayingTime       = 0.0f;
                        private bool                            m_OutroPlaying                  = false;

    [SerializeField]    private Transform                       m_PlayerFinalPosition;


    public void ChangePlayerMovementState(bool canMove)
    {
        m_PlayerMovementController.CanMove = canMove;
    }

    public void BridgeHasBeenBuilt()
    {
        m_BridgeHasBeenBuilt = true;
        Singletons.audioBookManager.bridgeHasBeenBuilt = true;
        Singletons.achievementManager.AchievementWasAccomplished(AchievementType.BridgeBuilt);
    }

    public void GasstationHasProgressed()
    {
        Debug.Log(m_FinishedGasstationEvents);

        m_FinishedGasstationEvents++;
        if(m_FinishedGasstationEvents == 1)
        {
            Singletons.achievementManager.AchievementWasAccomplished(AchievementType.Gasstation1);
        }

        if(m_FinishedGasstationEvents == 2)
        {
            Singletons.achievementManager.AchievementWasAccomplished(AchievementType.Gasstation2);
        }

        if(m_FinishedGasstationEvents == 3)
        {
            m_GasstationIsFinished = true;


            return;
        }

    }

    private void GameHasBeenFinished()
    {
        Singletons.achievementManager.AchievementWasAccomplished(AchievementType.Epilog);
        m_OutroLength       = Singletons.achievementManager.GetAchievementSoundLength(AchievementType.Epilog);
        m_OutroPlaying      = true;
        m_IntroPlaying      = false;

        ChangePlayerMovementState(false);
        m_PlayerMovementController.MovePlayerToSpecificPosition(m_PlayerFinalPosition.position);

        Singletons.timeManager.m_DayProgressed = false;
        m_PlayerMovementController.gameObject.GetComponent<PlayerSoundController>().m_PlayerSoundsAllowed = false;
        Singletons.backgroundMusicManager.SetNewVolume(0.0f);
        Singletons.ambientSoundManager.StopAllAmbientSounds();
    }

    private void GameHasStarted()
    {
        Singletons.achievementManager.AchievementWasAccomplished(AchievementType.Prolog);
        m_IntroLength           = Singletons.achievementManager.GetAchievementSoundLength(AchievementType.Prolog);
        m_IntroPlaying          = true;
        ChangePlayerMovementState(false);
        Singletons.ambientSoundManager.StopAllAmbientSounds();

    }


    // Start is called before the first frame update
    void Start()
    {
        GameHasStarted();

    }

    // Update is called once per frame
    void Update()
    {
        CheckForInput();
        CheckForEventProgress();
        CheckForEvents();

    }

    private void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKey(KeyCode.F4) && !m_InMenu)
        {
            Application.Quit();
        }
    }

    private void CheckForEvents()
    {
        if (m_GasstationIsFinished)
        {
            GameHasBeenFinished();
        }
    }

    private void CheckForEventProgress()
    {
        if (m_IntroPlaying)
        {
            if(m_CurrentIntroPlayingTime >= m_IntroLength)
            {
                m_IntroPlaying = false;
                ChangePlayerMovementState(true);
                m_PlayerInteractionController.UnfreezePlayer();
                m_BlackFadingPanel.GetComponent<Image>().color = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                Singletons.backgroundMusicManager.TryStartingTransition(Daytime.Night);
                Singletons.timeManager.m_DayProgressed = true;
                m_PlayerMovementController.gameObject.GetComponent<PlayerSoundController>().m_PlayerSoundsAllowed = true;
                Singletons.ambientSoundManager.StartAllAmbientSounds();
            }
            m_CurrentIntroPlayingTime += Time.deltaTime;
            if(m_CurrentIntroPlayingTime/m_IntroLength >= m_IntroIsBlackPercentage)
            {
                float BlackFadingAmount = MathHelper.Remap(m_CurrentIntroPlayingTime / m_IntroLength, m_IntroIsBlackPercentage, 1.0f, 0.0f, 1.0f);
                m_BlackFadingPanel.GetComponent<Image>().color = Color.Lerp(Color.black, new Vector4(0.0f, 0.0f, 0.0f, 0.0f), BlackFadingAmount);

            }
        }

        if (m_OutroPlaying)
        {
            if (m_CurrentOutroPlayingTime >= m_OutroLength)
            {
                m_OutroPlaying = false;
                m_BlackFadingPanel.GetComponent<Image>().color = Color.black;
                Singletons.audioBookManager.StopAudioClip();


            }
            m_CurrentOutroPlayingTime += Time.deltaTime;
            if (m_CurrentOutroPlayingTime / m_OutroLength >= m_OutroIsTransparentPercentage)
            {
                float BlackFadingAmount = MathHelper.Remap(m_CurrentOutroPlayingTime / m_OutroLength, m_OutroIsTransparentPercentage, m_OutroIsBlackPercentage, 0.0f, 1.0f);
                m_BlackFadingPanel.GetComponent<Image>().color = Color.Lerp(new Vector4(0.0f, 0.0f, 0.0f, 0.0f), Color.black, BlackFadingAmount);

            }
        }
    }
    private void OnDestroy()
    {

    }
}
