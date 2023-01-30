using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractableManager : MonoBehaviour
{
    
    [SerializeField] List<Interactable>             m_RegisteredInteractables   = new List<Interactable>();
    [SerializeField] List<InteractableStructure>    m_RegisteredStructures      = new List<InteractableStructure>();
    List<GameObject>                                m_InactiveInteractables     = new List<GameObject>();
    [SerializeField] int                            m_OverallInteractablesAmount;

    [SerializeField] private Material m_HighLightMaterial; 


   
    

    private void Start()
    {
        m_InactiveInteractables = new List<GameObject>();
        for(int i = 0; i < m_OverallInteractablesAmount; i++)
        {
            GameObject newItem = new GameObject();
            newItem.SetActive(false);
            newItem.AddComponent<GatheringInteractable>();
            newItem.AddComponent<MeshRenderer>();
            newItem.AddComponent<MeshFilter>();
            newItem.AddComponent<SphereCollider>();
            m_InactiveInteractables.Add(newItem);
        }
    }

    public GameObject SetInactiveInteractableActive(GameObject original)
    {
        for(int i = 0; i < m_InactiveInteractables.Count; i++)
        {
            if(m_InactiveInteractables[i].activeInHierarchy == false)
            {
                InteractableType originalType               = original.GetComponent<Interactable>().m_InteractableType;
                Interactable newInteractable                = m_InactiveInteractables[i].GetComponent<Interactable>();

                newInteractable.m_InteractableType          = originalType;
                newInteractable.GetComponent<GatheringInteractable>().enabled = true;
                
                original.GetComponent<Interactable>().CopyInteractableValues(newInteractable);

                m_InactiveInteractables[i].GetComponent<SphereCollider>().radius            = original.GetComponent<SphereCollider>().radius;
                m_InactiveInteractables[i].GetComponent<MeshRenderer>().sharedMaterial      = original.GetComponent<MeshRenderer>().sharedMaterial;
                m_InactiveInteractables[i].GetComponent<MeshFilter>().sharedMesh            = original.GetComponent<MeshFilter>().sharedMesh;
                m_InactiveInteractables[i].gameObject.transform.localScale                  = original.transform.localScale;
                m_InactiveInteractables[i].SetActive(true);
                

                return m_InactiveInteractables[i];
            }
            
        }

        return null;

    }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void RegisterInteractable(Interactable interactable)
    {
        if (!m_RegisteredInteractables.Contains(interactable))
        {
            SetMaterials(interactable);
            m_RegisteredInteractables.Add(interactable);

        }
    }

    private void SetMaterials(Interactable newInteractable)
    {
        newInteractable.standardMaterial = newInteractable.GetComponent<MeshRenderer>().material;
        newInteractable.highlightMaterial = m_HighLightMaterial;
    }
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void RegisterStructure(InteractableStructure structure)
    {
        if (!m_RegisteredStructures.Contains(structure))
        {
            m_RegisteredStructures.Add(structure);
        }
    }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void RemoveInteractable(Interactable interactable)
    {

        if (!m_RegisteredInteractables.Remove(interactable) && m_RegisteredInteractables.Count > 0)
        {
            Debug.LogError("Interactable to remove was not found in list!");
        }
        
    }


    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public List<Interactable> GetAllInteractablesWithinRadius(Vector3 position, float radius)
    {
        List<Interactable> foundInteractables = new List<Interactable>();

        for(int i = 0; i < m_RegisteredInteractables.Count; i++)
        {
            float distance = GetDistance(position, m_RegisteredInteractables[i].GetGameObject().gameObject.transform.position);
            if ( distance < radius)
            {
                
               foundInteractables.Add(m_RegisteredInteractables[i]);
            }
        }
        return foundInteractables;

    }


    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private List<Interactable> CheckForInteractablesInRangeWithIgnore(Vector3 position, float radius, Interactable ignore)
    {
        List<Interactable> withinRadius = new List<Interactable>();

        //check which ones are in the radius
        for (int i = 0; i < m_RegisteredInteractables.Count; i++)
        {
            if (m_RegisteredInteractables[i].gameObject != ignore.gameObject)
            {
                float distance = GetDistance(position, m_RegisteredInteractables[i].GetGameObject().gameObject.transform.position);
                if (distance < radius)
                {
                    withinRadius.Add(m_RegisteredInteractables[i]);
                }
            }

        }
        return withinRadius;
    }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private List<Interactable> CheckForInteractablesInRange(Vector3 position, float radius)
    {
        List<Interactable> withinRadius = new List<Interactable>();

        //check which ones are in the radius
        for (int i = 0; i < m_RegisteredInteractables.Count; i++)
        {
             float distance = GetDistance(position, m_RegisteredInteractables[i].GetGameObject().gameObject.transform.position);
                if (distance < radius)
                {
                    withinRadius.Add(m_RegisteredInteractables[i]);
                }
            

        }
        return withinRadius;
    }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public Interactable GetClosestInteractableWithinRadiusAndInFront(Vector3 position, Vector3 forward, float radius, float fieldOfView, GameObject ignoreThisInteractable)
    {
        Interactable closest                = null;
        List<Interactable> withinRadius     = new List<Interactable>();

        if (ignoreThisInteractable != null)
        {
            withinRadius = CheckForInteractablesInRangeWithIgnore(position, radius, ignoreThisInteractable.GetComponent<Interactable>());
        }
        else
        {
            withinRadius = CheckForInteractablesInRange(position, radius);
        }

        //check which ones of those are within the field of view

        if(withinRadius.Count > 0)
        {
            float closestDegreeToForwardVector = fieldOfView;
            float highestPriority = 0.0f;

            
            for (int i = 0; i < withinRadius.Count; i++)
            {
                Vector2 withinRadiusXZPos   = new Vector2(withinRadius[i].gameObject.transform.position.x, withinRadius[i].gameObject.transform.position.z);
                Vector2 posToForward        = new Vector2((position.x + forward.x) - position.x, (position.z + forward.z) - position.z);
                Vector2 posToForwardWS      = new Vector2((position.x + forward.x), (position.z + forward.z));
                Vector2 posToObject         = new Vector2(withinRadiusXZPos.x - position.x, withinRadiusXZPos.y - position.z);
             

                float angle = Vector2.SignedAngle(posToForward, posToObject);
                


                float distance = GetDistance(position, withinRadius[i].GetGameObject().gameObject.transform.position);

                //calculating the priority between distance and angle

                float remappedAngle     = MathHelper.Remap(Mathf.Abs(angle), 0, fieldOfView, 1.0f, 0.0f);
                float remappedDistance  = MathHelper.Remap(distance, 0, radius, 1.0f, 0.0f);

                float priority = remappedAngle * remappedDistance;

               
                if(priority > highestPriority)
                {
                    highestPriority = priority;
                    closest = withinRadius[i];
                }
                
                
            }
        }



        return closest; // but check if something actually came back
    }
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public Interactable GetClosestInteractableWithinRadius(Vector3 position, float radius)
    {

        Interactable closest    = null;
        float shortestDistance  = radius;
        for (int i = 0; i < m_RegisteredInteractables.Count; i++)
        {
            float distance = GetDistance(position, m_RegisteredInteractables[i].GetGameObject().gameObject.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance    = distance;
                closest             = m_RegisteredInteractables[i];
            }
        }

        return closest; // but check if something actually came back
    }


    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool PointIntersectsWithStructureNoDropRadius(Vector3 point)
    {
        bool intersects = false;

        for(int i = 0; i < m_RegisteredStructures.Count; i++)
        {
            if(Vector3.Magnitude(point - m_RegisteredStructures[i].gameObject.transform.position) <= m_RegisteredStructures[i].m_NoDropRange)
            {
                intersects = true;
            }
        }
        return intersects;
    }


    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private float GetDistance(Vector3 origin, Vector3 pointOfInterest)
    {
        return Vector3.Distance(origin, pointOfInterest);
    }



    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Highlight Coroutines 

    public void HighLightSpecificInteractableForXSeconds(ref Interactable interactable, float seconds)
    {
        StartCoroutine(HighLightThisInteractableForXSeconds(seconds, interactable));
    }

    


    public void HighLightAllInteractablesForXSeconds(float seconds)
    {

        StartCoroutine(HighLightAllForXSeconds(seconds));
    }


    public void HighLightAllOfTypeForXSeconds(float seconds, string type)
    {
        StartCoroutine(HighLightAllOfTypeForSeconds(seconds, type));
    }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    IEnumerator HighLightAllOfTypeForSeconds(float seconds, string tag)
    {
        for (int i = 0; i < m_RegisteredInteractables.Count; i++)
        {
            if(m_RegisteredInteractables[i].tag == tag)
            {
                m_RegisteredInteractables[i].GetComponent<MeshRenderer>().material = m_RegisteredInteractables[i].highlightMaterial;
            }
            
        }
        yield return new WaitForSecondsRealtime(seconds);
        for (int i = 0; i < m_RegisteredInteractables.Count; i++)
        {
            if (m_RegisteredInteractables[i].tag == tag)
            {
                m_RegisteredInteractables[i].GetComponent<MeshRenderer>().material = m_RegisteredInteractables[i].standardMaterial;
            }
        }
    }

    IEnumerator HighLightThisInteractableForXSeconds(float seconds, Interactable interactable)
    {
       interactable.GetComponent<MeshRenderer>().material = interactable.highlightMaterial;
        
        yield return new WaitForSecondsRealtime(seconds);

       interactable.GetComponent<MeshRenderer>().material = interactable.standardMaterial;
        
    }

    IEnumerator HighLightAllForXSeconds(float seconds)
    {
        for (int i = 0; i < m_RegisteredInteractables.Count; i++)
        {
            m_RegisteredInteractables[i].GetComponent<MeshRenderer>().material = m_RegisteredInteractables[i].highlightMaterial;
        }
        yield return new WaitForSecondsRealtime(seconds);
        for (int i = 0; i < m_RegisteredInteractables.Count; i++)
        {
            m_RegisteredInteractables[i].GetComponent<MeshRenderer>().material = m_RegisteredInteractables[i].standardMaterial;
        }
    }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void DrawAllNoDropZones()
    {
        for(int i = 0; i<  m_RegisteredStructures.Count; i++)
        {
            DebugDrawingInterface.DrawPersistentCircleXZ(m_RegisteredStructures[i].transform.position, m_RegisteredStructures[i].m_NoDropRange, Color.black, 200); 
        }
    }




}
