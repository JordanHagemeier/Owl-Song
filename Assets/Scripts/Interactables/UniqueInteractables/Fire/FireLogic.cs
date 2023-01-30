using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireState
{
    OFF,
    ON,
    Count
}
public class FireLogic : MonoBehaviour
{
    //audio
    [SerializeField] AudioSource firetrigger;
    [SerializeField] AudioClip [] litclip;



    //fire state
    [SerializeField] private FireState                  m_FireState; 

    //fire is off (Consumer state)
    [SerializeField] private CraftingGoodDictonary      m_FireOFFWantedItems;           public CraftingGoodDictonary fireOffWantedItems { get { return m_FireOFFWantedItems; } set { m_FireOFFWantedItems = value; } }
    [SerializeField] private List<int>                  m_FireOFFAmountOfWantedItems    = new List<int>();
    [SerializeField] private bool                       m_AllGoodsAreGathered = false;

    //fire is on (Exchanger state)
    [SerializeField] private FireRecipes                m_Recipes;
    [SerializeField] private CraftingGoodDictonary      m_FireONWantedItems;            public CraftingGoodDictonary fireONWantedItems { get { return m_FireONWantedItems; } set { m_FireONWantedItems = value; } }
    [SerializeField] private float                      m_InitialBurnTime; 
    [SerializeField] private float                      m_CurrentBurnTime;
    [SerializeField] private float                      m_AddedBurnTimePerWood;

    //UI
    private bool m_AmountOfWantedGoodsHasChanged = false;
    private bool m_AmountOfExchangeGoodsHasChanged = false;

    [SerializeField] GameObject m_UICanvas; public GameObject UICanvas { get { return m_UICanvas; } set { m_UICanvas = value; } }
    [SerializeField] private ConsumerUITextHandler m_UIHandler; public ConsumerUITextHandler UIHandler { get { return m_UIHandler; } set { m_UIHandler = value; } }


    [Header("Fire Asset Switching")]
    [SerializeField] private GameObject m_FireOFFLookGameobject;
    [SerializeField] private GameObject m_FireONLookGameobject;


    //Achievements
    private bool m_FirstFireLit = false;
    private bool m_FirstFireBurnedOut = false;

    //Worldspace UI
    [SerializeField] public GameObject m_IngredientAmountsUIGameObject;
    [SerializeField] private GameObject[] m_InstantiatedIngredientGameObjects;
    [SerializeField] private GameObject[] m_InstantiatedIngredientAmountGameObjects;

    public Texture2D[] m_NumberTextures;
    public GoodSubTypeToMaterialDictionary m_GoodSubTypeToMaterialDictionary;
    [SerializeField] private Texture2D m_NonValidTexture;

    [SerializeField] private Transform m_FireIsOnPosition;
    [SerializeField] private Transform m_FireIsOffPosition;
    [SerializeField] private Transform m_FireTransform;


    //Fire OFF:
    //UPDATE:
    // XX if (all goods are gathered)
    // XX -> m_FireSTate = ON 

    //DROP ITEM:
    // XX -> check if "Own Interactable Type" is corresponding with an entry of FireOFFWatedItems 
    // XX -> if yes: tell the object to despawn, reduce amount at "m_FireOFFAmountOfWantedItems" 

    // XX CHECK IF ALL GOODS ARE GATHERED
    // XX RESET ALL GOODS AND AMOUNTS

    //Fire ON:
    //UPDATE:
    // XX if (m_FireState == ON)
    // XX -> if CurrentBurnTime > 0
    // XX   ->reduce CurrentBurnTime by time.DeltaTime
    // XX -> else
    // XX   -> m_FireState = OFF

    //DROP ITEM:
    //1) check if type is "Crafting Good" or "Food"
    //2) Crafting Good:
    //-> check if "Own Interactable Type" is corresponding with an entry of FireONWatedItems
    //-> if yes: tell the object to despawn, add "AddedBurnTimePerWood" to "CurrentBurnTime"

