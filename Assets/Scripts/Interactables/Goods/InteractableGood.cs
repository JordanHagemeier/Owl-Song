using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableGood : Interactable
{

    public float    m_DespawnThreshold;
    public float    m_CurrentDespawnTimer = 0.0f;
    public bool     m_CountsDownToDespawn = false;
    //zwei Assets oder zwei Texturen/Materials, Effekt? 
    //prozent zahl ab wann geswitched wird

    public override void CopyInteractableValues(Interactable toCopyTo)
    {
        base.CopyInteractableValues(toCopyTo);
        if(toCopyTo.GetComponent<InteractableGood>() != null)
        {
            toCopyTo.GetComponent<InteractableGood>().m_DespawnThreshold        = m_DespawnThreshold;
            toCopyTo.GetComponent<InteractableGood>().m_CurrentDespawnTimer     = m_CurrentDespawnTimer;
            toCopyTo.GetComponent<InteractableGood>().m_CountsDownToDespawn     = m_CountsDownToDespawn;
        }
    }


    public override void Awake()
    {
        base.Awake();
        gameObject.layer = LayerMask.NameToLayer("Goods");
        gameObject.tag = "Goods";
        
    }
    public override bool OnInteract(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {

        if (type == InteractionTypes.Count)
        {
            Debug.Log(m_InteractableType.ToString());
        }

        if (type == InteractionTypes.PickUp)
        {
            m_CountsDownToDespawn = false;
            Debug.Log("Insert Pickup Line! ");
        }

        if (type == InteractionTypes.DropOff)
        {
            m_CountsDownToDespawn = true;
            m_CurrentDespawnTimer = 0.0f;
        }

        return true;
    }

    public override void Update()
    {
        base.Update();

        if(m_CountsDownToDespawn == true)
        {
            m_CurrentDespawnTimer += Time.deltaTime;
            if(m_CurrentDespawnTimer >= m_DespawnThreshold)
            {
                m_CountsDownToDespawn = false;
                m_CurrentDespawnTimer = 0.0f;
                gameObject.SetActive(false);
            }
        }
        
    }





}
