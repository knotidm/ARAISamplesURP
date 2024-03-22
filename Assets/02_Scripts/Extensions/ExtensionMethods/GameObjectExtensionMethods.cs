using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensionMethods
{
    public static void ChangeLayerRecursively(this GameObject target, string layerName)
    {
        target.layer = LayerMask.NameToLayer(layerName);

        Transform[] children = target.GetComponentsInChildren<Transform>();

        for (int i = 0; i < children.Length; ++i)
        {
            children[i].gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    public static void ChangeLayerRecursively(this GameObject target, int layerIndex)
    {
        if (target == null)
        {
            return;
        }

        target.layer = layerIndex;

        Transform[] children = target.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < children.Length; ++i)
        {
            children[i].gameObject.layer = layerIndex;
        }
    }

    public static int GetLayerMaskBitValue(this GameObject target)
    {
        if (target == null)
        {
            return 0;
        }

        return (1 << target.layer);
    }

    public static bool IsObjectOnLayerMask(this GameObject target, LayerMask checkLayer)
    {
        if (target == null)
        {
            return false;
        }

        return (target.GetLayerMaskBitValue() & checkLayer) != 0;
    }

    public static void DestroyClear(this List<GameObject> target, bool immediate = false)
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target[i] == null)
            {
                continue;
            }

            if (immediate == true)
            {
                GameObject.DestroyImmediate(target[i]);
            }
            else
            {
                GameObject.Destroy(target[i]);
            }
        }

        target.Clear();
    }

    public static void SetActiveAll(this GameObject[] target, bool state)
    {
        for (int i = 0; i < target.Length; i++)
        {
            if (target[i] != null)
            {
                target[i].SetActiveOptimized(state);
            }
        }
    }

    public static void SetActiveAll(this List<GameObject> target, bool state)
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target[i] != null)
            {
                target[i].SetActiveOptimized(state);
            }
        }
    }

    public static void SetActiveOptimized(this GameObject target, bool state)
    {
        if (target != null && target.activeSelf != state)
        {
            target.SetActive(state);
        }
    }

    public static Bounds GetObjectBoundsFromRenderers(this GameObject target)
    {
        Bounds bounds = new Bounds(target.transform.position, Vector3.zero);
        MeshRenderer[] targetRenderers = target.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            bounds.Encapsulate(targetRenderers[i].bounds);
        }

        return bounds;
    }

    public static Bounds GetObjectBoundsFromColliders(this GameObject target)
    {
        Bounds bounds = new Bounds(target.transform.position, Vector3.zero);
        Collider[] targetColliders = target.GetComponentsInChildren<Collider>();

        for (int i = 0; i < targetColliders.Length; i++)
        {
            bounds.Encapsulate(targetColliders[i].bounds);
        }

        return bounds;
    }

    public static List<MeshRenderer> GetMeshRenderersByTag(this GameObject source, string targetTag)
    {
        List<MeshRenderer> foundMeshRenderers = new List<MeshRenderer>();

        if (source == null)
        {
            return foundMeshRenderers;
        }

        MeshRenderer[] sourceRenderers = source.GetComponentsInChildren<MeshRenderer>() as MeshRenderer[];

        // get mesh renderers by tag
        foreach (MeshRenderer airplaneMeshRenderer in sourceRenderers)
        {
            GameObject currentElement = airplaneMeshRenderer.gameObject;
            if (currentElement.tag != targetTag)
            {
                continue;
            }

            foundMeshRenderers.Add(airplaneMeshRenderer);
        }

        return foundMeshRenderers;
    }

    public static void ForceRefreshObject(this GameObject target)
    {
        target.SetActiveOptimized(false);
        target.SetActiveOptimized(true);
    }

}