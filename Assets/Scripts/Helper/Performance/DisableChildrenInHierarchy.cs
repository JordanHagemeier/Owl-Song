using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableChildrenInHierarchy : MonoBehaviour
{
    //get player position
    //take own array of children,and look how far away they are from the player
    //disable every child that is too far away and not disabled
    //enable each child that is now on the radius
    [SerializeField] private Transform m_PlayerTransform;
    [SerializeField] private float radius;
    [SerializeField] private Transform[] m_EnvironmentAssets;
    // Start is called before the first frame update
    void Start()
    {
        List<Transform> tempChildren = new List<Transform>();
        foreach(Transform child in gameObject.transform)
        {

            List<Transform> temp = GetAllTransformsInChild(child);
            foreach(Transform childtemp in temp)
            {
                tempChildren.Add(childtemp);
            }
        }


        m_EnvironmentAssets = new Transform[tempChildren.Count];
        for(int i = 0; i < tempChildren.Count; i++)
        {
            m_EnvironmentAssets[i] = tempChildren[i];
        }
    }

    private List<Transform> GetAllTransformsInChild(Transform parent)
    {
        List<Transform> tempChildren = new List<Transform>();
        foreach (Transform child in parent)
        {
            tempChildren.Add(child);

        }

        return tempChildren;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAllChildrenInRadius();
    }

    private void UpdateAllChildrenInRadius()
    {
        for(int i = 0; i < m_EnvironmentAssets.Length; i++)
        {
            //float distanceToPlayer = Vector3.Magnitude(new Vector3(m_EnvironmentAssets[i].position.x - m_PlayerTransform.position.x, m_EnvironmentAssets[i].position.y - m_PlayerTransform.position.y, m_EnvironmentAssets[i].position.z - m_PlayerTransform.position.z));
            float distanceToPlayer = Mathf.Abs(m_EnvironmentAssets[i].position.x - m_PlayerTransform.position.x);
            if(distanceToPlayer < radius && m_EnvironmentAssets[i].gameObject.activeSelf == false)
            {
                m_EnvironmentAssets[i].gameObject.SetActive(true);
            }
            if(distanceToPlayer > radius)
            {
                if(m_EnvironmentAssets[i].gameObject.activeSelf == true)
                {
                    m_EnvironmentAssets[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
