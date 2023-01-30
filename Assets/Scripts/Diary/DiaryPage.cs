using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "Diary/Diary Page Entry")]

public class DiaryPage : ScriptableObject
{
    
    [SerializeField] public Texture2D                   m_StructureImage;
    [SerializeField] public Texture2D                   m_FinishedStructureImage;
    [SerializeField] public BluePrintToStructureMap     m_BlueprintWithStructure;
}
