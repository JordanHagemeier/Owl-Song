using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfficiencyManager : MonoBehaviour
{
    //Game Design
    [SerializeField] private float  m_CurrentEfficiency = 0;
    [SerializeField] private float  m_DefaultEfficieny;
    [SerializeField] private float  m_MinEfficiency;
    [SerializeField] private float  m_MaxEfficiency;
    [SerializeField] private int    m_MaxAmountOfMeals = 1;

    [Tooltip("Calculation: Clamp(Current ef. - (current ef. * decreasePerc.), min ef, max ef)")]
    [SerializeField] private float  m_DecreaseValueInPercentage; 

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentEfficiency             = m_DefaultEfficieny;
        TickManager.endOfDay            += CalculateAtEndOfDay;
        FoodManager.foodWasConsumed     += AddToEfficiency;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddToEfficiency(float multiplicationValue)
    {
        m_CurrentEfficiency = (1 + multiplicationValue) * m_CurrentEfficiency; // "1 + value" in order to make sure it never multiplies by less than 1, effectively decreasing the efficiency
    }

    public float currentEfficiency
    {
        get
        {
            return m_CurrentEfficiency;
        }
    }

    public void SetEfficiencyToDefaultValue()
    {
        m_CurrentEfficiency = m_DefaultEfficieny;
    }


    private void CalculateAtEndOfDay()
    {

        m_CurrentEfficiency = Mathf.Clamp(m_CurrentEfficiency - (m_CurrentEfficiency * m_DecreaseValueInPercentage), m_MinEfficiency, m_MaxEfficiency);
       
    }

    private void OnDestroy()
    {
        TickManager.endOfDay            -= CalculateAtEndOfDay;
        FoodManager.foodWasConsumed     -= AddToEfficiency;
    }
}
