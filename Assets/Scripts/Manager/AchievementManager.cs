using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
  
    [SerializeField] private AchievementToAudioDictionary m_AchievementAudioDictionary;

    public void AchievementWasAccomplished(AchievementType achievementType)
    {
        for(int i = 0; i < m_AchievementAudioDictionary.achievementAudioDictionary.Length; i++)
        {
            if(achievementType == m_AchievementAudioDictionary.achievementAudioDictionary[i].achievementType)
            {
                Singletons.audioBookManager.TryPlayingMilestoneAudioClip(m_AchievementAudioDictionary.achievementAudioDictionary[i].clip);
                return;
            }
        }
        Debug.Log("Achievement Audio Clip for this Achievement was not found.");
    }

    public float GetAchievementSoundLength(AchievementType achievementType)
    {
        float length = 0.0f;
        for (int i = 0; i < m_AchievementAudioDictionary.achievementAudioDictionary.Length; i++)
        {
            if (achievementType == m_AchievementAudioDictionary.achievementAudioDictionary[i].achievementType)
            {
                return m_AchievementAudioDictionary.achievementAudioDictionary[i].clip.length;
            }
        }
        Debug.Log("Achievement Audio Clip for this Achievement was not found.");
        return length;
    }
    
}
