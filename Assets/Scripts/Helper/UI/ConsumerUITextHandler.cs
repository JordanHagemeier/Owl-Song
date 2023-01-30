using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConsumerUITextHandler : MonoBehaviour
{
    [SerializeField] private GameObject                 m_BlueprintConstructionCanvas;                      public GameObject blueprintConstructionCanvas { get { return m_BlueprintConstructionCanvas; } set { m_BlueprintConstructionCanvas = value; } }
    //what does the consumer/exchanger want?
    [SerializeField] private CraftingGoodDictonary      m_WantedCraftingGoods;                              public CraftingGoodDictonary wantedCraftingGoods { get { return m_WantedCraftingGoods; } set { m_WantedCraftingGoods = value; } }
    [SerializeField] private List<int>                  m_WantedAmountPerCraftingGood   = new List<int>();  public List<int> wantedAmountPerCraftingGood { get { return m_WantedAmountPerCraftingGood; } set { m_WantedAmountPerCraftingGood = value; } }
    GameObject m_BlueprintCanvas;

    //what did it already produce?
    private int                                         m_CurrentlyHeldAmount           = 0;
    private GatheringInteractable                       m_GivenGood;                                        public GatheringInteractable givenGood { get { return m_GivenGood; } set { m_GivenGood = value; } }
    private int                                         m_SpawningProgress              = 0;
    private int                                         m_LastSpawningProgress          = 0;
    private bool                                        m_IsSpawning                    = false;
   
    private bool                                        m_TextInputsHaveChanged         = false;
    private bool                                        m_ClearText = false;

    [Header("Text Update")]
    [SerializeField] private float                      m_UpdateRate;
    private int                                         m_FrameCount                    = 0;
    private float                                       m_AccumulatedDeltaTime          = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_UpdateRate = 4.0f;
        SetUpBlueprintCanvasWithText(gameObject.transform);
        LocalisationManager.OnLanguageChanged += UpdateTextWithNewAmounts;
    }

    private void OnDestroy()
    {
        LocalisationManager.OnLanguageChanged -= UpdateTextWithNewAmounts;
    }

    private void OnEnable()
    {
        SetUpBlueprintCanvasWithText(gameObject.transform);
    }
    // Update is called once per frame
    void Update()
    {
        if (m_TextInputsHaveChanged)
        {
            m_TextInputsHaveChanged = false;
            UpdateTextWithNewAmounts();
        }
    }

    public void UpdateAmountList(List<int> newAmounts)
    {
        m_WantedAmountPerCraftingGood   = newAmounts;
        m_TextInputsHaveChanged         = true;
    }

    public void UpdateHoldAmount(int newHoldAmount)
    {
        m_CurrentlyHeldAmount           = newHoldAmount;
        m_TextInputsHaveChanged         = true;
    }

    public void UpdateSpawningProgress(int progress)
    {
        m_IsSpawning                    = true;
        m_SpawningProgress              = progress;
        m_TextInputsHaveChanged         = true;
    }


    public void ClearAllText()
    {
        m_ClearText                     = true;
        m_TextInputsHaveChanged         = true;
    }
    private float CalculateCurrentProgressInIntervals()
    {
        
       
        m_FrameCount++;
        m_AccumulatedDeltaTime += Time.deltaTime;
        if (m_AccumulatedDeltaTime > 1.0 / m_UpdateRate)
        {
            m_LastSpawningProgress = m_SpawningProgress;
            m_FrameCount = 0;
            m_AccumulatedDeltaTime -= 1.0f / m_UpdateRate;
        }
        float progressPerInterval = m_LastSpawningProgress;
        return progressPerInterval;
    }

    private void UpdateTextWithNewAmounts()
    {

        string finalText = null;
        
        if(m_WantedCraftingGoods != null)
        {
            string ingredients = null;
            for (int i = 0; i < m_WantedCraftingGoods.craftingGoodDictionary.Length; i++)
            {
                string ingredient = LocalisationManager.GetLocalisedValue("$GATHER_" + m_WantedCraftingGoods.craftingGoodDictionary[i].type.ToString());
                string amount = m_WantedAmountPerCraftingGood[i].ToString() + " \n";
                ingredients += " " + ingredient + " x " + amount;

            }

            finalText = ingredients;
        }

        if (m_IsSpawning)
        {
           
            m_IsSpawning = false;
            string spawningProgress = CalculateCurrentProgressInIntervals().ToString() + " %";
            finalText = finalText + " \n" + spawningProgress;
        }

        if (m_GivenGood != null)
        {
            string holdAmount = LocalisationManager.GetLocalisedValue("$GATHER_" + m_GivenGood.m_OwnCraftingGoodType.ToString()) + " x " + m_CurrentlyHeldAmount.ToString();
            finalText = holdAmount + " \n" + finalText;

            
        }

        if (m_ClearText)
        {
            m_ClearText = false;
            finalText = "";
        }

        if(m_BlueprintCanvas != null && m_BlueprintCanvas.GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            m_BlueprintCanvas.GetComponentInChildren<TextMeshProUGUI>().text = null;
            m_BlueprintCanvas.GetComponentInChildren<TextMeshProUGUI>().text = finalText;
        }
        
    }

    public void SetUpBlueprintCanvasWithText(Transform parent)
    {
        if(m_BlueprintConstructionCanvas != null && m_BlueprintCanvas == null)
        {
            m_BlueprintCanvas = Instantiate(m_BlueprintConstructionCanvas, parent);
            m_BlueprintCanvas.SetActive(true);
            m_BlueprintCanvas.gameObject.transform.position = parent.position;
            m_TextInputsHaveChanged = true;
        }
       
    }
}
