using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(InteractableManager))]
public class InteractableManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InteractableManager interactableManager = (InteractableManager)target;

        DrawDefaultInspector();
        
       
        if (GUILayout.Button("Highlight all Interactables"))
        {
            interactableManager.HighLightAllInteractablesForXSeconds(0.4f);
        }

        if (GUILayout.Button("Highlight all Spawners"))
        {
            interactableManager.HighLightAllOfTypeForXSeconds(0.4f, "Spawner");
        }
        if (GUILayout.Button("Highlight all Exchangers"))
        {
            interactableManager.HighLightAllOfTypeForXSeconds(0.4f, "Exchanger");
        }
        if (GUILayout.Button("Highlight all Consumers"))
        {
            interactableManager.HighLightAllOfTypeForXSeconds(0.4f, "Consumer");
        }
        if (GUILayout.Button("Draw all No Drop Zones"))
        {
            interactableManager.DrawAllNoDropZones();
        }


    }
}
