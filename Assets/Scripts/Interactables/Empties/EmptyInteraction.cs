using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyInteraction : InteractableStructure
{

    public override bool OnInteract(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {
        bool success = false;
        if(type == InteractionTypes.PickUp)
        {
            Singletons.constructionManager.OpenConstructionOnSelectedEmpty(gameObject);

        }
        return success;
    }

    public override void Update()
    {
        base.Update();
    }
}
