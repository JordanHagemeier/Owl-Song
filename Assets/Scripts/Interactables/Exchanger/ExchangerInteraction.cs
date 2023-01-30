using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangerInteraction : InteractableStructure
{

    public override bool OnInteract(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {
        bool success = false;
        if (type == InteractionTypes.Count)
        {
            Debug.Log(m_InteractableType.ToString());
            return success;
        }


        ExchangerLogic ownLogic = gameObject.GetComponent<ExchangerLogic>();
        if (ownLogic == null)
        {
            gameObject.AddComponent<ExchangerLogic>();
            ownLogic = gameObject.GetComponent<ExchangerLogic>();
        }

        if(ownLogic.OnInteract(type, playerInteractionController))
        {
            success = true;
        }
        return success;

    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Exchanger";
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
