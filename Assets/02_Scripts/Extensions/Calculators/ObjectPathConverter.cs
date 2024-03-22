using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ObjectPathConverter
{
    private static List<string> StringPartToRemoveFromResourcePath
    {
        get
        {
            return new List<string> { "Assets/Resources/", "Assets/StreamingAssets" };
        }
    }

    public static string GetObjectRelativePathWithExtension(Object objectToGetPath)
    {
        string relativePath = GetObjectFullPath(objectToGetPath);
        int cachedIndex = 0;

        foreach (string partToRemove in StringPartToRemoveFromResourcePath)
        {
            cachedIndex = relativePath.IndexOf(partToRemove);

            if (cachedIndex >= 0)
            {
                relativePath = relativePath.Remove(cachedIndex, partToRemove.Length);
            }
        }

        return relativePath;
    }

    public static string GetObjectRelativePathWithoutExtension(Object objectToGetPath)
    {
        string relativePath = GetObjectRelativePathWithExtension(objectToGetPath);
        int cachedIndex = relativePath.LastIndexOf('.');

        if (cachedIndex != -1)
        {
            relativePath = relativePath.Remove(cachedIndex);
        }

        return relativePath;
    }

    public static string GetObjectFullPath(Object objectToGetPath)
    {
#if UNITY_EDITOR
        return AssetDatabase.GetAssetPath(objectToGetPath);
#else
		return null;
#endif
    }
}
