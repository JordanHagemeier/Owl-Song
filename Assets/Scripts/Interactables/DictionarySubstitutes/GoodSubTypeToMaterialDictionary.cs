using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Crafting Goods/Crafting Sub Type to Material Dictionary")]
public class GoodSubTypeToMaterialDictionary : ScriptableObject
{
    [System.Serializable]
    public class GoodSubTypeToMaterialDictionaryEntry
    {
        public GoodSubTypes type;
        public Texture2D texture;
    }
    public GoodSubTypeToMaterialDictionaryEntry[] subTypeTextureDictionary;
}