    //3) Food:
    //-> check for each entry in the recipes if the food item is marked as "consumed food"
    //-> if yes: tell the object to despawn, spawn corresponding "exchanged" item 

    // Start is called before the first frame update
    void Start()
    {
        if(m_FireONLookGameobject == null | m_FireOFFLookGameobject == null)
        {
            Debug.LogError("Gameobjects for switching Firelook are null!");
        }
        ResetObjectAndWantedGoods();
        SetUpIngredientAmountsOnUI();
       
    }


    

    private void DisableAllWSUIElementsForNow()
    {
        int length = 4;
        if (m_IngredientAmountsUIGameObject.gameObject.tag == "EmptyBlueprintUI")
        {
            length = 3;
        }
        m_InstantiatedIngredientGameObjects = new GameObject[length];
        m_InstantiatedIngredientAmountGameObjects = new GameObject[length];

        for (int i = 0; i < length; i++)
        {
            int number = i + 1;
            string nameOfMaterial = "Material_" + number.ToString();
            string amountOfMaterial = "Amount_" + number.ToString();
            m_InstantiatedIngredientGameObjects[i] = m_IngredientAmountsUIGameObject.FindComponentInChildWithName<Transform>(nameOfMaterial).gameObject;
            m_InstantiatedIngredientAmountGameObjects[i] = m_IngredientAmountsUIGameObject.FindComponentInChildWithName<Transform>(amountOfMaterial).gameObject;

            m_InstantiatedIngredientAmountGameObjects[i].SetActive(false);
            m_InstantiatedIngredientGameObjects[i].SetActive(false);
        }
    }

    private void SetUpIngredientAmountsOnUI()
    {
        DisableAllWSUIElementsForNow();



        for (int i = 0; i < m_FireOFFWantedItems.craftingGoodDictionary.Length; i++)
        {


            Texture2D ingredientTexture = m_NonValidTexture;
            for (int k = 0; k < m_GoodSubTypeToMaterialDictionary.subTypeTextureDictionary.Length; k++)
            {
                if (m_GoodSubTypeToMaterialDictionary.subTypeTextureDictionary[k].type == m_FireOFFWantedItems.craftingGoodDictionary[i].type)
                {
                    ingredientTexture = m_GoodSubTypeToMaterialDictionary.subTypeTextureDictionary[k].texture;
                }
            }

            if (m_InstantiatedIngredientGameObjects[i].GetComponent<MeshRenderer>().materials.Length > 1)
            {
                Material[] matArray = m_InstantiatedIngredientGameObjects[i].GetComponent<MeshRenderer>().materials;
                matArray[1].mainTexture = ingredientTexture;
                m_InstantiatedIngredientGameObjects[i].GetComponent<MeshRenderer>().materials = matArray;
            }

            else
            {
                m_InstantiatedIngredientGameObjects[i].GetComponent<MeshRenderer>().material.mainTexture = ingredientTexture;
            }
            m_InstantiatedIngredientGameObjects[i].SetActive(true);
            m_InstantiatedIngredientAmountGameObjects[i].SetActive(true);
        }

        //Transform resultUI = m_IngredientAmountsUIGameObject.FindComponentInChildWithName<Transform>("Result");
        //if (resultUI != null)
        //{
        //    resultUI.gameObject.GetComponent<MeshRenderer>().material.mainTexture = m_ExchangedItemTexture;
        //}


        UpdateIngredientAmountsOnUI();
    }


    private void ChangeAllAmountGOActiveStates(bool state)
    {
        for(int i = 0; i < m_FireOFFWantedItems.craftingGoodDictionary.Length; i++)
        {
            m_InstantiatedIngredientAmountGameObjects[i].SetActive(state);

        }
        UpdateIngredientAmountsOnUI();
    }

