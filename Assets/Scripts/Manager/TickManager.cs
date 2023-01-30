using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TickManager : MonoBehaviour
{
    private float m_TimeSinceLastTick = 0;
    private int m_CurrentTick = 0;

    //Game Design Variables
    [SerializeField] private float m_TickLengthFactor;
    [SerializeField] private int m_AmountOfTicksPerDay;
    //[SerializeField] private float m_MorningLengthPercentage; //same length as afternoon
    //[SerializeField] private float m_MiddayLengthPercentage; //should be longer than morning and afternoon
    //[SerializeField] private float m_AfternoonLengthPercentage;
    //private int m_MorningTickLimit;
    //private int m_MiddayTickLimit;
    //private int m_AfternoonTickLimit;

    [Header("Setting of specific tick amount")]
    [Tooltip("This is used for how many ticks you want to start before the end of day event.")]
    [SerializeField] private int m_AmountOfTicksBeforeEndOfDay;

    //Events
    // 1. Tick event for everything that works with ticks
    // 2. end of day event for everything that needs it

    //Delegates
    

    public delegate void EndOfDay();
    public static EndOfDay endOfDay;

    public delegate void MorningEvent();
    public static MorningEvent morningEvent;

    public delegate void MiddayEvent();
    public static MiddayEvent middayEvent;

    public delegate void AfternoonEvent();
    public static AfternoonEvent afternoonEvent;

    // Start is called before the first frame update
    void Start()
    {
        //CalculateDayTimesLimits();
    }

    // Update is called once per frame
    void Update()
    {
        //currently not in use
        UpdateCurrentTick();
    }

    //this function is used to get the current ingame time for tasks like despawning, growth, crafting etc 
    public float GetElapsedIngameTime()
    {
        return Time.deltaTime * m_TickLengthFactor;
    }

    private void UpdateCurrentTick()
    {
        m_TimeSinceLastTick += Time.deltaTime;
        if(m_TimeSinceLastTick > m_TickLengthFactor)
        {
            m_TimeSinceLastTick = m_TimeSinceLastTick - m_TickLengthFactor;
            m_CurrentTick++;
            
            if (m_CurrentTick >= m_AmountOfTicksPerDay)
            {
                endOfDay();
                m_CurrentTick = 0;
            }
        }
    }

    public void SetCurrentTickToZero()
    {
        m_CurrentTick = 0;
        m_TimeSinceLastTick = 0;
    }

    public void SetCurrentTickToEndOfDay()
    {
        m_CurrentTick = m_AmountOfTicksPerDay - m_AmountOfTicksBeforeEndOfDay;
        m_TimeSinceLastTick = 0;
    }




    
    //private void CalculateDayTimesLimits()
    //{
    //    m_MorningTickLimit = Mathf.RoundToInt(m_AmountOfTicksPerDay * m_MorningLengthPercentage);
    //    m_MiddayTickLimit = Mathf.RoundToInt(m_AmountOfTicksPerDay * m_MiddayLengthPercentage);
    //    m_AfternoonTickLimit = Mathf.RoundToInt(m_AmountOfTicksPerDay * m_AfternoonLengthPercentage);
    //}


}
