using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Fire/Fire Recipes")]
public class FireRecipes : ScriptableObject
{
    public void Init(int entryAmount)
    {
        fireRecipes = new FireRecipesEntry[entryAmount];
    }
    [System.Serializable]
    public class FireRecipesEntry
    {
        public GoodSubTypes consumedFood;
        public GameObject   exchangedFood;
    }
    public FireRecipesEntry[] fireRecipes;
}
