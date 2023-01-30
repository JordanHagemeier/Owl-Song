using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawingManager : MonoBehaviour
{
    [SerializeField] Material   m_DebugRenderMaterialZTest = null;
    [SerializeField] Material   m_DebugRenderMaterialNoZTest = null;

    [SerializeField] private bool m_EnableDebugDrawings = true; public bool enableDebugDrawings { get { return m_EnableDebugDrawings; } }
  
    // Start is called before the first frame update
    void Start()
    {
        DebugDrawingInterface.Initialize(m_DebugRenderMaterialZTest, m_DebugRenderMaterialNoZTest);
        DebugDrawingRenderComponent.SetCullFunction(ShouldCullDebugDrawings);
       
    }

    bool ShouldCullDebugDrawings()
    {
        return false;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (m_EnableDebugDrawings)
        {
            DebugDrawingInterface.Tick();
        }
        else
        {
            DebugDrawingInterface.ClearAll();
        }
        
    }

    private void OnApplicationQuit()
    {
        DebugDrawingInterface.Uninitialize();
    }
}
