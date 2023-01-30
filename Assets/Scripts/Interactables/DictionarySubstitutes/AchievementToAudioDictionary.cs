using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AchievementType
{
    Prolog,
    Gasstation1,
    Gasstation2,
    Epilog,
    FirstNight,
    FirstMorning,
    PassingBerries,
    SecondNight,
    FirstDiaryInteraction,
    FirstBuildingPlanned,
    FirstBuildingFinished,
    BridgeBuilt,
    FireLit,
    FireStopped,
    ThirdNight,
    FourthNight,
    Count
}


[CreateAssetMenu(fileName = "Data", menuName = "Audio / Achievement Audio Clip Dictionary")]
public class AchievementToAudioDictionary : ScriptableObject
{
    public void Init(int entryAmount)
    {
        achievementAudioDictionary = new AchievementAudioDictionaryEntry[entryAmount];
    }
    [System.Serializable]
    public class AchievementAudioDictionaryEntry
    {
        public AudioClip clip;
        public AchievementType achievementType;
    }
    public AchievementAudioDictionaryEntry[] achievementAudioDictionary;
}
