using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LayerMask = System.Int16;

/*
 * Interface that allows you to create debug drawings in the form of
 * Texts, Lines & Polygons for both the editor as well as the game view.
 * Also supports drawings that persist for a number of ticks
 */

using GameTicks = System.Int16;

[System.Serializable]
public static class DebugDrawingInterface
{
	/*
	 * For future reference:
	 * https://answers.unity.com/questions/44848/how-to-draw-debug-text-into-scene.html
	 * https://github.com/jagt/unity3d-runtime-debug-draw/blob/master/Assets/RuntimeDebugDraw.cs
	 * 
	 * TODO: Create a material with a different zTest Property so that zTest can actually be used.
	 */
	 
	[SerializeField]
	private static GameObject   m_DrawingObject = null;

	[Header("Debug Manager - Debug Data")]
	public static Material      DebugMaterial;
	public static Material      DebugMaterialNoZTest;
		
	public class DebugText
	{
		public LayerMask    LayerMask_;

		public GUIContent   Text;
		public Vector3      Position; 
		public Color        TextColor;
		public int          TextSize;
		public GameTicks    RemainingTicks;
		public bool         ZTest;

		public bool         PositionIsWorldSpace;   
	}
	 
	public class DebugMesh
	{
		public Mesh         Mesh;
		public GameTicks    RemainingTicks;
		public bool         ZTest;
	}
	 
	public static List<DebugText> DebugTexts;
	public static List<DebugMesh> DebugMeshes;
	
	////////////////////////////////////////////////////////////////

	public static void Initialize(Material drawMaterial, Material drawMaterialNoZTest)
	{
		DebugMaterial         = drawMaterial;
		DebugMaterialNoZTest  = drawMaterialNoZTest;

		DebugTexts              = new List<DebugText>();
		DebugMeshes             = new List<DebugMesh>();
	}

	////////////////////////////////////////////////////////////////
	
	/*
	 * Lazy create the drawing object only if it is needed
	 */
	static void AssureDrawingObjectExists()
	{
		if (m_DrawingObject == null)
		{
			m_DrawingObject         = new GameObject("~ DebugDrawingRuntime");
			m_DrawingObject.AddComponent<DebugDrawingRenderComponent>();
		}
	}

	////////////////////////////////////////////////////////////////

	public static void Uninitialize()
	{
		if (m_DrawingObject != null)
		{
			GameObject.Destroy(m_DrawingObject);
		}
	}

    public static void ClearAll()
    {
        if(DebugTexts.Count > 0)
        {
            DebugTexts.Clear();
        }
       
        if(DebugMeshes.Count > 0)
        {
            DebugMeshes.Clear();
        }
       
    }
    ////////////////////////////////////////////////////////////////

    public static void Tick()
	{
		UpdateTicks();
	}
	
	////////////////////////////////////////////////////////////////
	
	static void UpdateTicks()
	{
		for (int i = 0; i < DebugTexts.Count; i++)
		{
			DebugTexts[i].RemainingTicks --;

			if (DebugTexts[i].RemainingTicks < 0)
			{
				DebugTexts.RemoveAt(i);
				i--;
			}
		}

		for (int i = 0; i < DebugMeshes.Count; i++)
		{
			DebugMeshes[i].RemainingTicks --;

			if (DebugMeshes[i].RemainingTicks < 0)
			{
				DebugMeshes.RemoveAt(i);
				i--;
			}
		}
	}

	////////////////////////////////////////////////////////////////
	// Text
	////////////////////////////////////////////////////////////////
	
