using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(EfficiencyManager))]
public class EfficiencyManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EfficiencyManager efficiencyManager = (EfficiencyManager)target;

        DrawDefaultInspector();

        
        if (GUILayout.Button("Set efficiency to default value"))
        {
            efficiencyManager.SetEfficiencyToDefaultValue();
        }
       

    }
}
