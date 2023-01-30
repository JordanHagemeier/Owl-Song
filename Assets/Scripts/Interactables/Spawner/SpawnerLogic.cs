using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerLogic : MonoBehaviour
{
    [SerializeField] private GameObject m_SpawningGood;

    [SerializeField]    private float               m_SpawnTime;
    [SerializeField]    private float               m_CurrentTimeSinceLastSpawn = 0.0f;

    [SerializeField]    private int                 m_HoldAmount;
    [SerializeField]    private int                 m_CurrentlyHeldAmount = 0;
    [SerializeField]    private bool                m_HoldAmountFull = false;
                        private bool                m_HoldAmountChanged = true;

    
    [SerializeField]         GameObject             m_UICanvas; public GameObject UICanvas              { get { return m_UICanvas; } set { m_UICanvas = value; } }
    [SerializeField] private ConsumerUITextHandler  m_UIHandler; public ConsumerUITextHandler UIHandler { get { return m_UIHandler; } set { m_UIHandler = value; } }

    [SerializeField] private GameObject             m_EmptySpawnerMesh;
    [SerializeField] private GameObject             m_FullSpawnerMesh;


    // Start is called before the first frame update
    void Start()
    {
        m_CurrentlyHeldAmount = m_HoldAmount;
        m_HoldAmountFull = true;

        if( m_EmptySpawnerMesh == null | m_FullSpawnerMesh == null )
        {
            Debug.LogError("Empty or Full Mesh are not given! Please check this Spawner: " + gameObject.name.ToString());
        }
        SetUpUIHandler();
    }
    private void SetUpUIHandler()
    {
        //m_UIHandler                                 = gameObject.AddComponent<ConsumerUITextHandler>();
        if (m_UIHandler == null)
        {
            m_UIHandler = gameObject.AddComponent<ConsumerUITextHandler>();
        }
        
            m_UIHandler.blueprintConstructionCanvas = m_UICanvas;
            m_UIHandler.givenGood = m_SpawningGood.GetComponent<GatheringInteractable>();
            m_UIHandler.UpdateHoldAmount(m_CurrentlyHeldAmount);
            m_UIHandler.SetUpBlueprintCanvasWithText(gameObject.transform);
        

    }
    // Update is called once per frame
    void Update()
    {

        UpdateSpawner();
        UpdateSpawnerLook();
        if (m_HoldAmountChanged)
        {
            m_HoldAmountChanged = false;
            m_UIHandler.UpdateHoldAmount(m_CurrentlyHeldAmount);
        }


    }

    public bool CanPickUpGood()
    {
        if (m_CurrentlyHeldAmount > 0)
        {
            return true;
        }
        return false;
    }

    private void UpdateSpawner()
    {
        if(m_CurrentlyHeldAmount < m_HoldAmount)
        {
            
            m_CurrentTimeSinceLastSpawn += Time.deltaTime;
            if(m_CurrentTimeSinceLastSpawn >= m_SpawnTime)
            {
                m_CurrentlyHeldAmount++;
                m_CurrentTimeSinceLastSpawn = 0.0f;
                m_HoldAmountChanged = true;
            }
        }
        
    }

    private void UpdateSpawnerLook()
    {
        if( m_CurrentlyHeldAmount == 0)
        {
            if(m_EmptySpawnerMesh == null | m_FullSpawnerMesh == null)
            {
                //Debug.LogWarning("Still no empty or full mesh!");
                return;
            }
            m_EmptySpawnerMesh.SetActive(true);
            m_FullSpawnerMesh.SetActive(false);
        }
        if( m_CurrentlyHeldAmount > 0)
        {
            if (m_EmptySpawnerMesh == null | m_FullSpawnerMesh == null)
            {
                //Debug.LogWarning("Still no empty or full mesh!");
                return;
            }
            //gameObject.GetComponent<MeshFilter>().mesh = m_FullSpawnerMesh;
            m_EmptySpawnerMesh.SetActive(false);
            m_FullSpawnerMesh.SetActive(true);
        }
    }

    public bool OnInteract(InteractionTypes type, PlayerInteractionController playerInteractionController)
    {
        bool success = false;
        if (type == InteractionTypes.PickUp)
        {
            if (m_CurrentlyHeldAmount > 0)
            {

                
                GameObject copyOfSpawnObject;
                copyOfSpawnObject = Singletons.interactableManager.SetInactiveInteractableActive(m_SpawningGood);
                playerInteractionController.GiveGoodToPlayer(copyOfSpawnObject);
                m_CurrentlyHeldAmount--;
                m_CurrentTimeSinceLastSpawn = 0.0f;
                m_HoldAmountChanged = true;

                success = true;
                
                
               
            }
            else
            {
                Debug.Log("Spawner has not finished spawning.");
            }
        }

        return success;
    }
}
