using System.Collections.Generic;
using UnityEngine;

public static class PrimitiveTypesExtensionMethods
{
    public static Vector3 Set(this Vector3 target, float valueX, float valueY, float valueZ)
    {
        Vector3 output = target;
        output.x = valueX;
        output.y = valueY;
        output.z = valueZ;

        return output;
    }

    public static Vector3 SetX(this Vector3 target, float value)
    {
        Vector3 output = target;
        output.x = value;

        return output;
    }

    public static Vector3 SetY(this Vector3 target, float value)
    {
        Vector3 output = target;
        output.y = value;

        return output;
    }

    public static Vector3 SetZ(this Vector3 target, float value)
    {
        Vector3 output = target;
        output.z = value;

        return output;
    }

    public static Vector2 Set(this Vector2 target, float valueX, float valueY)
    {
        Vector2 output = target;
        output.x = valueX;
        output.y = valueY;

        return output;
    }

    public static Vector2 SetX(this Vector2 target, float value)
    {
        Vector2 output = target;
        output.x = value;

        return output;
    }

    public static Vector2 SetY(this Vector2 target, float value)
    {
        Vector2 output = target;
        output.y = value;

        return output;
    }

    public static bool IsInRange(this int source, int min, int max)
    {
        return IsInRange((float)source, min, max);
    }

    public static bool IsInRange(this float source, float min, float max)
    {
        if (min > max)
        {
            return (source >= max && source <= min);
        }

        return (source >= min && source <= max);
    }

    public static bool IsInRange(this int source, Vector2 range)
    {
        return IsInRange((float)source, range);
    }

    public static bool IsInRange(this float source, Vector2 range)
    {
        if (range.x > range.y)
        {
            return (source >= range.y && source <= range.x);
        }

        return (source >= range.x && source <= range.y);
    }

    public static bool ContainsElement(this string[] target, string lookupObject)
    {
        for (int i = 0; i < target.Length; i++)
        {
            if (target[i] == lookupObject)
            {
                return true;
            }
        }

        return false;
    }

    public static bool ContainsElement(this List<string> target, string lookupObject)
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target[i] == lookupObject)
            {
                return true;
            }
        }

        return false;
    }

    public static bool ContainsAny(this string[] target, string[] lookupArray)
    {
        for (int targetIndex = 0; targetIndex < target.Length; targetIndex++)
        {
            for (int lookupIndex = 0; lookupIndex < lookupArray.Length; lookupIndex++)
            {
                if (target[targetIndex] == lookupArray[lookupIndex])
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool ContainsAny(this List<string> target, List<string> lookupArray)
    {
        for (int targetIndex = 0; targetIndex < target.Count; targetIndex++)
        {
            for (int lookupIndex = 0; lookupIndex < lookupArray.Count; lookupIndex++)
            {
                if (target[targetIndex] == lookupArray[lookupIndex])
                {
                    return true;
                }
            }
        }

        return false;
    }
}