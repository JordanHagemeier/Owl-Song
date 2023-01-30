using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerInteraction : InteractableStructure
{

    public override bool OnInteract(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {
        bool success = false;
        if (type == InteractionTypes.Count)
        {
            Debug.Log(m_InteractableType.ToString());
            return success;
        }

        
        SpawnerLogic ownLogic = gameObject.GetComponent<SpawnerLogic>();
        if (ownLogic == null)
        {
            gameObject.AddComponent<SpawnerLogic>();
            ownLogic = gameObject.GetComponent<SpawnerLogic>();
        }

        if (ownLogic.OnInteract(type, playerInteractionController))
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
        gameObject.tag = "Spawner";
    }

    public override void Update()
    {
        base.Update();
    }

}
