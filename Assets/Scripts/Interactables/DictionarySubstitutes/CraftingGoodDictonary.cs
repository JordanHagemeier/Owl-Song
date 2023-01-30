using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Exchanger/Crafting Good Input Dictionary")]
public class CraftingGoodDictonary : ScriptableObject
{
    public void Init(int entryAmount)
    {
        craftingGoodDictionary = new CraftingGoodDictionaryEntry[entryAmount];
    }
    [System.Serializable]
    public class CraftingGoodDictionaryEntry
    {
        public GoodSubTypes type;
        public int amount;
    }
    public CraftingGoodDictionaryEntry[] craftingGoodDictionary;
}
