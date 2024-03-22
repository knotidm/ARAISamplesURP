using System.IO;
using UnityEngine;

public static class FileDataLoader
{
    // <summary> Load a PNG or JPG file from disk to a Texture2D. Returns null if load fails. </summary>
    public static Texture2D LoadTextureFromDisk(string filePath)
    {
        Texture2D textureToLoad = null;
        byte[] fileData;

        if (File.Exists(filePath) == true)
        {
            fileData = File.ReadAllBytes(filePath);
            textureToLoad = new Texture2D(2, 2);

            if (textureToLoad.LoadImage(fileData) == false)
            {
                textureToLoad = null;
            }
        }

        return textureToLoad;
    }
}
