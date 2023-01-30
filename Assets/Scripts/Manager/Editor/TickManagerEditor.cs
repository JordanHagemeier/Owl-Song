using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TickManager))]
public class TickManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TickManager tickManager = (TickManager)target;

        DrawDefaultInspector();

        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Set current tick to zero"))
        {
            tickManager.SetCurrentTickToZero();
        }
        if (GUILayout.Button("Set current tick to end of day"))
        {
            tickManager.SetCurrentTickToEndOfDay();
        }


    }
}
