using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExchangerLogic : MonoBehaviour
{
    [SerializeField] private GameObject                         m_ExchangedItem;
    [SerializeField] private Texture2D                          m_ExchangedItemTexture;

    //does the exchanger need time for spawning?
    [SerializeField] private bool                               m_SpawningTakesTime     = false;
    [SerializeField] private float                              m_SpawnTimePerExchange  = 0.0f;
    [SerializeField] private float                              m_CurrentTime           = 0.0f;
    [SerializeField] private int                                m_CurrentlyHeldAmount;
    [SerializeField] private int                                m_MaxHoldAmount;
    private bool m_InProcessingStage = false;
    private bool m_ExchangedItemsAvailable = false;

    //hold amount for exchanger? 
    

    //Wanted crafting goods
    [SerializeField] private CraftingGoodDictonary              m_WantedCraftingGoods;
    [SerializeField] private List<int>                          m_WantedAmountPerCraftingGood        = new List<int>();

    //Consumer UI
    private bool m_AmountOfWantedGoodsHasChanged    = false;
    private bool m_AmountOfExchangeGoodsHasChanged  = false;

    //[SerializeField] GameObject m_UICanvas;                     public GameObject UICanvas              { get { return m_UICanvas; } set { m_UICanvas = value; } }
    //[SerializeField] private ConsumerUITextHandler m_UIHandler; public ConsumerUITextHandler UIHandler  { get { return m_UIHandler; } set { m_UIHandler = value; } }

    [SerializeField] private GameObject m_EmptyExchangerGameObject;
    [SerializeField] private GameObject m_ProcessingExchangerGameObject;
    [SerializeField] private GameObject m_FullExchangerGameObject;


    //Worldspace UI
    [SerializeField] public GameObject m_IngredientAmountsUIGameObject;
    [SerializeField] private GameObject[] m_InstantiatedIngredientGameObjects;
    [SerializeField] private GameObject[] m_InstantiatedIngredientAmountGameObjects;

    public Texture2D[] m_NumberTextures;
    public GoodSubTypeToMaterialDictionary m_GoodSubTypeToMaterialDictionary;
    [SerializeField] private Texture2D m_NonValidTexture;
    private bool m_ResultAmountUIActive = false;

    // Start is called before the first frame update
    void Start()
    {
       
        if(m_ExchangedItem == null)
        {
            Debug.LogWarning("Exchange Item for " + gameObject.name + " is empty!");
        }
        for(int i = 0; i < m_WantedCraftingGoods.craftingGoodDictionary.Length; i++)
        {
            int amountInTheBeginning = m_WantedCraftingGoods.craftingGoodDictionary[i].amount;
            m_WantedAmountPerCraftingGood.Add(amountInTheBeginning);
        }
        //SetUpUIHandler();
        SetUpIngredientAmountsOnUI();

        Transform LightGO = m_IngredientAmountsUIGameObject.FindComponentInChildWithName<Transform>("Spot Light");
        Singletons.daytimeManager.AddLightToAffectedLights(LightGO.GetComponent<Light>());
    }

    //private void SetUpUIHandler()
    //{
    //    if (m_UIHandler == null)
    //    {
    //        m_UIHandler = gameObject.AddComponent<ConsumerUITextHandler>();
    //    }
    //        m_UIHandler.blueprintConstructionCanvas     = m_UICanvas;
    //        m_UIHandler.wantedCraftingGoods             = m_WantedCraftingGoods;
    //        m_UIHandler.wantedAmountPerCraftingGood     = m_WantedAmountPerCraftingGood;
    //        m_UIHandler.givenGood                       = m_ExchangedItem.GetComponent<GatheringInteractable>();
    //        m_UIHandler.UpdateHoldAmount(m_CurrentlyHeldAmount);
    //        m_UIHandler.SetUpBlueprintCanvasWithText(gameObject.transform);


    //}
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



        for (int i = 0; i < m_WantedCraftingGoods.craftingGoodDictionary.Length; i++)
        {


            Texture2D ingredientTexture = m_NonValidTexture;
            for (int k = 0; k < m_GoodSubTypeToMaterialDictionary.subTypeTextureDictionary.Length; k++)
            {
                if (m_GoodSubTypeToMaterialDictionary.subTypeTextureDictionary[k].type == m_WantedCraftingGoods.craftingGoodDictionary[i].type)
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

        Transform resultUI = m_IngredientAmountsUIGameObject.FindComponentInChildWithName<Transform>("Result");
        if (resultUI != null)
        {
            resultUI.gameObject.GetComponent<MeshRenderer>().material.mainTexture = m_ExchangedItemTexture;
        }


        UpdateIngredientAmountsOnUI();
    }


    private void UpdateIngredientAmountsOnUI()
    {
        for (int i = 0; i < m_WantedCraftingGoods.craftingGoodDictionary.Length; i++)
        {



            if (m_InstantiatedIngredientAmountGameObjects[i].GetComponent<MeshRenderer>().materials.Length > 1)
            {
                Material[] matArray = m_InstantiatedIngredientAmountGameObjects[i].GetComponent<MeshRenderer>().materials;
                matArray[1].mainTexture = m_NumberTextures[m_WantedAmountPerCraftingGood[i]];
                m_InstantiatedIngredientAmountGameObjects[i].GetComponent<MeshRenderer>().materials = matArray;
            }

            else
            {
                m_InstantiatedIngredientAmountGameObjects[i].GetComponent<MeshRenderer>().material.mainTexture = m_NumberTextures[m_WantedAmountPerCraftingGood[i]];
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        //check if the exchanger received some goods to work with
        if (m_AmountOfWantedGoodsHasChanged)
        {
            m_AmountOfWantedGoodsHasChanged = false;
            //m_UIHandler.UpdateAmountList(m_WantedAmountPerCraftingGood);
            UpdateIngredientAmountsOnUI();
        }

        if (m_AmountOfExchangeGoodsHasChanged)
        {
            m_AmountOfExchangeGoodsHasChanged = false;
            //m_UIHandler.UpdateHoldAmount(m_CurrentlyHeldAmount);
        }

        //check if all goods are gathered for converting them into the exchange good
        if(m_ExchangedItemsAvailable == false)
        {
            if (CheckIfAllGoodsAreGathered())
            {
                if(m_InProcessingStage == false)
                {
                    m_InProcessingStage = true;
                }
                if (IsExchangeTimerFinished())
                {
                    m_InProcessingStage                 = false;
                    m_CurrentlyHeldAmount               = m_MaxHoldAmount;
                    m_ExchangedItemsAvailable           = true;
                    m_AmountOfExchangeGoodsHasChanged   = true;
                }
            }
        }
        

        if(m_CurrentlyHeldAmount == 0 && m_ExchangedItemsAvailable)
        {
            m_ExchangedItemsAvailable = false;
            ResetObjectAndWantedGoods();
            
        }

        UpdateExchangerLook();
        UpdateResultAmountUI();
        //all goods are gathered, taking in items is disabled
        //conversion begins over time
        //when time is finished, all gathered goods are set to empty and the exchanged goods are in full amount
        //objects are ready for taking
        //objects are ready for taking until player has taken all the exchanged goods, then "objects are ready" is false and items can be given again to the exchanger
    }

    private void UpdateResultAmountUI()
    {
        Transform resultAmountUI = m_IngredientAmountsUIGameObject.FindComponentInChildWithName<Transform>("Result_Amount");
        if (m_ExchangedItemsAvailable)
        {
            if (!m_ResultAmountUIActive)
            {
                resultAmountUI.gameObject.SetActive(true);
                m_ResultAmountUIActive = true;
            }


            if (resultAmountUI != null && m_ResultAmountUIActive)
            {
                resultAmountUI.gameObject.GetComponent<MeshRenderer>().material.mainTexture = m_NumberTextures[m_CurrentlyHeldAmount];
            }
        }
       

        if(!m_ExchangedItemsAvailable && m_ResultAmountUIActive)
        {
            resultAmountUI.gameObject.SetActive(false);
            m_ResultAmountUIActive = false;
        }
    }



    private void UpdateExchangerLook()
    {
        if (m_EmptyExchangerGameObject == null | m_FullExchangerGameObject == null | m_ProcessingExchangerGameObject == null)
        {
            //Debug.LogWarning("Still no empty or full mesh!");
            return;
        }

        //check if neither processing nor fully exchangable stage is active 
        if (!m_InProcessingStage && !m_ExchangedItemsAvailable)
        {
            if(!m_EmptyExchangerGameObject.activeSelf | m_FullExchangerGameObject.activeSelf | m_ProcessingExchangerGameObject.activeSelf)
            {
                m_EmptyExchangerGameObject.SetActive(true);
                m_FullExchangerGameObject.SetActive(false);
                m_ProcessingExchangerGameObject.SetActive(false);
                return;
            }
        }

        //check if processing stage is active 
        if (m_InProcessingStage)
        {
           
            if (m_EmptyExchangerGameObject.activeSelf | m_FullExchangerGameObject.activeSelf | !m_ProcessingExchangerGameObject.activeSelf)
            {
                m_EmptyExchangerGameObject.SetActive(false);
                m_FullExchangerGameObject.SetActive(false);
                m_ProcessingExchangerGameObject.SetActive(true);
                return;
            }
            
        }

        //check if exchanger is in exchanging stage
        if (m_ExchangedItemsAvailable)
        {
            if (m_EmptyExchangerGameObject.activeSelf | !m_FullExchangerGameObject.activeSelf | m_ProcessingExchangerGameObject.activeSelf)
            {
                m_EmptyExchangerGameObject.SetActive(false);
                m_FullExchangerGameObject.SetActive(true);
                m_ProcessingExchangerGameObject.SetActive(false);
                return;
            }
        }
    }

    private bool IsExchangeTimerFinished()
    {
        bool finished = false;
        if (m_SpawningTakesTime)
        {
            m_CurrentTime += Time.deltaTime;

            int spawningProgress = (int)((m_CurrentTime / m_SpawnTimePerExchange) * 100.0f);
            //m_UIHandler.UpdateSpawningProgress(spawningProgress);
            if (m_CurrentTime >= m_SpawnTimePerExchange)
            {
                m_CurrentTime = 0.0f;
                finished = true;
                

                return finished;
            }
            return finished;
        }
        else
        {
            finished = true;
            return finished;
        }
       
    }
    

    private bool CheckIfAllGoodsAreGathered()
    {
        for(int i = 0; i < m_WantedAmountPerCraftingGood.Count; i++)
        {
            if(m_WantedAmountPerCraftingGood[i] > 0)
            {
                return false;
            }
        }
        return true;
    }

    private void ResetObjectAndWantedGoods()
    {
        m_AmountOfWantedGoodsHasChanged = true;
        for (int i = 0; i < m_WantedCraftingGoods.craftingGoodDictionary.Length; i++)
        {
            m_WantedAmountPerCraftingGood[i]    = m_WantedCraftingGoods.craftingGoodDictionary[i].amount;
        }
    }

    public bool OnInteract(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {
        bool success = false;
        if (type == InteractionTypes.DropOff)
        {

            
                for (int i = 0; i < m_WantedAmountPerCraftingGood.Count; i++)
                {
                    if (playerInteractionController.currentInteractable.GetComponent<GatheringInteractable>().m_OwnCraftingGoodType == m_WantedCraftingGoods.craftingGoodDictionary[i].type)
                    {
                        if (m_WantedAmountPerCraftingGood[i] > 0)
                        {
                            m_WantedAmountPerCraftingGood[i]--;
                            playerInteractionController.TellPlayerToDropGood();
                            m_AmountOfWantedGoodsHasChanged = true;
                            success = true;
                            return success;
                        }

                    }
                }
            
            
               
            
        }
        if(type == InteractionTypes.PickUp)
        {
            if (m_CurrentlyHeldAmount > 0)
            {
                GameObject copyOfSpawnObject;
                copyOfSpawnObject   = Singletons.interactableManager.SetInactiveInteractableActive(m_ExchangedItem);
                playerInteractionController.GiveGoodToPlayer(copyOfSpawnObject);

                m_CurrentlyHeldAmount--;
                m_AmountOfExchangeGoodsHasChanged      = true;
                m_CurrentTime           = 0.0f;
                success                 = true;
                return success;
            }
        }

        return success;
    }
}
