using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float m_PlaceholderRadius;

    public InteractableType m_InteractableType;

    [SerializeField] private string m_InteractableName;

    [SerializeField] private bool m_IsCurrentlyInFocus = false;  public bool isCurrentlyInFocus { get { return m_IsCurrentlyInFocus; } set { m_IsCurrentlyInFocus = value; } }
    [SerializeField] private bool m_IsHighLighted = false;
    [SerializeField] private Material m_StandardMaterial; public Material standardMaterial { get { return m_StandardMaterial; } set { m_StandardMaterial = value; } }
    [SerializeField] private Material m_HighLightMaterial; public Material highlightMaterial { get { return m_HighLightMaterial; } set { m_HighLightMaterial = value; } }

    public virtual void CopyInteractableValues(Interactable toCopyTo)
    {
        toCopyTo.GetComponent<Interactable>().m_PlaceholderRadius   = m_PlaceholderRadius;
        toCopyTo.GetComponent<Interactable>().m_StandardMaterial    = m_StandardMaterial;
        toCopyTo.GetComponent<Interactable>().m_HighLightMaterial   = m_HighLightMaterial;
        toCopyTo.GetComponent<Interactable>().m_InteractableName    = m_InteractableName;
        toCopyTo.GetComponent<Interactable>().m_InteractableType    = m_InteractableType;
    }
    public virtual void Awake()
    {
        Singletons.interactableManager.RegisterInteractable(this);
        m_PlaceholderRadius = gameObject.transform.localScale.x * 0.5f;

    }

    private void OnEnable()
    {
        Singletons.interactableManager.RegisterInteractable(this);
        m_PlaceholderRadius = gameObject.transform.localScale.x * 0.5f;
    }

    public virtual void OnDisable()
    {
        if (!Singletons.IsShuttingDown())
        {
            Singletons.interactableManager.RemoveInteractable(this);
        }
        
    }
    public abstract bool OnInteract(InteractionTypes type, PlayerInteractionController playerInteractionController);
    public GameObject GetGameObject() {  return gameObject; }


    public virtual void Update()
    {
        CheckIfIsInFocusOfPlayer();
    }

    private void CheckIfIsInFocusOfPlayer()
    {
        if (m_IsCurrentlyInFocus && !m_IsHighLighted)
        {
            gameObject.GetComponent<MeshRenderer>().material = m_HighLightMaterial;
            m_IsHighLighted = true;
        }
        if (!m_IsCurrentlyInFocus && m_IsHighLighted)
        {
            gameObject.GetComponent<MeshRenderer>().material = m_StandardMaterial;
            m_IsHighLighted = false;
        }
        m_IsCurrentlyInFocus = false;
    }
}