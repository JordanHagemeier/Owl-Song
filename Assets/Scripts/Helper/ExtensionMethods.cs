using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ExtensionMethods-Helper to allow for syntax vector3.xz();

public enum Mode2D
{
    OnYPlane,
    OnZPlane,
}

///////////////////////////////////////////////////////////////////////////

public static class VectorExtensions
{
    ///////////////////////////////////////////////////////////////////////////

    public static bool AlmostEquals(this float f1, float f2, float epsilon = 0.001f)
    {
        return Mathf.Abs(f1 - f2) < epsilon;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static bool AlmostEquals(this Vector2 f1, Vector2 f2, float epsilon = 0.001f)
    {
        return AlmostEquals(f1.x, f2.x, epsilon) && AlmostEquals(f1.y, f2.y, epsilon);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector2 XZ(this Vector3 vec3D)
    {
        return new Vector2(vec3D.x, vec3D.z);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector2 XY(this Vector3 vec3D)
    {
        return new Vector2(vec3D.x, vec3D.y);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector3 To3D(this Vector2 vec2D, float y)
    {
        return new Vector3(vec2D.x, y, vec2D.y);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector2Int ToInt(this Vector2 vec2D)
    {
        return new Vector2Int((int)vec2D.x, (int)vec2D.y);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector3 ToVector3(this float f)
    {
        return new Vector3(f, f, f);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector2 ToVector2(this float f)
    {
        return new Vector2(f, f);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector2 ClampToInside(this Rect rect, Vector2 pos)
    {
        pos.x = Mathf.Max(rect.xMin, pos.x);
        pos.y = Mathf.Max(rect.yMin, pos.y);
        pos.x = Mathf.Min(rect.xMax, pos.x);
        pos.y = Mathf.Min(rect.yMax, pos.y);

        return pos;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        return new Vector2(
         v.x * cos - v.y * sin,
         v.x * sin + v.y * cos);
     
        
    }


    ///////////////////////////////////////////////////////////////////////////

    public static Rect GetUnion(this Rect rect, Rect other)
    {
        Rect outRect = new Rect();
        outRect.min = Vector2.Min(rect.min, other.min);
        outRect.max = Vector2.Max(rect.max, other.max);

        return outRect;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Rect GetIntersection(this Rect rect, Rect other)
    {
        Rect outRect = new Rect();
        outRect.min = Vector2.Max(rect.min, other.min);
        outRect.max = Vector2.Min(rect.max, other.max);

        return outRect;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static List<V> GetOrCreateList<K, V>(this Dictionary<K, List<V>> dict, K key)
    {
        List<V> list;
        if (dict.TryGetValue(key, out list))
        {
            return list;
        }

        list = new List<V>();
        dict.Add(key, list);

        return list;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static bool RemoveFromList_DeleteWhenEmpty<K, V>(this Dictionary<K, List<V>> dict, K key, V value)
    {
        List<V> list;
        if (!dict.TryGetValue(key, out list))
        {
            Debug.LogWarning("Could not remove " + key.ToString_IfNotNull() + ", " + value.ToString_IfNotNull());
            return false;
        }

        bool removed = list.Remove(value);

        if (list.Count == 0)
        {
            dict.Remove(key);
        }

        return removed;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static string ToString_IfNotNull<T>(this T obj)
    {
        if (obj == null)
        {
            return "[null]";
        }

        return obj.ToString();
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector3 To3D(this Vector2 pos2D, Mode2D mode2D, float planeCoordinate)
    {
        if (mode2D == Mode2D.OnYPlane)
        {
            return new Vector3(pos2D.x, planeCoordinate, pos2D.y);
        }
        else
        {
            return new Vector3(pos2D.x, pos2D.y, planeCoordinate);
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static string AddBrackets(this string str)
    {
        return "[" + str + "]";
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector2 MoveTowards(this Vector2 start, Vector2 target, float distance)
    {
        Vector2 delta = target - start;

        if (distance * distance > delta.sqrMagnitude)
        {
            return target;
        }

        return start + delta.normalized * distance;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector2 MoveDirection(this Vector2 start, Vector2 target, float distance)
    {
        Vector2 delta = target - start;

        return delta.normalized * distance;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static bool IsAsciiLetter(this char ch)
    {
        return ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'));
    }

    ///////////////////////////////////////////////////////////////////////////

    public static float Remap(this float value, float oldRangeMin, float oldRangeMax, float newRangeMin, float newRangeMax, bool clamp)
    {
        float newValue = (value - oldRangeMin) / (oldRangeMax - oldRangeMin) * (newRangeMax - newRangeMin) + newRangeMin;

        if (clamp)
        {
            newValue = Mathf.Clamp(newValue, newRangeMin, newRangeMax);
        }

        return newValue;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static T CallGetComponent_InEditor_IfNecessary<T>(this Behaviour behaviour, ref T cachedValue) where T : Component
    {
        if (!cachedValue)
        {
            if (!Application.isPlaying)
            {
                cachedValue = behaviour.GetComponent<T>();
            }
        }

        return cachedValue;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static T CallGetComponent_IfNecessary<T>(this Behaviour behaviour, ref T cachedValue, bool skipNullCheck) where T : Component
    {
        if (!skipNullCheck && !cachedValue)
        {
            cachedValue = behaviour.GetComponent<T>();
        }

        return cachedValue;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static ref T ConditionalRef<T>(this bool condition, ref T onTrue, ref T onFalse)
    {
        if (condition)
        {
            return ref onTrue;
        }
        else
        {
            return ref onFalse;
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Vector2 GetSwapped(this Vector2 vecOld, bool doSwap)
    {
        return doSwap ? new Vector2(vecOld.y, vecOld.x) : vecOld;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static bool Equals_IgnoreCase(this string value, string other)
    {
        return value.Equals(other, System.StringComparison.InvariantCultureIgnoreCase);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static Color GetWithModifiedAlpha(this Color col, float a)
    {
        return new Color(col.r, col.g, col.b, a);
    }

    ///////////////////////////////////////////////////////////////////////////

    public static string FlattenToString<T>(this List<T> list)
    {
        if (list == null)
        {
            return "[null]";
        }

        string outStr = "{";

        for (int i = 0; i < list.Count; ++i)
        {
            outStr += (list[i] != null) ? list[i].ToString() : "[null]";

            if (i != list.Count - 1)
            {
                outStr += ", ";
            }

            outStr += "}";
        }

        return outStr;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static void Resize<T>(this List<T> list, int size, T c)
    {
        int cur = list.Count;
        if (size < cur)
        {
            list.RemoveRange(size, cur - size);
        }
        else if (size > cur)
        {
            if (size > list.Capacity)
            {
                list.Capacity = size;
            }

            list.AddRange(Enumerable.Repeat(c, size - cur));
        }
    }

    ///////////////////////////////////////////////////////////////////////////

    public static void Resize<T>(this List<T> list, int cize) where T : new()
    {
        Resize(list, cize, new T());
    }

    ///////////////////////////////////////////////////////////////////////////

    public static void SwapAndPop_Unsafe<T>(this List<T> list, ref int indexToRemoveAndDecrement)
    {
        list[indexToRemoveAndDecrement] = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        --indexToRemoveAndDecrement;
    }

    ///////////////////////////////////////////////////////////////////////////

    public static T Back<T>(this T[] arr)
    {
        return arr[arr.Length - 1];
    }

    ///////////////////////////////////////////////////////////////////////////

    public static T Back<T>(this List<T> lst)
    {
        return lst[lst.Count - 1];
    }

    ///////////////////////////////////////////////////////////////////////////

   

    ///////////////////////////////////////////////////////////////////////////

    public static string AddNewlineIfNotEmpty(this string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            str += "\n";
        }

        return str;
    }

    ///////////////////////////////////////////////////////////////////////////

    
    ///////////////////////////////////////////////////////////////////////////
}