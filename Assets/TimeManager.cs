using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    [SerializeField] private float m_SingleDayDuration;
    private float m_CurrentDayTime = 0.0f;
    [SerializeField] private float m_DayTimeNoChangeDuration;
    private float m_QuarterOfADay;
    private bool m_FirstEvening = false;
    private bool m_FirstMorning = false;
    private bool m_SecondEvening = false;

    public bool m_DayProgressed = false;

    // Start is called before the first frame update
    void Start()
    {
        m_QuarterOfADay = (m_SingleDayDuration - m_DayTimeNoChangeDuration) / 4.0f;
        Singletons.daytimeManager.OverallTransitionDuration = m_QuarterOfADay;
        Singletons.daytimeManager.TryTransitioningToNextLightSetting();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DayProgressed)
        {
            CheckForDaytimeChanges();

        }
    }

    private void CheckForDaytimeChanges()
    {
        m_CurrentDayTime += Time.deltaTime;
        if(m_CurrentDayTime >= m_QuarterOfADay && (m_CurrentDayTime - m_QuarterOfADay) >= m_DayTimeNoChangeDuration)
        {
            Singletons.daytimeManager.TryTransitioningToNextLightSetting();
            m_CurrentDayTime = 0.0f;
        }
    }
}
