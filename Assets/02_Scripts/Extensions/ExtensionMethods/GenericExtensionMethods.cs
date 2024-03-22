using System.Collections.Generic;
using UnityEngine;

public static class GenericExtensionMethods
{
    public static void MoveTo(this Rigidbody obj, Transform target)
    {
        obj.position = target.position;
        obj.rotation = target.rotation;
    }

    public static void ActivateGameObject(this Component component, bool state = true)
    {
        component?.gameObject.SetActiveOptimized(state);
    }

    public static void DeactivateGameObject(this Component component)
    {
        component?.gameObject.SetActiveOptimized(false);
    }

    public static void SetGameObjectState(this Component component, bool state)
    {
        component?.gameObject.SetActiveOptimized(state);
    }

    public static T GetComponentInParentExcludeSelf<T>(this Component component) where T : Component
    {
        if (component == null || component.transform.parent == null)
        {
            return default(T);
        }

        return component.transform.parent.GetComponentInParent<T>();
    }

    public static T[] GetComponentsInChildrenPath<T>(this Component target, int inPathOccurences) where T : Component
    {
        List<T> result = new List<T>();

        if (target == null || target.transform == null)
        {
            return result.ToArray();
        }

        T inObjectComponent = target.GetComponent<T>();

        if (inObjectComponent != null)
        {
            result.Add(inObjectComponent);
            inPathOccurences--;
        }

        if (inPathOccurences > 0)
        {
            for (int i = 0; i < target.transform.childCount; ++i)
            {
                Transform child = target.transform.GetChild(i);
                result.AddRange(GetComponentsInChildrenPath<T>(child, inPathOccurences));
            }
        }

        return result.ToArray();
    }

    public static void GetComponentsInChildrenPath<T>(this Component target, ref List<T> result, int inPathOccurences) where T : Component
    {
        if (target == null || target.transform == null)
        {
            return;
        }

        T inObjectComponent = target.GetComponent<T>();

        if (inObjectComponent != null)
        {
            result.Add(inObjectComponent);
            inPathOccurences--;
        }

        if (inPathOccurences > 0)
        {
            for (int i = 0; i < target.transform.childCount; ++i)
            {
                Transform child = target.transform.GetChild(i);

                GetComponentsInChildrenPath<T>(child, ref result, inPathOccurences);
            }
        }
    }

    public static Sprite ToSprite(this Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public static void DestroyClear<T>(this List<T> target, bool immediate = false) where T : Component
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target[i] == null)
            {
                continue;
            }

            if (immediate == true || (Application.isEditor == true && Application.isPlaying == false))
            {
                GameObject.DestroyImmediate(target[i].gameObject);
            }
            else
            {
                GameObject.Destroy(target[i].gameObject);
            }
        }

        target.Clear();
    }

    public static void SetGameObjectActiveAll<T>(this List<T> target, bool state) where T : Component
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target[i] != null)
            {
                target[i].gameObject.SetActiveOptimized(state);
            }
        }
    }

    public static void SetEnableOptimized<T>(this List<T> target) where T : Behaviour
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target != null && target[i].enabled == false)
            {
                target[i].enabled = true;
            }
        }
    }

    public static void SetDisableOptimized<T>(this List<T> target) where T : Behaviour
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target != null && target[i].enabled == true)
            {
                target[i].enabled = false;
            }
        }
    }

    public static bool ContainsElement<T>(this T[] target, T lookupObject) where T : UnityEngine.Object
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

    public static int IndexOf<T>(this T[] collection, T target) where T : Component
    {
        for (int i = 0; i < collection.Length; i++)
        {
            if (target == collection[i])
            {
                return i;
            }
        }

        return -1;
    }

    public static bool BuiltinContains<T>(this T[] collection, T value) where T : UnityEngine.Object
    {
        for (int i = 0; i < collection.Length; i++)
        {
            if (collection[i] == value)
            {
                return true;
            }
        }

        return false;
    }
}