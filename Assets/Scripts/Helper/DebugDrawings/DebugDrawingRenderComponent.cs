using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DebugDrawingInterface;

// TODO: Find own solution or accept this workaround 
// vvvvvvvvvvvvvvvvvvvvvvvvvvvvv

using LayerMask = System.Int16;
public static class MathExtensions
{
    public static Vector2 xz(this Vector3 vector3D)
    {
        return new Vector2(vector3D.x, vector3D.z);
    }

    ////////////////////////////////////////////////////////////////

    public static Vector2 xy(this Vector3 vector3D)
    {
        return new Vector2(vector3D.x, vector3D.y);
    }
}

// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

public class DebugDrawingRenderComponent : MonoBehaviour
{
    [SerializeField]
    GUIStyle m_TextStyle = null;
    [SerializeField]
    Material m_zTestMaterial = null;
    [SerializeField]
    Material m_NoZTestMaterial = null;

    public delegate bool CullFunction();
    private static CullFunction m_CullFunction;
    public static void SetCullFunction(CullFunction cullFunction)
    {
        m_CullFunction = cullFunction;
    }


    private void Start()
    {
        m_TextStyle = new GUIStyle();
        m_TextStyle.alignment = TextAnchor.UpperCenter;

        m_zTestMaterial = DebugDrawingInterface.DebugMaterial;
        m_NoZTestMaterial = DebugDrawingInterface.DebugMaterialNoZTest;
    }

    ////////////////////////////////////////////////////////////////

    private void OnGUI()
    {
        ////////////////////////////////////////////////////////////////
        // Texts
        List<DebugText> debugTexts = DebugDrawingInterface.DebugTexts;

        for (int i = 0; i < debugTexts.Count; i++)
        {
            DebugText debugText = debugTexts[i];

            ////////////////////////////////////////////////////////////////

            if (m_CullFunction())
            {
                continue;
            }

            ////////////////////////////////////////////////////////////////

            Vector2 drawPosition = debugText.Position.xy();
            if (debugText.PositionIsWorldSpace)
            {
                Vector2 viewPortPosition = Camera.main.WorldToScreenPoint(debugText.Position).xy();
                drawPosition = new Vector2(viewPortPosition.x, Screen.height - viewPortPosition.y);
            }

            m_TextStyle.normal.textColor = debugText.TextColor;
            m_TextStyle.fontSize = debugText.TextSize;

            Rect rect = new Rect(drawPosition, m_TextStyle.CalcSize(debugText.Text));
            GUI.Label(rect, debugText.Text, m_TextStyle);
        }
    }

    ////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (Camera.current == null)
        {
            return;
        }

        UnityEditor.Handles.BeginGUI();

        ////////////////////////////////////////////////////////////////
        // Texts

        List<DebugText> debugTexts = DebugDrawingInterface.DebugTexts;
        
        if (debugTexts == null)
        {
            
            UnityEditor.Handles.EndGUI();
            return;
        }
        for (int i = 0; i < debugTexts.Count; i++)
        {
            DebugText debugText = debugTexts[i];

            ////////////////////////////////////////////////////////////////

            if (m_CullFunction())
            {
                continue;
            }

            ////////////////////////////////////////////////////////////////

            Vector2 drawPosition = debugText.Position.xy();
            if (debugText.PositionIsWorldSpace)
            {
                Vector2 viewPortPosition = Camera.current.WorldToScreenPoint(debugText.Position).xy();
                drawPosition = new Vector2(viewPortPosition.x, Screen.height - viewPortPosition.y);
            }

            m_TextStyle.normal.textColor = debugText.TextColor;
            m_TextStyle.fontSize = debugText.TextSize;

            Rect rect = new Rect(drawPosition, m_TextStyle.CalcSize(debugText.Text));
            GUI.Label(rect, debugText.Text, m_TextStyle);
        }

        UnityEditor.Handles.EndGUI();
    }

#endif

    ////////////////////////////////////////////////////////////////

    private const int UI_LAYER = 4;

    private void LateUpdate()
    {
        ////////////////////////////////////////////////////////////////
        // Lines & Polygons
        List<DebugMesh> debugLines = DebugDrawingInterface.DebugMeshes;

        for (int i = 0; i < debugLines.Count; i++)
        {
            DebugMesh debugLine = debugLines[i];

            ////////////////////////////////////////////////////////////////

            if (m_CullFunction())
            {
                continue;
            }

            ////////////////////////////////////////////////////////////////

            Graphics.DrawMesh(debugLine.Mesh, Vector3.zero, Quaternion.identity,
                                debugLine.ZTest ? m_zTestMaterial : m_NoZTestMaterial,
                                layer: UI_LAYER, camera: null, submeshIndex: 0, properties: null,
                                castShadows: false, receiveShadows: false);
        }
    }


    
}
