using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;



public class DiaryManager : MonoBehaviour
{
    [SerializeField] private EmptyInteraction           m_CallingEmpty;

    //Construction gameObject for blueprint symbolization
   
    //UI Elements of Diary
    [SerializeField] private GameObject                 m_DiaryPagePanel;
    [SerializeField] private GameObject                 m_DiaryPageLayout; 
    [SerializeField] private GameObject                 buttonToSelect; // < Debug only
    [SerializeField] private List<GameObject>           m_ConstructedPages;

    private List<bool>                                  m_DiaryPageConsumerConstructed;
   

    //Diary Page Counting and Management
    [SerializeField] private List<DiaryPage>            m_DiaryPages;
    [SerializeField] private int                        m_CurrentPageNumber         = 0;

    [SerializeField] private bool                       m_DiaryIsOpen               = false;
    private bool                                        m_DiaryCanBeOpened          = true;
    [SerializeField] private float                      m_DiaryBetweenInteractionsThreshold;
    private float                                       m_DiaryOpenTimer; 
    //1. game manager should know which buildings already exist
    //1.1. when page is constructed, check if blueprint was already built, if yes: activate "done" check, deactivate "build" option
    //1.1. on construction: check if there is a next page, if no: deactivate the "next" button. Same with "previous"
    //1.2. "cancel" always exists
    //2. getting the information from page in List via current Page Number
    //2.1. getting structure "image" from page 
    //2.1.1. does the image already involve the amount of goods? or is that something that needs to be created on page creation?
    
    
    // Start is called before the first frame update
    void Start()
    {
        m_DiaryPageConsumerConstructed = new List<bool>();
        for(int k = 0; k < m_DiaryPages.Count; k++)
        {
            m_DiaryPageConsumerConstructed.Add(false);
        }


        for(int i = 0; i < m_DiaryPages.Count; i++)
        {
            ConstructCurrentDiaryPageUI(i);
        }
        m_DiaryPagePanel.SetActive(false);
        m_DiaryPageLayout.SetActive(false);
    }

    GameObject GetCurrentPageObject()
    {
        return m_ConstructedPages[m_CurrentPageNumber];
    }

    public void TryOpenDiary(EmptyInteraction callingEmpty)
    {
        if (m_DiaryIsOpen || !m_DiaryCanBeOpened)
        {
            return;
        }

        m_DiaryIsOpen = true;
        m_CallingEmpty = callingEmpty;
        Time.timeScale = 0.0f;
        m_DiaryPagePanel.SetActive(true);
        m_CurrentPageNumber = 0;

        //we need to set the ui element with the button on it active before setting the button as the selected gameObject! 
        GetCurrentPageObject().SetActive(true);
        SetSelectedButton("Next");
    }

    private void SetSelectedButton(string previousSelectedButton)
    {
        buttonToSelect = GetCurrentPageObject().FindComponentInChildWithTag<Button>(previousSelectedButton).gameObject;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }


    public void CloseDiary()
    {
        m_DiaryPagePanel.SetActive(false);
        GetCurrentPageObject().SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale      = 1.0f; // Implement pause inside of player instead?!
        m_CurrentPageNumber = 0;
        m_DiaryIsOpen       = false;
        m_DiaryCanBeOpened  = false;

    }

    

    public void OpenNextPage()
    {
        Debug.Assert(m_CurrentPageNumber < m_DiaryPages.Count - 1, "Trying to open page after end of diary");
        if (m_CurrentPageNumber >= m_DiaryPages.Count - 1)
        {
            return;
        }

        GetCurrentPageObject().SetActive(false);
        m_CurrentPageNumber++;
        GetCurrentPageObject().SetActive(true);

        bool isLastPage = m_CurrentPageNumber == m_DiaryPages.Count - 1;
        if (isLastPage)
        {
            SetSelectedButton("Previous");
        }
        else
        {
            SetSelectedButton("Next");
        }

    }

    public void OpenPreviousPage()
    {
        if (m_CurrentPageNumber - 1 >= 0)
        {
            GetCurrentPageObject().SetActive(false);
            m_CurrentPageNumber--;
            GetCurrentPageObject().SetActive(true);
            if(m_CurrentPageNumber == 0)
            {
                SetSelectedButton("Next");
            }
            else
            {
                SetSelectedButton("Previous");
            }
           
        }
    }

    public void BuildBlueprint()
    {
        if (m_DiaryPageConsumerConstructed[m_CurrentPageNumber])
        {
            Debug.LogWarning("Trying to build a blueprint that is already built");
            return;
        }

        m_DiaryPageConsumerConstructed[m_CurrentPageNumber] = true;
        Button currentBuildButton = GetCurrentPageObject().FindComponentInChildWithTag<Button>("Build");
        currentBuildButton.interactable = false;
        Singletons.blueprintManager.ChangeEmptyToBlueprint(m_DiaryPages[m_CurrentPageNumber], m_CallingEmpty);

        CloseDiary();
    }

    
    private void ConstructCurrentDiaryPageUI(int pageNumber)
    {
       
        GameObject newPageLayout = Instantiate(m_DiaryPageLayout, m_DiaryPagePanel.transform);

        //check if this page is first or last page and disable corresponding buttons like "previous" on first page and "next" on last page
        if(pageNumber == 0)
        {
           
            Button currentBuildButton = newPageLayout.FindComponentInChildWithTag<Button>("Previous");
            currentBuildButton.interactable = false;
        }
        else if(pageNumber == m_DiaryPages.Count - 1)
        {
            Button currentBuildButton = newPageLayout.FindComponentInChildWithTag<Button>("Next");
            currentBuildButton.interactable = false;
        }


        BluePrintToStructureMap.BlueprintToStructure currentBlueprint    = m_DiaryPages[pageNumber].m_BlueprintWithStructure.m_blueprintToStructure;

        //convert the structure name to text
        TextMeshProUGUI structureName = newPageLayout.FindComponentInChildWithTag<TextMeshProUGUI>("StructureName");
        structureName.SetText(currentBlueprint.blueprint.ToString());

        //convert the ingredient names and amounts to text
        TextMeshProUGUI ingredientText                          = newPageLayout.FindComponentInChildWithTag<TextMeshProUGUI>("Needed");
        string ingredients                                      = "";


        for (int i = 0; i < currentBlueprint.blueprintRecipes.Length; i++)
        {
            string ingredient   = currentBlueprint.blueprintRecipes[i].type.ToString();
            string amount       = currentBlueprint.blueprintRecipes[i].amount.ToString();
            ingredients         += " " + ingredient + " x " + amount;
            
        }
        ingredientText.SetText(ingredients);

        m_ConstructedPages.Add(newPageLayout);
        newPageLayout.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_DiaryCanBeOpened)
        {
            UpdateDiaryOpenCooldown();
        }
    }

    void UpdateDiaryOpenCooldown()
    {
        m_DiaryOpenTimer += Time.deltaTime;
        if (m_DiaryOpenTimer >= m_DiaryBetweenInteractionsThreshold)
        {
            m_DiaryOpenTimer = 0.0f;
            m_DiaryCanBeOpened = true;
        }
    }
}