    private void UpdateIngredientAmountsOnUI()
    {
        CraftingGoodDictonary currentlyWantedGoods = m_FireOFFWantedItems;
        if(m_FireState == FireState.ON)
        {
            return;
        }



        for (int i = 0; i < currentlyWantedGoods.craftingGoodDictionary.Length; i++)
        {

            if (m_InstantiatedIngredientAmountGameObjects[i].GetComponent<MeshRenderer>().materials.Length > 1)
            {
                Material[] matArray = m_InstantiatedIngredientAmountGameObjects[i].GetComponent<MeshRenderer>().materials;
                matArray[1].mainTexture = m_NumberTextures[m_FireOFFAmountOfWantedItems[i]];
                m_InstantiatedIngredientAmountGameObjects[i].GetComponent<MeshRenderer>().materials = matArray;
            }

            else
            {
                m_InstantiatedIngredientAmountGameObjects[i].GetComponent<MeshRenderer>().material.mainTexture = m_NumberTextures[m_FireOFFAmountOfWantedItems[i]];
            }
        }
    }

    private void LerpFireFromOnToOffPositionOverTime()
    {
        float currentFireStatus =  Mathf.Clamp(m_CurrentBurnTime, 0.0f, m_InitialBurnTime) / m_InitialBurnTime;
        m_FireTransform.transform.position = Vector3.Lerp(m_FireIsOffPosition.transform.position, m_FireIsOnPosition.transform.position, currentFireStatus);
    }


    // Update is called once per frame
    void Update()
    {

        if(m_FireState == FireState.OFF)
        {
            if(m_FireOFFLookGameobject != null )
            {
                if (m_FireONLookGameobject.activeSelf == true | m_FireOFFLookGameobject.activeSelf == false)
                {
                    m_FireOFFLookGameobject.SetActive(true);
                    m_FireONLookGameobject.SetActive(false);
                    ChangeAllAmountGOActiveStates(true);
                }
            }

            if (m_AmountOfWantedGoodsHasChanged)
            {
                m_AmountOfWantedGoodsHasChanged = false;
                UpdateIngredientAmountsOnUI();
                //m_UIHandler.UpdateAmountList(m_FireOFFAmountOfWantedItems);

            }

            m_AllGoodsAreGathered = CheckIfAllGoodsAreGathered();
            if (m_AllGoodsAreGathered)
            {
                int clipkey = Random.Range(0, litclip.Length - 1);

                firetrigger.clip = litclip[clipkey];
                firetrigger.Play();

                if (!m_FirstFireLit)
                {
                    m_FirstFireLit = true;
                    Singletons.achievementManager.AchievementWasAccomplished(AchievementType.FireLit);
                }
                m_AllGoodsAreGathered   = false;
                m_FireState             = FireState.ON;
                m_CurrentBurnTime       = m_InitialBurnTime;
                //m_UIHandler.ClearAllText();
                
            }
        }
        else if(m_FireState == FireState.ON)
        {
            if (m_FireONLookGameobject != null)
            {
                if(m_FireONLookGameobject.activeSelf == false | m_FireOFFLookGameobject.activeSelf == true)
                {
                    m_FireOFFLookGameobject.SetActive(false);
                    m_FireONLookGameobject.SetActive(true);
                    ChangeAllAmountGOActiveStates(false);

                }
            }

            if (m_CurrentBurnTime >= 0)
            {
                m_CurrentBurnTime -= Time.deltaTime;
                LerpFireFromOnToOffPositionOverTime();
                //m_UIHandler.UpdateSpawningProgress((int)m_CurrentBurnTime);
            }
            else
            {

                if (!m_FirstFireBurnedOut)
                {
                    m_FirstFireBurnedOut = true;
                    Singletons.achievementManager.AchievementWasAccomplished(AchievementType.FireStopped);
                }
                m_FireState = FireState.OFF;
                ResetObjectAndWantedGoods();
            }
        }
    }

