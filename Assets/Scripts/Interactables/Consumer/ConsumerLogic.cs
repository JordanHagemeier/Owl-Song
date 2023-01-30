using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConsumerLogic : MonoBehaviour
{
    
   
    [SerializeField] private BlueprintTypes         m_OwnBluePrintType;                                 public BlueprintTypes ownBluePrintType { get { return m_OwnBluePrintType; } set { m_OwnBluePrintType = value; } }
    [SerializeField] private CraftingGoodDictonary  m_WantedCraftingGoods;                              public CraftingGoodDictonary wantedCraftingGoods { get { return m_WantedCraftingGoods; } set { m_WantedCraftingGoods = value; } }
    [SerializeField] private List<int>              m_WantedAmountPerCraftingGood   = new List<int>();
    [SerializeField] private GameObject             m_DesiredStructure              = null;             public GameObject desiredStructure { get { return m_DesiredStructure; } set { m_DesiredStructure = value; } }
    //Consumer UI
    private bool                                    amountsHaveChanged              = false;
    //[SerializeField] GameObject                     m_UICanvas;                                         public GameObject UICanvas { get { return m_UICanvas; } set { m_UICanvas = value; } }
    //[SerializeField] private ConsumerUITextHandler  m_UIHandler;                                        public ConsumerUITextHandler UIHandler { get { return m_UIHandler; } set { m_UIHandler = value; } }

    [SerializeField] public     GameObject m_IngredientAmountsUIGameObject;
    [SerializeField] private    GameObject[] m_InstantiatedIngredientGameObjects;
    [SerializeField] private    GameObject[] m_InstantiatedIngredientAmountGameObjects;
    public Texture2D m_ExchangedItemTexture;
    public Texture2D[] m_NumberTextures;
    public GoodSubTypeToMaterialDictionary m_GoodSubTypeToMaterialDictionary;
    [SerializeField] private Texture2D m_NonValidTexture;

    public static event Action<bool> OnDrop;

    // Start is called before the first frame update
    void Start()
    {
        if(m_WantedCraftingGoods != null)
        {
            for (int i = 0; i < m_WantedCraftingGoods.craftingGoodDictionary.Length; i++)
            {
                int amountInTheBeginning = m_WantedCraftingGoods.craftingGoodDictionary[i].amount;
                m_WantedAmountPerCraftingGood.Add(amountInTheBeginning);
            }
            if(m_IngredientAmountsUIGameObject != null)
            {
                SetUpIngredientAmountsOnUI();

            }
            //SetUpUIHandler();
        }

        Transform LightGO = m_IngredientAmountsUIGameObject.FindComponentInChildWithName<Transform>("Spot Light");
        Singletons.daytimeManager.AddLightToAffectedLights(LightGO.GetComponent<Light>());



    }

    //private void SetUpUIHandler()
    //{
    //    //m_UIHandler                                 = gameObject.AddComponent<ConsumerUITextHandler>();
    //    if (m_UIHandler == null)
    //    {
    //        m_UIHandler = gameObject.AddComponent<ConsumerUITextHandler>();
    //    }
    //    m_UIHandler.blueprintConstructionCanvas     = m_UICanvas;
    //    m_UIHandler.wantedCraftingGoods             = m_WantedCraftingGoods;
    //    m_UIHandler.wantedAmountPerCraftingGood     = m_WantedAmountPerCraftingGood;
    //    m_UIHandler.SetUpBlueprintCanvasWithText(gameObject.transform);
        
       
    //}

    // Update is called once per frame
    void Update()
    {
        if (CheckIfAllGoodsAreGathered())
        {
            Singletons.blueprintManager.ConsumerIsDone(this);
        }

        if (amountsHaveChanged)
        {
            amountsHaveChanged = false;
            UpdateIngredientAmountsOnUI();
            OnDrop(false);
            //if(m_UIHandler != null)
            //{
            //    m_UIHandler.UpdateAmountList(m_WantedAmountPerCraftingGood);
            //}

        }
    }


    private void DisableAllWSUIElementsForNow()
    {
        int length = 4;
        if(m_IngredientAmountsUIGameObject.gameObject.tag == "EmptyBlueprintUI")
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
            
            if(m_InstantiatedIngredientGameObjects[i].GetComponent<MeshRenderer>().materials.Length > 1)
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
        for(int i = 0; i < m_WantedCraftingGoods.craftingGoodDictionary.Length; i++)
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

    private bool CheckIfAllGoodsAreGathered()
    {
        for (int i = 0; i < m_WantedAmountPerCraftingGood.Count; i++)
        {
            if (m_WantedAmountPerCraftingGood[i] > 0)
            {
                return false;
            }
        }
        return true;
    }

    public bool OnInteract(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {
        bool success = false;
        if (type == InteractionTypes.DropOff)
        {
            
          
            GoodSubTypes givenCraftinGoodType = playerInteractionController.currentInteractable.GetComponent<GatheringInteractable>().m_OwnCraftingGoodType;
            for (int i = 0; i < m_WantedAmountPerCraftingGood.Count; i++)
            {
                if (givenCraftinGoodType == m_WantedCraftingGoods.craftingGoodDictionary[i].type)
                {
                    if (m_WantedAmountPerCraftingGood[i] > 0)
                    {
                        m_WantedAmountPerCraftingGood[i]--;
                        playerInteractionController.TellPlayerToDropGood();
                        amountsHaveChanged = true;

                        success = true;
                        return success;
                    }

                }
            }

            Debug.Log("Consumer cannot take Good!");
            
        }

        return success;
    }
}

