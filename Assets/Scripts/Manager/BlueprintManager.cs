using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlueprintManager : MonoBehaviour
{
    //[SerializeField] private Mesh       m_BlueprintSymbolMesh;
    //[SerializeField] private Material   m_BlueprintSymbolMaterial;
    [SerializeField] private GameObject m_BlueprintCone;
    [SerializeField] private float      m_BlueprintSphereColliderRadius;

    //UI Element for the blueprint when its in construction
    [SerializeField] private GameObject m_BlueprintConstructionCanvas;
    public Texture2D[] m_NumberTextures;

    //Achievements
    private bool m_FirstBuildingFinished = false;
    [SerializeField] private GoodSubTypeToMaterialDictionary m_GoodSubTypeToMaterialDictionary;

    public void ConsumerIsDone(ConsumerLogic consumer)
    {
        if (consumer.ownBluePrintType == BlueprintTypes.Justus ||
            consumer.ownBluePrintType == BlueprintTypes.Peter ||
            consumer.ownBluePrintType == BlueprintTypes.Bob)
        {
            Singletons.gameStateManager.GasstationHasProgressed();
        }


        else if(consumer.ownBluePrintType == BlueprintTypes.Bridge)
        {
            Singletons.gameStateManager.BridgeHasBeenBuilt();

        }


        UpgradeBlueprint(consumer);


    }

    private CraftingGoodDictonary ConvertBlueprintRecipesToCraftinGoodNeeds(BluePrintToStructureMap bluePrintToStructureMap)
    {
        CraftingGoodDictonary destinationCraftingDictionary = ScriptableObject.CreateInstance<CraftingGoodDictonary>();

        destinationCraftingDictionary.Init(bluePrintToStructureMap.m_blueprintToStructure.blueprintRecipes.Length);
        for (int i = 0; i < bluePrintToStructureMap.m_blueprintToStructure.blueprintRecipes.Length; i++)
        {
            destinationCraftingDictionary.craftingGoodDictionary[i] = new CraftingGoodDictonary.CraftingGoodDictionaryEntry();
            destinationCraftingDictionary.craftingGoodDictionary[i].type = bluePrintToStructureMap.m_blueprintToStructure.blueprintRecipes[i].type;
            destinationCraftingDictionary.craftingGoodDictionary[i].amount = bluePrintToStructureMap.m_blueprintToStructure.blueprintRecipes[i].amount;
        }
        return destinationCraftingDictionary;
    }


    public void ChangeEmptyToBlueprint(DiaryPage currentStructureOnPage, EmptyInteraction m_CallingEmpty)
    {
        GameObject currentEmpty = m_CallingEmpty.gameObject;

        Singletons.interactableManager.RemoveInteractable(m_CallingEmpty);
        m_CallingEmpty.enabled = false;

        if (currentEmpty.GetComponent<Collider>() == null)
        {
            currentEmpty.AddComponent<SphereCollider>();
            currentEmpty.GetComponent<SphereCollider>().radius = m_BlueprintSphereColliderRadius;
        }
        currentEmpty.AddComponent<ConsumerInteraction>();
        currentEmpty.AddComponent<ConsumerLogic>();
        if(currentEmpty.GetComponent<NavMeshObstacle>() == null)
        {
            currentEmpty.AddComponent<NavMeshObstacle>();

        }

        ConsumerLogic currentLogic              = currentEmpty.GetComponent<ConsumerLogic>();
        currentLogic.ownBluePrintType           = currentStructureOnPage.m_BlueprintWithStructure.m_blueprintToStructure.blueprint;
        currentLogic.wantedCraftingGoods        = ConvertBlueprintRecipesToCraftinGoodNeeds(currentStructureOnPage.m_BlueprintWithStructure);
        currentLogic.desiredStructure           = currentStructureOnPage.m_BlueprintWithStructure.m_blueprintToStructure.structure;


        currentLogic.m_IngredientAmountsUIGameObject    = currentEmpty.gameObject.FindComponentInChildWithTag<Transform>("EmptyBlueprintUI").gameObject;
        currentLogic.m_IngredientAmountsUIGameObject.FindComponentInChildWithName<Transform>("Spot Light").gameObject.SetActive(true);
        currentLogic.m_NumberTextures                   = m_NumberTextures;
        currentLogic.m_GoodSubTypeToMaterialDictionary  = m_GoodSubTypeToMaterialDictionary;

        //currentLogic.UICanvas                   = m_BlueprintConstructionCanvas;
        //currentLogic.UIHandler                  = currentEmpty.AddComponent<ConsumerUITextHandler>();

        //set the "construction sight cone" mesh
        //currentEmpty.GetComponent<MeshFilter>().mesh = m_BlueprintSymbolMesh;
        //currentEmpty.GetComponent<ConsumerInteraction>().standardMaterial = m_BlueprintSymbolMaterial;
        Instantiate(m_BlueprintCone, currentEmpty.transform);


    }

    private void UpgradeBlueprint(ConsumerLogic consumer)
    {
        //1. get corresponding prefab of final structure for type
        //2. get position of consumer for swapping assets
        //3. set blueprint consumer inactive
        //4. set new structure active -> Object pooling? Or instantiating?

        if(m_FirstBuildingFinished == false)
        {
            if (consumer.ownBluePrintType == BlueprintTypes.Bob || consumer.ownBluePrintType == BlueprintTypes.Justus || consumer.ownBluePrintType == BlueprintTypes.Peter)
            { }

            else
            {
                m_FirstBuildingFinished = true;
                Singletons.achievementManager.AchievementWasAccomplished(AchievementType.FirstBuildingFinished);
            }
        }

         consumer.GetComponent<ConsumerInteraction>().enabled   = false;

         GameObject newStructure                                = Instantiate(consumer.desiredStructure, consumer.gameObject.transform.parent);
         newStructure.transform.position                        = consumer.transform.localPosition;
         newStructure.transform.rotation                        = consumer.transform.localRotation;
         consumer.transform.position                            = new Vector3(0.0f, 0.0f, 0.0f);

         consumer.gameObject.SetActive(false);



    }
}
