using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CheatManager))]
public class CheatManagerEditor : Editor
{

  

    public override void OnInspectorGUI()
    {
        CheatManager cheatManager = (CheatManager)target;

        DrawDefaultInspector();
        //GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Yolololo"))
        {
            cheatManager.IAmAlive();
        }
        //if(GUILayout.Button("Change Hidden Wall"))
        //{
        //    cheatManager.ChangeWallState();
        //}
      
        

        CheckDebugTextToggle(cheatManager);
    }


    private void CheckDebugTextToggle(CheatManager cheatManager)
    {
       
        bool currentState = cheatManager.CheckForActiveDebugText();
        bool newDebugTextState = EditorGUILayout.Toggle("DebugText", currentState);

        if (newDebugTextState && !cheatManager.CheckForActiveDebugText())
        {
            cheatManager.ChangeDebugTextState(true);

        }
        if(!newDebugTextState && cheatManager.CheckForActiveDebugText())
        {
            cheatManager.ChangeDebugTextState(false);
        }

        
    }
}