	public static void DrawSSText(string text, Vector2 screenPosition, Color color, int textSize = 11, bool zTest = false )
	{
		DrawPersistentSSText(text, screenPosition, color, 0, textSize, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentSSText(string text, Vector2 screenPosition, Color color, GameTicks tickCount, int textSize = 11, bool zTest = false)
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugText debugText = new DebugText
		{
			Text                    = new GUIContent(text),
			TextColor               = color,
			TextSize                = textSize,
			RemainingTicks          = tickCount,
			ZTest                   = zTest,
			Position                = screenPosition,
			PositionIsWorldSpace    = false
		};

		DebugTexts.Add(debugText);
	}

	////////////////////////////////////////////////////////////////
	
	public static void DrawWSText(string text, Vector3 worldPosition, Color color, int textSize = 11, bool zTest = false )
	{
		DrawPersistentWSText(text, worldPosition, color, 0, textSize, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentWSText(string text, Vector3 worldPosition, Color color, GameTicks tickCount, int textSize = 11, bool zTest = false )
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugText debugText = new DebugText
        { 
			Text                    = new GUIContent(text),
			TextColor               = color,
			TextSize                = textSize,
			RemainingTicks          = tickCount,
			ZTest                   = zTest,
			Position                = worldPosition,
			PositionIsWorldSpace    = true
		};

		DebugTexts.Add(debugText);
	}
	
	////////////////////////////////////////////////////////////////
	// Line
	////////////////////////////////////////////////////////////////

	public static void DrawLine(Vector3 from, Vector3 to, Color color, bool zTest = false )
	{
		DrawPersistentLine(from, to, color, 0, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentLine(Vector3 from, Vector3 to, Color color, GameTicks tickCount, bool zTest = false )
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugMesh debugLine         = new DebugMesh();
		
		debugLine.RemainingTicks    = tickCount;
		debugLine.ZTest             = zTest;

		List<Vector3> linePositions = new List<Vector3>();
		linePositions.Add(from);
		linePositions.Add(to);

		////////////////////////////////////////////////////////////////
		
		List<int> lineIndices       = new List<int>();
		List<Color> lineColors      = new List<Color>();

		for (int i = 0; i < linePositions.Count; i++)
		{
			lineColors.Add(color);
			lineIndices.Add(i);
		}

		////////////////////////////////////////////////////////////////
		
		Mesh mesh = new Mesh();
		mesh.SetVertices(linePositions);
		mesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
		mesh.SetColors(lineColors);
		debugLine.Mesh  = mesh;

		////////////////////////////////////////////////////////////////

		DebugMeshes.Add(debugLine);
	}

	////////////////////////////////////////////////////////////////
   
	// Line Alts
	public static void DrawRay(Vector3 from, Vector3 direction, float length, Color color, bool zTest = false)
	{
		DrawLine(from, from + direction * length, color, zTest);
	}

	public static void DrawRay(Vector3 from, Vector3 direction, Color color, bool zTest = false)
	{
		DrawLine(from, from + direction, color, zTest);
	}

	public static void DrawRay(Ray ray, float length, Color color, bool zTest = false)
	{
		DrawLine(ray.origin, ray.origin + ray.direction * length, color, zTest);
	}

	public static void DrawPersistentRay(Vector3 from, Vector3 direction, float length, Color color, GameTicks tickCount, bool zTest = false)
	{
		DrawPersistentLine(from, from + direction * length, color, tickCount, zTest);
	}

	public static void DrawPersistentRay(Vector3 from, Vector3 direction, Color color, GameTicks tickCount, bool zTest = false)
	{
		DrawPersistentLine(from, from + direction, color, tickCount, zTest);
	}

	public static void DrawPersistentRay(Ray ray, float length, Color color, GameTicks tickCount, bool zTest = false)
	{
		DrawPersistentLine(ray.origin, ray.origin + ray.direction * length, color, tickCount, zTest);
	}
	
	////////////////////////////////////////////////////////////////

	public static void DrawLines(List<Vector3> verts, Color color, bool zTest = false)
	{
		DrawPersistentLines(verts, color, 0, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentLines(List<Vector3> verts, Color color, GameTicks tickCount, bool zTest = false)
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugMesh debugLine         = new DebugMesh();
		debugLine.RemainingTicks    = tickCount;
		debugLine.ZTest             = zTest;
		
		////////////////////////////////////////////////////////////////
		
		List<int> lineIndices       = new List<int>();
		List<Color> lineColors      = new List<Color>();

		for (int i = 0; i < verts.Count; i++)
		{
			lineColors.Add(color);
			lineIndices.Add(i);
		}

		////////////////////////////////////////////////////////////////
		
		Mesh mesh = new Mesh();
		mesh.SetVertices(verts);
		mesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
		mesh.SetColors(lineColors);
		debugLine.Mesh  = mesh;

		////////////////////////////////////////////////////////////////

		DebugMeshes.Add(debugLine);
	}
	////////////////////////////////////////////////////////////////

	public static void DrawXYRect(Vector3 min, Vector3 size, Color color, bool zTest = false)
	{
		DrawPersistentXYRect(min, size, color, 0, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentXYRect(Vector3 min, Vector3 size, Color color, GameTicks tickCount, bool zTest = false)
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugMesh debugLine         = new DebugMesh();
		debugLine.RemainingTicks    = tickCount;
		debugLine.ZTest             = zTest;
		
		List<Vector3> linePositions = new List<Vector3>();
		linePositions.Add(min);
		linePositions.Add(min + new Vector3(size.x, 0, 0));
		linePositions.Add(min + new Vector3(size.x, 0, 0));
		linePositions.Add(min + new Vector3(size.x, size.y, 0));
		linePositions.Add(min + new Vector3(size.x, size.y, 0));
		linePositions.Add(min + new Vector3(0, size.y, 0));
		linePositions.Add(min + new Vector3(0, size.y, 0));
		linePositions.Add(min);

		////////////////////////////////////////////////////////////////
		
		List<int> lineIndices       = new List<int>();
		List<Color> lineColors      = new List<Color>();

		for (int i = 0; i < linePositions.Count; i++)
		{
			lineColors.Add(color);
			lineIndices.Add(i);
		}

		////////////////////////////////////////////////////////////////
		
		Mesh mesh = new Mesh();
		mesh.SetVertices(linePositions);
		mesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
		mesh.SetColors(lineColors);
		debugLine.Mesh  = mesh;

		////////////////////////////////////////////////////////////////

		DebugMeshes.Add(debugLine);
	}

	//////////////////////////////////////////////////////////////////

	public static void DrawArrow(Vector3 from, Vector3 to, Color color, float arrowHeadLength = 0.5f, float arrowHeadAngleDeg = 40.0f, bool zTest = false)
	{
		DrawPersistentArrow(from, to, color, 0, arrowHeadLength, arrowHeadAngleDeg, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentArrow(Vector3 from, Vector3 to, Color color, GameTicks tickCount, float arrowHeadLength, float arrowHeadAngleDeg, bool zTest = false)
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugMesh debugLine         = new DebugMesh();
		debugLine.RemainingTicks    = tickCount;
		debugLine.ZTest             = zTest;

		List<Vector3> linePositions = new List<Vector3>();

		////////////////////////////////////////////////////////////////

		// Body
		linePositions.Add(from);
		linePositions.Add(to);

		// Arrow Head
		Vector3 direction  = (to - from).normalized * arrowHeadLength;
		Vector3 normal     = Vector3.Cross(direction, Vector3.up);
		Vector3 direction1 = Quaternion.AngleAxis(-arrowHeadAngleDeg, Vector3.up)   * direction;
		Vector3 direction2 = Quaternion.AngleAxis(-arrowHeadAngleDeg, normal)       * direction;
		Vector3 direction3 = Quaternion.AngleAxis(arrowHeadAngleDeg, Vector3.up)    * direction;
		Vector3 direction4 = Quaternion.AngleAxis(arrowHeadAngleDeg, normal)        * direction;
		
		linePositions.Add(to);
		linePositions.Add(to - direction1);

		linePositions.Add(to);
		linePositions.Add(to - direction2);

		linePositions.Add(to);
		linePositions.Add(to - direction3);

		linePositions.Add(to);
		linePositions.Add(to - direction4);
		
		// Connections
		linePositions.Add(to - direction1);
		linePositions.Add(to - direction2);
		
		linePositions.Add(to - direction2);
		linePositions.Add(to - direction3);
		
		linePositions.Add(to - direction3);
		linePositions.Add(to - direction4);

		linePositions.Add(to - direction4);
		linePositions.Add(to - direction1);

		////////////////////////////////////////////////////////////////
				
		List<int> lineIndices       = new List<int>();
		List<Color> lineColors      = new List<Color>();

		for (int i = 0; i < linePositions.Count; i++)
		{
			lineColors.Add(color);
			lineIndices.Add(i);
		}

		////////////////////////////////////////////////////////////////
		
		Mesh mesh = new Mesh();
		mesh.SetVertices(linePositions);
		mesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
		mesh.SetColors(lineColors);
		debugLine.Mesh  = mesh;

		////////////////////////////////////////////////////////////////

		DebugMeshes.Add(debugLine);
	}

	//////////////////////////////////////////////////////////////////

	public static void DrawPointBox(Vector3 point, float radius, Color color, bool zTest = false)
	{
		DrawPersistentPointBox(point, radius, color, 0, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentPointBox(Vector3 point, float radius, Color color, GameTicks tickCount, bool zTest = false)
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugMesh debugLine         = new DebugMesh();
		debugLine.RemainingTicks    = tickCount;
		debugLine.ZTest             = zTest;

		List<Vector3> linePositions = new List<Vector3>();

		////////////////////////////////////////////////////////////////

		// Box

		float P = radius / 2.0f;
		float N = - radius / 2.0f;
		
		// Upper Rect
		linePositions.Add(point + new Vector3(P, P, P));
		linePositions.Add(point + new Vector3(N, P, P));
		
		linePositions.Add(point + new Vector3(P, P, P));
		linePositions.Add(point + new Vector3(P, P, N));
		
		linePositions.Add(point + new Vector3(N, P, N));
		linePositions.Add(point + new Vector3(N, P, P));
		
		linePositions.Add(point + new Vector3(N, P, N));
		linePositions.Add(point + new Vector3(P, P, N));
		
		// Lower Rect
		linePositions.Add(point + new Vector3(P, N, P));
		linePositions.Add(point + new Vector3(N, N, P));
		
		linePositions.Add(point + new Vector3(P, N, P));
		linePositions.Add(point + new Vector3(P, N, N));
		
		linePositions.Add(point + new Vector3(N, N, N));
		linePositions.Add(point + new Vector3(N, N, P));
		
		linePositions.Add(point + new Vector3(N, N, N));
		linePositions.Add(point + new Vector3(P, N, N));

		// Stripes between
		linePositions.Add(point + new Vector3(P, P, P));
		linePositions.Add(point + new Vector3(P, N, P));
		
		linePositions.Add(point + new Vector3(N, P, P));
		linePositions.Add(point + new Vector3(N, N, P));
		
		linePositions.Add(point + new Vector3(P, P, N));
		linePositions.Add(point + new Vector3(P, N, N));
		
		linePositions.Add(point + new Vector3(N, P, N));
		linePositions.Add(point + new Vector3(N, N, N));
		
		////////////////////////////////////////////////////////////////
				
		List<int> lineIndices       = new List<int>();
		List<Color> lineColors      = new List<Color>();

		for (int i = 0; i < linePositions.Count; i++)
		{
			lineColors.Add(color);
			lineIndices.Add(i);
		}

		////////////////////////////////////////////////////////////////
		
		Mesh mesh = new Mesh();
		mesh.SetVertices(linePositions);
		mesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
		mesh.SetColors(lineColors);
		debugLine.Mesh  = mesh;

		////////////////////////////////////////////////////////////////

		DebugMeshes.Add(debugLine);
	}

	////////////////////////////////////////////////////////////////
	// Polygons
	////////////////////////////////////////////////////////////////
	
	public static void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Color color, bool zTest = false )
	{
		DrawPersistentTriangle(a, b, c, color, 0, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentTriangle(Vector3 a, Vector3 b, Vector3 c, Color color, GameTicks tickCount, bool zTest = false )
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugMesh debugMesh         = new DebugMesh();
		debugMesh.RemainingTicks    = tickCount;
		debugMesh.ZTest             = zTest;

		List<Vector3> vertices      = new List<Vector3>();
		vertices.Add(a);
		vertices.Add(b);
		vertices.Add(c);

		////////////////////////////////////////////////////////////////
		
		List<int> indices       = new List<int>();
		List<Color> colors      = new List<Color>();
		
		for (int i = 0; i < vertices.Count; i++)
		{
			colors.Add(color);
			indices.Add(i);
		}

		////////////////////////////////////////////////////////////////
		
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
		mesh.SetColors(colors);

		debugMesh.Mesh = mesh;

		////////////////////////////////////////////////////////////////

		DebugMeshes.Add(debugMesh);
	}
	
	public static void DrawTriangle(Vector3 from, Vector3 to, float width, Color color, bool zTest = false)
	{
		DrawPersistentTriangle(from, to, width , color, 0, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentTriangle(Vector3 from, Vector3 to, float width, Color color, GameTicks tickCount, bool zTest = false )
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugMesh debugMesh         = new DebugMesh();
		debugMesh.RemainingTicks    = tickCount;
		debugMesh.ZTest             = zTest;

		List<Vector3> vertices      = new List<Vector3>();
		Vector3 directionN          = (to - from).normalized;
		Vector3 normal              = Vector3.Cross(directionN, Vector3.up);

		vertices.Add(from - normal * width / 2.0f);
		vertices.Add(from + normal * width / 2.0f);
		vertices.Add(to);

		////////////////////////////////////////////////////////////////
		
		List<int> indices       = new List<int>();
		List<Color> colors      = new List<Color>();
		
		for (int i = 0; i < vertices.Count; i++)
		{
			colors.Add(color);
			indices.Add(i);
		}

		////////////////////////////////////////////////////////////////
		
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
		mesh.SetColors(colors);

		debugMesh.Mesh = mesh;

		////////////////////////////////////////////////////////////////

		DebugMeshes.Add(debugMesh);
	}

	////////////////////////////////////////////////////////////////
	
	public static void DrawQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color, bool zTest = false )
	{
		DrawPersistentQuad(a, b, c, d, color, 0, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color, GameTicks tickCount, bool zTest = false )
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugMesh debugMesh         = new DebugMesh();
		debugMesh.RemainingTicks    = tickCount;
		debugMesh.ZTest             = zTest;

		List<Vector3> vertices      = new List<Vector3>();
		vertices.Add(a);
		vertices.Add(b);
		vertices.Add(c);
		vertices.Add(a);
		vertices.Add(c);
		vertices.Add(d);

		////////////////////////////////////////////////////////////////
		
		List<int> indices       = new List<int>();
		List<Color> colors      = new List<Color>();
		
		for (int i = 0; i < vertices.Count; i++)
		{
			colors.Add(color);
			indices.Add(i);
		}

		////////////////////////////////////////////////////////////////
		
		Mesh mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
		mesh.SetColors(colors);

		debugMesh.Mesh = mesh;

		////////////////////////////////////////////////////////////////

		DebugMeshes.Add(debugMesh);
    }

    ////////////////////////////////////////////////////////////////

    public static void DrawCircleXY(Vector3 center, float radius, Color color, bool zTest = false)
    {
        DrawPersistentCircleXY(center, radius, color, 0, zTest);
    }

    ////////////////////////////////////////////////////////////////

    public static void DrawPersistentCircleXY(Vector3 center, float radius, Color color, GameTicks tickCount, bool zTest = false)
    {
        AssureDrawingObjectExists();

        ////////////////////////////////////////////////////////////////

        DebugMesh debugMesh = new DebugMesh();
        debugMesh.RemainingTicks = tickCount;
        debugMesh.ZTest = zTest;

        List<Vector3> vertices = new List<Vector3>();
        const int VERTS = 24;
        for (int i = 0; i < VERTS; i++)
        {
            float angleRad = 2 * Mathf.PI / (VERTS - 1) * i;

            Vector3 position = center + new Vector3(Mathf.Sin(angleRad), Mathf.Cos(angleRad), center.z) * radius;
            vertices.Add(position);
        }

        ////////////////////////////////////////////////////////////////

        List<int> indices = new List<int>();
        List<Color> colors = new List<Color>();

        for (int i = 0; i < vertices.Count - 1; i++)
        {
            colors.Add(color);
            indices.Add((i) % (vertices.Count - 1));
            indices.Add((i + 1) % (vertices.Count - 1));
        }

        colors.Add(color);

        ////////////////////////////////////////////////////////////////

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        mesh.SetColors(colors);

        debugMesh.Mesh = mesh;

        ////////////////////////////////////////////////////////////////

        DebugMeshes.Add(debugMesh);
    }

    ////////////////////////////////////////////////////////////////

    public static void DrawCircleXZ(Vector3 center, float radius, Color color, bool zTest = false)
    {
        DrawPersistentCircleXZ(center, radius, color, 0, zTest);
    }

    ////////////////////////////////////////////////////////////////

    public static void DrawPersistentCircleXZ(Vector3 center, float radius, Color color, GameTicks tickCount, bool zTest = false)
    {
        AssureDrawingObjectExists();

        ////////////////////////////////////////////////////////////////

        DebugMesh debugMesh = new DebugMesh();
        debugMesh.RemainingTicks = tickCount;
        debugMesh.ZTest = zTest;

        List<Vector3> vertices = new List<Vector3>();
        const int VERTS = 24;
        for (int i = 0; i < VERTS; i++)
        {
            float angleRad = 2 * Mathf.PI / (VERTS - 1) * i;

            Vector3 position = center + new Vector3(Mathf.Sin(angleRad), 0.0f, Mathf.Cos(angleRad)) * radius;
            vertices.Add(position);
        }

        ////////////////////////////////////////////////////////////////

        List<int> indices = new List<int>();
        List<Color> colors = new List<Color>();

        for (int i = 0; i < vertices.Count - 1; i++)
        {
            colors.Add(color);
            indices.Add((i) % (vertices.Count - 1));
            indices.Add((i + 1) % (vertices.Count - 1));
        }

        colors.Add(color);

        ////////////////////////////////////////////////////////////////

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        mesh.SetColors(colors);

        debugMesh.Mesh = mesh;

        ////////////////////////////////////////////////////////////////

        DebugMeshes.Add(debugMesh);
    }

    ////////////////////////////////////////////////////////////////

    public static void DrawCircleXYFilled(Vector3 center, float radius, Color color, bool zTest = false)
    {
        DrawPersistentCircleXYFilled(center, radius, color, 0, zTest);
    }

    ////////////////////////////////////////////////////////////////

    public static void DrawPersistentCircleXYFilled(Vector3 center, float radius, Color color, GameTicks tickCount, bool zTest = false)
    {
        AssureDrawingObjectExists();

        ////////////////////////////////////////////////////////////////

        DebugMesh debugMesh = new DebugMesh();
        debugMesh.RemainingTicks = tickCount;
        debugMesh.ZTest = zTest;

        List<Vector3> vertices = new List<Vector3>();

        vertices.Add(center);
        const int RIM_VERTS = 24;
        for (int i = 0; i < RIM_VERTS; i++)
        {
            float angleRad = 2 * Mathf.PI / (RIM_VERTS - 1) * i;

            Vector3 position = center + new Vector3(Mathf.Sin(angleRad), Mathf.Cos(angleRad), 0) * radius;
            vertices.Add(position);
        }

        ////////////////////////////////////////////////////////////////

        List<int> indices = new List<int>();
        List<Color> colors = new List<Color>();

        for (int i = 0; i < RIM_VERTS; i++)
        {
            colors.Add(color);
            indices.Add(0);
            indices.Add(1 + (i) % (RIM_VERTS));
            indices.Add(1 + (i + 1) % (RIM_VERTS));
        }

        colors.Add(color);

        ////////////////////////////////////////////////////////////////

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        //mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        mesh.SetTriangles(indices, 0);
        mesh.SetColors(colors);

        debugMesh.Mesh = mesh;

        ////////////////////////////////////////////////////////////////

        DebugMeshes.Add(debugMesh);
    }

    ////////////////////////////////////////////////////////////////

    public static void DrawCircleXZFilled(Vector3 center, float radius, Color color, bool zTest = false)
    {
        DrawPersistentCircleXZFilled(center, radius, color, 0, zTest);
    }

    ////////////////////////////////////////////////////////////////

    public static void DrawPersistentCircleXZFilled(Vector3 center, float radius, Color color, GameTicks tickCount, bool zTest = false)
    {
        AssureDrawingObjectExists();

        ////////////////////////////////////////////////////////////////

        DebugMesh debugMesh = new DebugMesh();
        debugMesh.RemainingTicks = tickCount;
        debugMesh.ZTest = zTest;

        List<Vector3> vertices = new List<Vector3>();

        vertices.Add(center);
        const int RIM_VERTS = 24;
        for (int i = 0; i < RIM_VERTS; i++)
        {
            float angleRad = 2 * Mathf.PI / (RIM_VERTS - 1) * i;

            Vector3 position = center + new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad)) * radius;
            vertices.Add(position);
        }

        ////////////////////////////////////////////////////////////////

        List<int> indices = new List<int>();
        List<Color> colors = new List<Color>();

        for (int i = 0; i < RIM_VERTS; i++)
        {
            colors.Add(color);
            indices.Add(0);
            indices.Add(1 + (i) % (RIM_VERTS));
            indices.Add(1 + (i + 1) % (RIM_VERTS));
        }

        colors.Add(color);

        ////////////////////////////////////////////////////////////////

        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        mesh.SetColors(colors);

        debugMesh.Mesh = mesh;

        ////////////////////////////////////////////////////////////////

        DebugMeshes.Add(debugMesh);
    }

    ////////////////////////////////////////////////////////////////

    public static void DrawVerts(List<Vector3> verts, Color color, bool zTest = false )
	{
		DrawPersistentVerts(verts, color, 0, zTest);
	}

	////////////////////////////////////////////////////////////////

	public static void DrawPersistentVerts(List<Vector3> verts, Color color, GameTicks tickCount, bool zTest = false )
	{
		AssureDrawingObjectExists();

		////////////////////////////////////////////////////////////////

		DebugMesh debugMesh         = new DebugMesh();
		debugMesh.RemainingTicks    = tickCount;
		debugMesh.ZTest             = zTest;

		////////////////////////////////////////////////////////////////
		
		List<int> indices       = new List<int>();
		List<Color> colors      = new List<Color>();
		List<Vector2> uvs       = new List<Vector2>();
		
		for (int i = 0; i < verts.Count; i++)
		{
			colors.Add(color);
			indices.Add(i);
			uvs.Add(verts[i].xz());
		}

		////////////////////////////////////////////////////////////////
		
		Mesh mesh = new Mesh();
		mesh.SetVertices(verts);
		mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
		mesh.SetUVs(0, uvs);
		mesh.SetColors(colors);

		debugMesh.Mesh = mesh;

		////////////////////////////////////////////////////////////////

		DebugMeshes.Add(debugMesh);
	}

}