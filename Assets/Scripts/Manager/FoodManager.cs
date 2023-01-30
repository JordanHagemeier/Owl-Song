using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{

    //1) need a list of all food objects 
    //2) need to know how much food was consumated today
    //3) reset method for when the day ends

    [SerializeField] private int m_AmountOfConsumedFoodToday = 0;
    [SerializeField] private int m_LimitForDailyFoodConsumption;

    public delegate void FoodWasConsumed(float foodValue);
    public static FoodWasConsumed foodWasConsumed;


    // Start is called before the first frame update
    void Start()
    {
        TickManager.endOfDay += ResetAmountOfConsumatedFood;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //player or food needs to check against this in order to complete the food consumption 

    public bool CheckIfFoodConsumptionPossible(GatheringInteractable foodObject)
    {
        if(m_AmountOfConsumedFoodToday < m_LimitForDailyFoodConsumption)
        {
            m_AmountOfConsumedFoodToday++;
            foodWasConsumed(foodObject.foodValue);
            return true;
        }
        return false;
    }


    private void ResetAmountOfConsumatedFood()
    {
        m_AmountOfConsumedFoodToday = 0;
    }

    private void OnDestroy()
    {
        TickManager.endOfDay -= ResetAmountOfConsumatedFood;
    }

}
