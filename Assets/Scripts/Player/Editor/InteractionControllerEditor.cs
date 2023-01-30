using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(InteractionControllerEditor))]
public class InteractionControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InteractableManager interactableManager = (InteractableManager)target;

        DrawDefaultInspector();


    }

    

}
