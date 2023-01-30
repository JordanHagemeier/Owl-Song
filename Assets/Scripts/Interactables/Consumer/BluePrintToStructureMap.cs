using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "BluePrints/Blue Print Dictionary")]

public class BluePrintToStructureMap : ScriptableObject
{
    [System.Serializable]
    public class BlueprintRecipe
    {
        public GoodSubTypes type;
        public int amount;
    }

    [System.Serializable]
    public class BlueprintToStructure
    {
        public GameObject structure;
        public BlueprintTypes blueprint;
        public BlueprintRecipe[] blueprintRecipes;
    }

    public BlueprintToStructure m_blueprintToStructure;
}
