using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensionMethods
{
    public static void ResetLocal(this Transform target, Transform parent)
    {
        target.SetParent(parent);
        ResetLocal(target);
    }

    public static void ResetLocal(this Transform target)
    {
        target.SetLocal(Vector3.zero, Quaternion.identity, Vector3.one);
    }

    public static void SetLocal(this Transform target, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        target.localPosition = localPosition;
        target.localRotation = localRotation;
        target.localScale = localScale;
    }

    public static void ClearChildren(this Transform target, bool immediate = false, int clearFromIndex = 0)
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in target)
        {
            children.Add(child.gameObject);
        }

        for (int i = clearFromIndex; i < children.Count; i++)
        {
            if (immediate == true)
            {
                GameObject.DestroyImmediate(children[i]);
            }
            else
            {
                GameObject.Destroy(children[i]);
            }
        }
    }

    public static void SetChildrenActiveOptimized(this Transform target, bool activate)
    {
        foreach (Transform child in target)
        {
            child.gameObject.SetActiveOptimized(activate);
        }
    }

    public static void ForceRefreshObject(this Transform target)
    {
        GameObjectExtensionMethods.ForceRefreshObject(target.gameObject);
    }
}