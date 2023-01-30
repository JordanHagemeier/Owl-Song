using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DaytimeManager))]
public class DaytimeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DaytimeManager daytimeManager = (DaytimeManager)target;

        DrawDefaultInspector();


        if (GUILayout.Button("Switch to next Lighting Setting"))
        {
            daytimeManager.TryTransitioningToNextLightSetting();
        }


    }
}
