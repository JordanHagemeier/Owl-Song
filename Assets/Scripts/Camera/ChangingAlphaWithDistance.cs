using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingAlphaWithDistance : MonoBehaviour
{
    [SerializeField] private Camera         m_Camera;
    [SerializeField] private Vector3        m_TreetopPosition;
    [SerializeField] private Renderer       m_Renderer;
    [SerializeField] private float          m_MinDistanceTillDither;
    [SerializeField] private float          m_MaxDistanceTillDither;

    [SerializeField] private float m_TreetopHeightInPercentage; 
    // Start is called before the first frame update
    void Start()
    {
        m_Camera            = Singletons.gameStateManager.camera;
        m_Renderer          = gameObject.GetComponent<Renderer>();
        m_TreetopPosition   = gameObject.transform.position;

        m_TreetopPosition.y = (gameObject.GetComponent<MeshFilter>().mesh.bounds.size.y * gameObject.transform.localScale.y) * m_TreetopHeightInPercentage;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
    }

    private void CheckDistance()
    {
       
        float distance = Vector3.Magnitude(m_Camera.transform.position - m_TreetopPosition);
        distance = Mathf.Clamp01(MathHelper.Remap(distance, m_MinDistanceTillDither, m_MaxDistanceTillDither, 0.0f, 1.0f));
       m_Renderer.material.SetFloat("_DistanceCamToObject", distance);
    }

    //private void OnDrawGizmos()
    //{
    //    DebugDrawingInterface.DrawPersistentPointBox(m_TreetopPosition, 1.0f, Color.red, 1000);
    //}
}
