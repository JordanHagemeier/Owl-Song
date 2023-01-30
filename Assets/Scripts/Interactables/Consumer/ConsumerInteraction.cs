using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumerInteraction : InteractableStructure
{

    public override bool OnInteract(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {
        bool success = false;
        if (type == InteractionTypes.Count)
        {
            Debug.Log(m_InteractableType.ToString());
            return success;
        }


        ConsumerLogic ownLogic = gameObject.GetComponent<ConsumerLogic>();
        if (ownLogic == null)
        {
            gameObject.AddComponent<ConsumerLogic>();
            ownLogic = gameObject.GetComponent<ConsumerLogic>();
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
        m_InteractableType = InteractableType.Spawner;
        gameObject.layer = LayerMask.NameToLayer("Structure");
        gameObject.tag = "Consumer";
    }

    public override void Update()
    {
        base.Update();
    }
}
