using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singletons : MonoBehaviour
{
    public AchievementManager   m_AchievementManager;           public static AchievementManager achievementManager         { get { return instance.m_AchievementManager; } }
    public AmbientSoundManager  m_AmbientSoundManager;          public static AmbientSoundManager ambientSoundManager       { get { return instance.m_AmbientSoundManager; } }
    public AudioManager         m_AudioManager;                 public static AudioManager audioManager                     { get { return instance.m_AudioManager; } }
    public AudioBookManager     m_AudioBookManager;             public static AudioBookManager audioBookManager             { get { return instance.m_AudioBookManager; } }
    public BackgroundMusicManager m_BackgroundMusicManager;     public static BackgroundMusicManager backgroundMusicManager { get { return instance.m_BackgroundMusicManager; } }
    public BlueprintManager     m_BlueprintManager;             public static BlueprintManager blueprintManager             { get { return instance.m_BlueprintManager; } }
    public CheatManager         m_CheatManager;                 public static CheatManager cheatManager                     { get { return instance.m_CheatManager; } }
    public ConstructionManager  m_ConstructionManager;          public static ConstructionManager constructionManager       { get { return instance.m_ConstructionManager; } }
    public CSVReader            m_CSVReader;                    public static CSVReader csvReader                           { get { return instance.m_CSVReader; } }
    public DaytimeManager       m_DaytimeManager;               public static DaytimeManager daytimeManager                 { get { return instance.m_DaytimeManager; } }
    public DiaryManager         m_DiaryManager;                 public static DiaryManager diaryManager                     { get { return instance.m_DiaryManager; } }
    public DebugDrawingManager  m_DebugDrawingManager;          public static DebugDrawingManager debugDrawingManager       { get { return instance.m_DebugDrawingManager; } }
    public DebugTextHelper      m_DebugTextHelper;              public static DebugTextHelper debugTextHelper               { get { return instance.m_DebugTextHelper; } }
    public EfficiencyManager    m_EfficiencyManager;            public static EfficiencyManager efficiencyManager           { get { return instance.m_EfficiencyManager; } }
    public FoodManager          m_FoodManager;                  public static FoodManager foodManager                       { get { return instance.m_FoodManager; } }
    public GameStateManager     m_GameStateManager;             public static GameStateManager gameStateManager             { get { return instance.m_GameStateManager; } }
    public InteractableManager  m_InteractableManager;          public static InteractableManager interactableManager       { get { return instance.m_InteractableManager; } }
    public LocalisationManager  m_LocalisationManager;          public static LocalisationManager localisationManager       { get { return instance.m_LocalisationManager; } }
    public SubtitleManager      m_SubtitleManager;              public static SubtitleManager subtitleManager               { get { return instance.m_SubtitleManager; } }
    public TickManager          m_TickManager;                  public static TickManager tickManager                       { get { return instance.m_TickManager; } }
    public TimeManager          m_TimeManager;                  public static TimeManager timeManager                       { get { return instance.m_TimeManager; } }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static bool s_IsShuttingDown = false;

    public static bool IsShuttingDown()
    {
        return s_IsShuttingDown;
    }

    private void OnApplicationQuit()
    {
        s_IsShuttingDown = true;
    }

/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    private static Singletons s_Instance;

    

    public static Singletons instance
    {
        
        get
        {
            if (!s_Instance)
            {
                s_Instance = GameObject.FindObjectOfType<Singletons>();
            }
            return s_Instance;
        }
    }



}
