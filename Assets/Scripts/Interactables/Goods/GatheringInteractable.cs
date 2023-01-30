using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringInteractable : InteractableGood
{

    public GoodSubTypes m_OwnCraftingGoodType; 
    [SerializeField] private float m_FoodEfficiencyValue; public float foodValue { get { return m_FoodEfficiencyValue; } }



    public override void CopyInteractableValues(Interactable toCopyTo)
    {
        base.CopyInteractableValues(toCopyTo);
        toCopyTo.GetComponent<GatheringInteractable>().m_OwnCraftingGoodType = m_OwnCraftingGoodType;
        toCopyTo.GetComponent<GatheringInteractable>().m_FoodEfficiencyValue = m_FoodEfficiencyValue;
    }



    public override void Awake()
    {
        base.Awake();
        
    }

    public override void Update()
    {
        base.Update();
    }



private void OnApplicationQuit()
    {
        Singletons.interactableManager.RemoveInteractable(this);
    }
}