    private bool DropOffGoodsForIgnition(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {
        bool success = false;
        if(m_FireState == FireState.OFF)
        {
            for (int i = 0; i < m_FireOFFAmountOfWantedItems.Count; i++)
            {
                if (playerInteractionController.currentInteractable.GetComponent<GatheringInteractable>().m_OwnCraftingGoodType == m_FireOFFWantedItems.craftingGoodDictionary[i].type)
                {
                    if (m_FireOFFAmountOfWantedItems[i] > 0)
                    {
                        m_FireOFFAmountOfWantedItems[i]--;
                        playerInteractionController.TellPlayerToDropGood();
                        m_AmountOfWantedGoodsHasChanged = true;
                        success = true;
                        return success;
                    }

                }
            }
        }
        if(m_FireState == FireState.ON)
        {
            for (int i = 0; i < m_FireONWantedItems.craftingGoodDictionary.Length; i++)
            {
                if (playerInteractionController.currentInteractable.GetComponent<GatheringInteractable>().m_OwnCraftingGoodType == m_FireONWantedItems.craftingGoodDictionary[i].type)
                {
                    playerInteractionController.TellPlayerToDropGood();
                    m_CurrentBurnTime += m_AddedBurnTimePerWood;
                    success = true;
                    return success;
                }
            }
        }

        return success;
    }

   

    private bool CheckIfAllGoodsAreGathered()
    {
        for (int i = 0; i < m_FireOFFAmountOfWantedItems.Count; i++)
        {
            if (m_FireOFFAmountOfWantedItems[i] > 0)
            {
                return false;
            }
        }
        return true;
    }

    private void ResetObjectAndWantedGoods()
    {
        if(m_FireOFFAmountOfWantedItems.Count != m_FireOFFWantedItems.craftingGoodDictionary.Length | m_FireOFFAmountOfWantedItems == null)
        {
            m_FireOFFAmountOfWantedItems = new List<int>();
            for(int i = 0; i < m_FireOFFWantedItems.craftingGoodDictionary.Length; i++)
            {
                m_FireOFFAmountOfWantedItems.Add(m_FireOFFWantedItems.craftingGoodDictionary[i].amount);
            }
        }
        for (int i = 0; i < m_FireOFFWantedItems.craftingGoodDictionary.Length; i++)
        {
            m_FireOFFAmountOfWantedItems[i] = m_FireOFFWantedItems.craftingGoodDictionary[i].amount;
        }
        m_AmountOfWantedGoodsHasChanged     = true;
    }

    public bool OnInteract(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {
        bool success = false;
        if (type == InteractionTypes.DropOff)
        {
            
            if (playerInteractionController.currentInteractable.m_InteractableType == InteractableType.CraftingGood)
            {
                success = DropOffGoodsForIgnition(type, playerInteractionController);
            }

            else if (playerInteractionController.currentInteractable.m_InteractableType == InteractableType.Food)
            {
                success = CheckFoodForCorrespondingCookedFood(type, playerInteractionController);
            }
            
            

        }

        return success;
       
    }

    private bool CheckFoodForCorrespondingCookedFood(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {
        bool success = false;
        if (m_FireState == FireState.ON)
        {
            for (int i = 0; i < m_Recipes.fireRecipes.Length; i++)
            {
                if (playerInteractionController.currentInteractable.GetComponent<GatheringInteractable>().m_OwnCraftingGoodType == m_Recipes.fireRecipes[i].consumedFood)
                {
                    playerInteractionController.TellPlayerToDropGood();

                    GameObject copyOfSpawnObject;
                    copyOfSpawnObject = Singletons.interactableManager.SetInactiveInteractableActive(m_Recipes.fireRecipes[i].exchangedFood);
                    playerInteractionController.GiveGoodToPlayer(copyOfSpawnObject);
                    success = true;
                }
            }
        }

        return success;
    }
}
