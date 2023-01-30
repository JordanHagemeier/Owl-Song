using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class ConstructionManager : MonoBehaviour
{
    public static event Action<bool> OnInMenu;

    [SerializeField] private AudioSource    m_source;
    [SerializeField] private AudioClip      nextclip;
    [SerializeField] private AudioClip      previousclip;
    [SerializeField] private AudioClip      cancelclip;
    [SerializeField] private AudioClip      confirmclip;
    [SerializeField] private AudioClip      openclip;
    private bool                            m_confirming = false;

    [SerializeField] private Transform      m_SelectedEmptyPlace;
                     private GameObject     m_BlueprintUIObject;
    [SerializeField] private Vector3        m_ConstructionOverlayHiddenPosition;
                     private int            m_CurrentPageNumber = 0;


    //Construction of Overlay
    [SerializeField] private GameObject     m_OverlayPagesRoot;
    [SerializeField] private GameObject     m_OverlayPageLayout;
    [SerializeField] private GameObject[]   m_InstantiatedIngredientGameObjects;
    [SerializeField] private GameObject[]   m_InstantiatedIngredientAmountGameObjects;

    [SerializeField] private List<DiaryPage>    m_BlueprintPages;
    [SerializeField] private List<GameObject>   m_ConstructedPages;

    private List<bool>                                  m_BlueprintPageConsumerConstructed;
    [SerializeField] GoodSubTypeToMaterialDictionary    m_GoodSubTypeToMaterialDictionary;
    [SerializeField] private Texture2D                  m_NonValidTexture;
    public Texture2D[] m_NumberTextures;


    private bool m_ConstructionOpened = false;
    [SerializeField] private float m_TimeTillMenuIsOfficiallyClosed = 0.5f;


    //Achievements
    private bool m_FirstEmptyInteraction = false;
    private bool m_FirstBlueprintBuilt = false;
    // Start is called before the first frame update
    void Start()
    {
        ConstructAllOverlayPages();
    }

    public void OpenConstructionOnSelectedEmpty(GameObject selectedEmpty)
    {

        if (m_ConstructionOpened)
        {
            return;
        }
        if(m_FirstEmptyInteraction == false)
        {
            m_FirstEmptyInteraction = true;
            Singletons.achievementManager.AchievementWasAccomplished(AchievementType.FirstDiaryInteraction);
        }

        OnInMenu(true);

        m_ConstructionOpened = true;
        Singletons.gameStateManager.m_InMenu = true;
        Singletons.gameStateManager.ChangePlayerMovementState(false);
        m_OverlayPagesRoot.SetActive(true);

        //rotates the upper part of the empty to appear 
        m_SelectedEmptyPlace = selectedEmpty.transform;
        //transforms the worldspace ui overlay to the position on the upper part 
        m_BlueprintUIObject = m_SelectedEmptyPlace.gameObject.FindComponentInChildWithTag<Transform>("EmptyBlueprintUI").gameObject;
        m_BlueprintUIObject.gameObject.FindComponentInChildWithName<Light>("Spot Light").gameObject.SetActive(true);
        StartCoroutine(RotateBlueprint(m_BlueprintUIObject.transform, -90.0f, 0.5f));

        //enables as many "ingredient" slots as are needed  and transforms them with their own offset to the new parent empty
        for (int i = 0; i< m_InstantiatedIngredientGameObjects.Length; i++)
        {
            int number                                      = i + 1;
            string nameOfMaterial                           = "Material_" + number.ToString();
            string amountOfMaterial                         = "Amount_" + number.ToString();
            m_InstantiatedIngredientGameObjects[i]          = m_BlueprintUIObject.FindComponentInChildWithName<Transform>(nameOfMaterial).gameObject;
            m_InstantiatedIngredientAmountGameObjects[i]    = m_BlueprintUIObject.FindComponentInChildWithName<Transform>(amountOfMaterial).gameObject;
            
        }


        EnableNextPage(m_CurrentPageNumber);
        PlayPaperSound(openclip);
    }


    IEnumerator RotateBlueprint(Transform blueprintTransform, float directionDegrees, float duration)
    {
        float startRotation     = blueprintTransform.localEulerAngles.x;
        float endRotation       = startRotation + directionDegrees;
        float t                 = 0.0f;

        while (t < duration)
        {
            t += Time.deltaTime;

          
            float Rotation = Mathf.Lerp(startRotation, endRotation, t / duration);
            blueprintTransform.localEulerAngles = new Vector3(Rotation, 0.0f, 0.0f);
            
            yield return null;
        }
        blueprintTransform.localEulerAngles = new Vector3(endRotation, 0.0f, 0.0f);
    }
    private void PlayPaperSound(AudioClip clip)
    {
        m_source.clip = clip;
        m_source.Play();
        //play knister sound
    }

    GameObject GetCurrentPageObject()
    {
        return m_ConstructedPages[m_CurrentPageNumber];
    }


    public void SwitchToNextBlueprintOnDisplay()
    {
        

        GetCurrentPageObject().SetActive(false);
        m_CurrentPageNumber++;
        if(m_CurrentPageNumber >= m_ConstructedPages.Count)
        {
            m_CurrentPageNumber = 0;
        }
        GetCurrentPageObject().SetActive(true);
        EnableNextPage(m_CurrentPageNumber);
        PlayPaperSound(nextclip);
    }

    public void SwitchToPreviousBlueprintOnDisplay()
    {
        GetCurrentPageObject().SetActive(false);
        m_CurrentPageNumber--;
        if (m_CurrentPageNumber < 0)
        {
            m_CurrentPageNumber = m_ConstructedPages.Count -1;
        }
        GetCurrentPageObject().SetActive(true);
        EnableNextPage(m_CurrentPageNumber);
        PlayPaperSound(previousclip);
    }

    public void ExitConstructionOnSelectedEmptyWithoutBuilding()
    {

        //rotate the upper part down again
        //disables the activated "ingredient" slots
        //sets the worldspace ui back to its null position
        StartCoroutine(RotateBlueprint(m_BlueprintUIObject.transform, 90.0f, 0.5f));
        m_BlueprintUIObject.gameObject.FindComponentInChildWithName<Light>("Spot Light").gameObject.SetActive(false);

        m_confirming = false;
        CloseConstructionSite();
    }

    public void BuildDisplayedBlueprintOnSelectedEmpty()
    {
        //clone the current construction page
        //set the worldspace ui back to its null position
        //give the empty to the blueprint manager

        if (m_BlueprintPageConsumerConstructed[m_CurrentPageNumber])
        {
            Debug.LogWarning("Trying to build a blueprint that is already built");
            return;
        }

        m_BlueprintPageConsumerConstructed[m_CurrentPageNumber] = true;
        Singletons.blueprintManager.ChangeEmptyToBlueprint(m_BlueprintPages[m_CurrentPageNumber], m_SelectedEmptyPlace.GetComponent<EmptyInteraction>());
        if(m_FirstBlueprintBuilt == false)
        {
            m_FirstBlueprintBuilt = true;
            Singletons.achievementManager.AchievementWasAccomplished(AchievementType.FirstBuildingPlanned);

        }
        m_confirming = true;
        CloseConstructionSite();
    }

    private void CloseConstructionSite()
    {
        
        Singletons.gameStateManager.ChangePlayerMovementState(true);
        m_ConstructionOpened                    = false;
        StartCoroutine(WaitToTellGamestateManagerThatInventoryHasClosed(m_TimeTillMenuIsOfficiallyClosed));
        Singletons.gameStateManager.m_PlayerInteractionController.TellPlayerToStopInteractionAnimation();
        if (m_confirming)
        { PlayPaperSound(confirmclip); }
        else
        { PlayPaperSound(cancelclip); }
        OnInMenu(false);
    }


    private void ConstructAllOverlayPages()
    {
        m_BlueprintPageConsumerConstructed = new List<bool>();
        for (int k = 0; k < m_BlueprintPages.Count; k++)
        {
            m_BlueprintPageConsumerConstructed.Add(false);
        }


        for (int i = 0; i < m_BlueprintPages.Count; i++)
        {
            ConstructCurrentOverlayPage(i);
        }

        m_OverlayPagesRoot.SetActive(false);
        m_OverlayPageLayout.SetActive(false);
    }

    private void EnableNextPage(int pageNumber)
    {
        //get materials for the ingredient slots
        BluePrintToStructureMap.BlueprintToStructure currentBlueprint = m_BlueprintPages[pageNumber].m_BlueprintWithStructure.m_blueprintToStructure;
        SetAllIngredientTextures(currentBlueprint);
        SetUpStructureImage(m_BlueprintPageConsumerConstructed[pageNumber]);

    }

    private void SetAllIngredientTextures(BluePrintToStructureMap.BlueprintToStructure currentBlueprint)
    {
        for(int k = 0; k < m_InstantiatedIngredientGameObjects.Length; k++)
        {
            m_InstantiatedIngredientGameObjects[k].SetActive(false);
            m_InstantiatedIngredientAmountGameObjects[k].SetActive(false);
        }


        for (int i = 0; i < currentBlueprint.blueprintRecipes.Length; i++)
        {
            GoodSubTypes ingredient = currentBlueprint.blueprintRecipes[i].type;
            int amount = currentBlueprint.blueprintRecipes[i].amount;


            Texture2D ingredientTexture = m_NonValidTexture;
            for (int k = 0; k < m_GoodSubTypeToMaterialDictionary.subTypeTextureDictionary.Length; k++)
            {
                if (m_GoodSubTypeToMaterialDictionary.subTypeTextureDictionary[k].type == ingredient)
                {
                    ingredientTexture = m_GoodSubTypeToMaterialDictionary.subTypeTextureDictionary[k].texture;
                }
            }

            m_InstantiatedIngredientGameObjects[i].GetComponent<MeshRenderer>().material.mainTexture = ingredientTexture;
            m_InstantiatedIngredientAmountGameObjects[i].GetComponent<MeshRenderer>().material.mainTexture = m_NumberTextures[amount]; 
            m_InstantiatedIngredientGameObjects[i].SetActive(true);
            m_InstantiatedIngredientAmountGameObjects[i].SetActive(true);

        }
    }

    private void SetUpStructureImage(bool alreadyConstructed)
    {
        GameObject structureImageGO                             = m_BlueprintUIObject.gameObject.FindComponentInChildWithName<Transform>("Blueprint_Menu_Pappaufsteller").gameObject;
        var materials                                           = structureImageGO.GetComponent<MeshRenderer>().materials;
        if (alreadyConstructed)
        {
            materials[1].mainTexture = m_BlueprintPages[m_CurrentPageNumber].m_FinishedStructureImage;
        }
        else
        {
            materials[1].mainTexture                                = m_BlueprintPages[m_CurrentPageNumber].m_StructureImage;

        }
        structureImageGO.GetComponent<MeshRenderer>().materials = materials;
        
    }
    private void ConstructCurrentOverlayPage(int pageNumber)
    {
        GameObject newPageLayout = Instantiate(m_OverlayPageLayout, m_OverlayPagesRoot.transform);
        m_ConstructedPages.Add(newPageLayout);
        newPageLayout.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInput();
       
        
        
    }


    IEnumerator WaitToTellGamestateManagerThatInventoryHasClosed(float waitingtime)
    {
        yield return new WaitForSeconds(waitingtime);
        Singletons.gameStateManager.m_InMenu = false;
    }

    private void CheckForInput()
    {
        if (m_ConstructionOpened)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SwitchToPreviousBlueprintOnDisplay();
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                SwitchToNextBlueprintOnDisplay();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                BuildDisplayedBlueprintOnSelectedEmpty();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitConstructionOnSelectedEmptyWithoutBuilding();
            }
        }
       
    }


   
}


public static class Helper
{
    public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag) where T : Component
    {
        Transform t = parent.transform;
        //parent.transform.GetChild();
        //parent.transform.childCount;
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }

    public static T FindComponentInChildWithName<T>(this GameObject parent, string name) where T : Component
    {
        Transform t = parent.transform;
        foreach (Transform tr in t)
        {
            if (tr.name == name)
            {
                return tr.GetComponent<T>();
            }
        }
        return null;
    }
}