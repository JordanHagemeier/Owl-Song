using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


///////////////////////////////////////////////////////////////////////////

public class DebugDrawHelper
{
    const float DEFAULT_DEBUG_Z_OFFSET = -0.001f;
    const float DEFAULT_DEBUG_Y_OFFSET = 0.001f;
    static float GetDebugOffset_Plane(Mode2D mode2D) { return (mode2D == Mode2D.OnYPlane) ? DEFAULT_DEBUG_Y_OFFSET : DEFAULT_DEBUG_Z_OFFSET; }

    ///////////////////////////////////////////////////////////////////////////

    public static void DrawPoint(Mode2D mode2D, Vector2 pos2D, Color color, string text = null)
    {
        if (!Application.isEditor)
        {
            return;
        }

        const float CROSS_SIZE = 0.2f;
        Vector2 offset1_2D = new Vector2(1, 1) * CROSS_SIZE;
        Vector2 offset2_2D = new Vector2(1, -1) * CROSS_SIZE;

        float planeCoordinate = GetDebugOffset_Plane(mode2D);

        Vector3 offset1 = offset1_2D.To3D(mode2D, planeCoordinate);
        Vector3 offset2 = offset2_2D.To3D(mode2D, planeCoordinate);
        Vector3 pos3D = pos2D.To3D(mode2D, planeCoordinate);

        DrawLine(pos3D - offset1, pos3D + offset1, color);
        DrawLine(pos3D - offset2, pos3D + offset2, color);

        if (!string.IsNullOrEmpty(text))
        {
            DrawText(mode2D, text, pos2D, color, false);
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    static Vector3[] s_TmpLinePoints;

    public static void DrawLine(Vector3 pos, Vector3 pos2, Color color, float width)
    {
        if (!Application.isEditor)
        {
            return;
        }

#if UNITY_EDITOR
        if (s_TmpLinePoints == null)
        {
            s_TmpLinePoints = new Vector3[2];
        }

        s_TmpLinePoints[0] = pos;
        s_TmpLinePoints[1] = pos2;

        Handles.color = color;
        Handles.DrawAAPolyLine(width, s_TmpLinePoints);
#else
			Debug.DrawLine(pos, pos2, color);
#endif
    }

    ///////////////////////////////////////////////////////////////////////////
    ///
    public static void DrawLine(Vector3 pos, Vector3 pos2, Color color)
    {
        if (!Application.isEditor)
        {
            return;
        }

#if UNITY_EDITOR
        if (s_TmpLinePoints == null)
        {
            s_TmpLinePoints = new Vector3[2];
        }

        s_TmpLinePoints[0] = pos;
        s_TmpLinePoints[1] = pos2;

        Handles.color = color;
        Handles.DrawAAPolyLine(1.0f, s_TmpLinePoints);
#else
			Debug.DrawLine(pos, pos2, color);
#endif
    }

    ///////////////////////////////////////////////////////////////////////////

    public static void DrawLine(Mode2D mode2D, Vector2 pos, Vector2 pos2, Color color)
    {
        Vector3 pos_3D = pos.To3D(mode2D, GetDebugOffset_Plane(mode2D));
        Vector3 pos2_3D = pos2.To3D(mode2D, GetDebugOffset_Plane(mode2D));

        DrawLine(pos_3D, pos2_3D, color);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static void DrawText(Mode2D mode2D, string text, Vector2 pos, Color color, bool bold = false)
    {
#if UNITY_EDITOR

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = color;
        if (bold)
        {
            labelStyle.fontStyle = FontStyle.Bold;
        }

        Vector3 pos3D = pos.To3D(mode2D, GetDebugOffset_Plane(mode2D));

        Handles.Label(pos.To3D(-0.1f), text, labelStyle);
#endif
    }

    ///////////////////////////////////////////////////////////////////////////


    public static void DrawRect(Mode2D mode2D, Rect rect, Color color, bool fillRect, string text = null)
    {
        DrawRect(mode2D, rect.min, rect.max, color, fillRect, text);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static void DrawRect(Mode2D mode2D, Vector2 posMin, Vector2 posMax, Color color, bool fillRect, string text = null, float planeCoordinate = 0)
    {
        if (posMin.x > posMax.x || posMin.y > posMax.y)
        {
            // Warn if rect is degenerated
            color = Color.Lerp(new Color(1, 0, 0, 0.8f), new Color(0.2f, 0, 0, 0.8f), 0.5f + 0.5f * Mathf.Sin(Time.realtimeSinceStartup * 5.0f));
        }

        Vector2 size = posMax - posMin;

        Vector2 pos0 = posMin + new Vector2(0.0f, 0.0f);
        Vector2 pos1 = posMin + new Vector2(size.x, 0.0f);
        Vector2 pos2 = posMin + new Vector2(size.x, size.y);
        Vector2 pos3 = posMin + new Vector2(0.0f, size.y);

        DrawPolygon(mode2D, pos0, pos1, pos2, pos3, color, fillRect, planeCoordinate);

        if (!string.IsNullOrEmpty(text))
        {
            DebugDrawHelper.DrawText(mode2D, text, pos0, color.GetWithModifiedAlpha(1.0f));
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public static void DrawTriangle(Mode2D mode2D, Vector2 pos0, Vector2 pos1, Vector2 pos2, Color color, bool fill, float z = 0)
    {
        DrawPolygon(mode2D, pos0, pos1, pos2, null, color, fill, z);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static void DrawArrow(Mode2D mode2D, Vector2 from, Vector2 to, Color color, bool fill, float z = 0)
    {
        float relativeWidth = 0.4f;

        Vector2 direction = to - from;

        Vector2 orthogonalCCW = new Vector2(-direction.y, direction.x);

        Vector2 p0 = to;
        Vector2 p1 = from - (orthogonalCCW * relativeWidth);
        Vector2 p2 = from + (orthogonalCCW * relativeWidth);

        DrawTriangle(mode2D, p0, p1, p2, color, fill, z);
    }

    ///////////////////////////////////////////////////////////////////////////

    static Material s_DebugDrawMaterial = null;
    public static Material GetDebugDrawMaterial()
    {
        if (!s_DebugDrawMaterial)
        {
            string debugShader = "UI/Unlit/Detail";

            Shader unlitShader = Shader.Find(debugShader);
            if (!unlitShader)
            {
                Debug.LogWarning("Debug Shader not found");
                return null;
            }
            s_DebugDrawMaterial = new Material(unlitShader);
        }

        return s_DebugDrawMaterial;
    }

    ///////////////////////////////////////////////////////////////////////////

    private static void DrawPolygon(Mode2D mode2D, Vector2 pos0_2D, Vector2 pos1_2D, Vector2 pos2_2D, Vector2? pos3_2D, Color color, bool fillRect, float planeCoordinate)
    {
        planeCoordinate += GetDebugOffset_Plane(mode2D);

        Vector3 pos0 = pos0_2D.To3D(mode2D, planeCoordinate);
        Vector3 pos1 = pos1_2D.To3D(mode2D, planeCoordinate);
        Vector3 pos2 = pos2_2D.To3D(mode2D, planeCoordinate);
        Vector3? pos3_ = pos3_2D.HasValue ? pos3_2D.Value.To3D(mode2D, planeCoordinate) : (Vector3?)null;

        DrawPolygon(pos0, pos1, pos2, pos3_, color, fillRect);
    }

    ///////////////////////////////////////////////////////////////////////////

    static List<Vector2> s_CirclePointsNorm = null;
    static List<Vector3> s_CirclePointsWS_Tmp = null;

    public static void DrawCircle(Mode2D mode2D, Vector2 center2D, float axisOffset, float radius2D, Color color, bool fill, string text = null)
    {
        Vector3 center3D = center2D.To3D(mode2D, axisOffset + GetDebugOffset_Plane(mode2D));

        
        if (s_CirclePointsNorm == null)
        {
            int CIRCLE_POINT_COUNT = 25;
            s_CirclePointsNorm = new List<Vector2>();

            for (int i = 0; i < CIRCLE_POINT_COUNT; ++i)
            {
                float relativeAngle = i / (float)(CIRCLE_POINT_COUNT - 1);
                float angle = relativeAngle * Mathf.PI * 2.0f;

                float x = Mathf.Sin(angle);
                float y = Mathf.Cos(angle);

                s_CirclePointsNorm.Add(new Vector2(x, y));
            }
        }

        if (s_CirclePointsWS_Tmp == null)
        {
            s_CirclePointsWS_Tmp = new List<Vector3>();
        }

        // TODO: Create Triangle-Fan-Mesh instead of on-the-fly-geometry

        s_CirclePointsWS_Tmp.Clear();

        for (int i = 0; i < s_CirclePointsNorm.Count; ++i)
        {
            Vector2 pos = center2D + s_CirclePointsNorm[i] * radius2D;
            s_CirclePointsWS_Tmp.Add(pos.To3D(mode2D, axisOffset + GetDebugOffset_Plane(mode2D)));
        }

        DrawTriangleFan(center3D, s_CirclePointsWS_Tmp, color, fill);

        if (!string.IsNullOrEmpty(text))
        {
            Vector2 CircleTextPos = center2D;
            CircleTextPos.y -= radius2D;

            DebugDrawHelper.DrawText(mode2D, text, CircleTextPos, color.GetWithModifiedAlpha(1.0f));
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public static void DrawPolygon(Vector3 pos0, Vector3 pos1, Vector3 pos2, Vector3? pos3, Color color, bool fill)
    {
        if (!Camera.current)
        {
            return;
        }

        if (fill)
        {
            Material debugMaterial = GetDebugDrawMaterial();
            if (!debugMaterial)
            {
                Debug.LogWarning("No Debug Material found");
                return;
            }

            debugMaterial.SetPass(0);

            GL.PushMatrix();
            GL.LoadProjectionMatrix(Camera.current.projectionMatrix);
            GL.modelview = Camera.current.worldToCameraMatrix;

            GL.Begin(pos3.HasValue ? GL.QUADS : GL.TRIANGLES);

            GL.Color(color);
            GL.Vertex3(pos0.x, pos0.y, pos0.z);
            GL.Vertex3(pos1.x, pos1.y, pos1.z);
            GL.Vertex3(pos2.x, pos2.y, pos2.z);
            if (pos3.HasValue)
            {
                GL.Vertex3(pos3.Value.x, pos3.Value.y, pos3.Value.z);
            }
            GL.End();

            GL.PopMatrix();
        }
        else
        {
            DebugDrawHelper.DrawLine(pos0, pos1, color);
            DebugDrawHelper.DrawLine(pos1, pos2, color);

            if (pos3.HasValue)
            {
                DebugDrawHelper.DrawLine(pos2, pos3.Value, color);
                DebugDrawHelper.DrawLine(pos3.Value, pos0, color);
            }
            else
            {
                DebugDrawHelper.DrawLine(pos2, pos0, color);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public static void DrawTriangleFan(Vector3 p0, List<Vector3> points, Color color, bool fill)
    {
        if (!Camera.current)
        {
            return;
        }

        if (fill)
        {
            Material debugMaterial = GetDebugDrawMaterial();
            if (!debugMaterial)
            {
                Debug.LogWarning("No Debug Material found");
                return;
            }

            debugMaterial.SetPass(0);

            GL.PushMatrix();
            GL.LoadProjectionMatrix(Camera.current.projectionMatrix);
            GL.modelview = Camera.current.worldToCameraMatrix;

            GL.Begin(GL.TRIANGLE_STRIP);

            GL.Color(color);

            GL.Vertex3(p0.x, p0.y, p0.z);
            bool lastWasCenter = true;
            foreach (Vector3 point in points)
            {
                GL.Vertex3(point.x, point.y, point.z);
                if (lastWasCenter)
                {
                    lastWasCenter = false;
                }
                else
                {
                    GL.Vertex3(point.x, point.y, point.z);
                    GL.Vertex3(p0.x, p0.y, p0.z);
                    lastWasCenter = true;
                }
            }

            GL.End();

            GL.PopMatrix();
        }
        else
        {
            for (int i = 1; i < points.Count; ++i)
            {
                Vector3 pA = points[i - 1];
                Vector3 pB = points[i];

                DebugDrawHelper.DrawLine(pA, pB, color);
            }

            /*if (points.Length > 1)
			{
				Debug.DrawLine(points[points.Length - 1], points[0], color);
			}*/
        }
    }

    ///////////////////////////////////////////////////////////////////////////

}

///////////////////////////////////////////////////////////////////////////

