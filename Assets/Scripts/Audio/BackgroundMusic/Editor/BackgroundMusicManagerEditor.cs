using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(BackgroundMusicManager))]
public class BackgroundMusicManagerEditor : Editor
{
    float volume = 1.0f;
    public override void OnInspectorGUI()
    {
        BackgroundMusicManager backgroundMusicManager = (BackgroundMusicManager)target; 
        volume = backgroundMusicManager.GetCurrentMusicVolume();
        DrawDefaultInspector();

        

        //GUI.enabled = Application.isPlaying;
        EditorGUILayout.PrefixLabel("Volume");
        volume = EditorGUILayout.Slider(volume, 0.0f, 1.0f);
        backgroundMusicManager.SetNewVolume(volume);
        

    }

}
