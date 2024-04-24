using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public static class CustomExtensionMethods
{
    private static List<float> NormalizedProbabilitiesCollecion { get; set; } = new List<float>();

    public static T GetRandomElement<T>(this List<T> collection)
    {
        if ((collection == null) || (collection.Count <= 0))
        {
            return default;
        }

        int index = MathematicalCalculator.UnityRandom(0, collection.Count);

        return collection[index];
    }

    public static T GetRandomElement<T>(this T[] collection)
    {
        if ((collection == null) || (collection.Length <= 0))
        {
            return default;
        }

        int index = MathematicalCalculator.UnityRandom(0, collection.Length);

        return collection[index];
    }

    public static int GetRandomElementIndex<T>(this List<T> collection)
    {
        if ((collection == null) || (collection.Count <= 0))
        {
            return -1;
        }

        return MathematicalCalculator.UnityRandom(0, collection.Count);
    }

    public static int GetRandomElementIndex<T>(this T[] collection)
    {
        if ((collection == null) || (collection.Length <= 0))
        {
            return -1;
        }

        return MathematicalCalculator.UnityRandom(0, collection.Length);
    }

    public static T GetFirstElement<T>(this List<T> target) where T : class
    {
        if (target == null || target.Count <= 0)
        {
            return default;
        }

        return target[0];
    }

    public static T GetLastElement<T>(this List<T> collection)
    {
        if (collection == null || collection.Count <= 0)
        {
            return default;
        }

        return collection[^1];
    }

    public static int GetLastElementIndex<T>(this List<T> collection)
    {
        if (collection == null || collection.Count <= 0)
        {
            return -1;
        }

        return collection.Count - 1;
    }

    public static int GetLastElementIndex<T>(this T[] collection)
    {
        if (collection == null || collection.Length <= 0)
        {
            return -1;
        }

        return collection.Length - 1;
    }

    public static bool IsLastElementIndex<T>(this List<T> collection, int index)
    {
        return (collection != null) && index == collection.Count - 1;
    }

    public static T GetFirstElement<T>(this T[] collection)
    {
        if (collection == null || collection.Length <= 0)
        {
            return default;
        }

        return collection[0];
    }
    public static T GetLastElement<T>(this T[] collection)
    {
        if (collection == null || collection.Length <= 0)
        {
            return default;
        }

        return collection[^1];
    }

    public static bool IsEmpty<T>(this List<T> collection)
    {
        return collection == null || collection.Count <= 0;
    }

    public static void RemoveNullElements<T>(this List<T> target)
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target[i] == null)
            {
                target.RemoveAt(i);
                i--;
            }
        }
    }

    public static T GetRandomElementWithProbabiltity<T>(this List<T> collection) where T : IProbability
    {
        float totalProbability = 0.0f;
        float nextProbabilityStep = 0.0f;
        float newRandomValue = MathematicalCalculator.SystemRandom(0.0f, 1.0f);

        NormalizedProbabilitiesCollecion.Clear();

        for (int i = 0; i < collection.Count; i++)
        {
            totalProbability += (collection[i].Probability > 0.0f) ? collection[i].Probability : 0.0f;
        }

        for (int i = 0; i < collection.Count; i++)
        {
            nextProbabilityStep += collection[i].Probability;
            NormalizedProbabilitiesCollecion.Add(nextProbabilityStep / totalProbability);
        }

        for (int i = 0; i < NormalizedProbabilitiesCollecion.Count; i++)
        {
            if (newRandomValue < NormalizedProbabilitiesCollecion[i])
            {
                return collection[i];
            }
        }

        return default;
    }

    public static string[] GetElementNamesCollection<T>(this List<T> collection) where T : IName
    {
        string[] names = new string[collection.Count];

        for (int i = 0; i < names.Length; i++)
        {
            names[i] = collection[i].Name;
        }

        return names;
    }

    public static int GetElementIndexByName<T>(this List<T> collection, string name) where T : IName
    {
        for (int i = 0; i < collection.Count; i++)
        {
            if (collection[i].Name == name)
            {
                return i;
            }
        }

        return -1;
    }

    public static T GetElementByName<T>(this List<T> collection, string name) where T : IName
    {
        int index = collection.GetElementIndexByName(name);
        return (index != -1) ? collection[index] : default;
    }

    private static System.Random random = new System.Random();

    public static void ShuffleList<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    public static string AddOrdinal(this int number)
    {
        if (number <= 0)
        {
            return number.ToString();
        }

        switch (number % 100)
        {
            case 11:
            case 12:
            case 13:
                return $"{number}th";
        }

        switch (number % 10)
        {
            case 1:
                return $"{number}st";
            case 2:
                return $"{number}nd";
            case 3:
                return $"{number}rd";
            default:
                return $"{number}th";
        }
    }

    public static string ToProperString(this string value)
    {
        if (value == null)
        {
            return null;
        }

        if (value.Length < 2)
        {
            return value.ToUpper();
        }

        string result = value.Substring(0, 1).ToUpper();

        for (int i = 1; i < value.Length; i++)
        {
            if (char.IsUpper(value[i]))
            {
                result += " ";
            }

            result += value[i];
        }

        return result;
    }

    public static void CopyToClipboard(this string value)
    {
        GUIUtility.systemCopyBuffer = value;
    }

    private const string orderedListStartTag = "<ol>";
    private const string orderedListEndTag = "</ol>";
    private const string unorderedListStartTag = "<ul>";
    private const string unorderedListEndTag = "</ul>";
    private const string listElementStartTag = "<li>";
    private const string listElementEndTag = "</li>";

    /*
    <p>This is test without format</p><p><br></p><p><strong>This is a bold test</strong></p><p><br></p><p><em>This is italic test</em></p><p><br></p><p>This is underlined</p><p><br></p><p><span style="color: rgb(0, 102, 204);">This is blue color</span></p><p><br></p><ol><li>This is list with number</li><li>This is list with number 2</li></ol><p><br></p><ul><li>This is bullet list</li><li>This is bullet list 2</li></ul><p><br></p>
    */
    public static string HTMLListToTextMeshProRichText(this string text)
    {
        text = text.Replace("<p>", " ");
        text = text.Replace("</p>", " ");

        int orderedListStartIndex = text.IndexOf(orderedListStartTag);
        int unorderedListStartIndex = text.IndexOf(unorderedListStartTag);
        int orderedListEndIndex = text.IndexOf(orderedListEndTag) + orderedListEndTag.Length;
        int unorderedListEndIndex = text.IndexOf(unorderedListEndTag) + unorderedListEndTag.Length;
        string orderedHTMLListText = "";
        string unorderedHTMLListText = "";

        if (orderedListStartIndex != -1)
        {
            int orderedListLength = orderedListEndIndex - orderedListStartIndex;
            orderedHTMLListText = text.Substring(orderedListStartIndex, orderedListLength);
        }

        if (unorderedListStartIndex != -1)
        {
            int unorderedListLength = unorderedListEndIndex - unorderedListStartIndex;
            unorderedHTMLListText = text.Substring(unorderedListStartIndex, unorderedListLength);
        }

        string orderedListText = orderedHTMLListText.Replace(orderedListStartTag, "").Replace(orderedListEndTag, "")
            .Replace(listElementStartTag, "~").Replace(listElementEndTag, "");

        char[] array = orderedListText.ToCharArray();
        string orderedList = "";

        for (int i = 0; i < array.Length; i++)
        {
            char character = array[i];
            string orderedList1 = "";

            if (character == '~')
            {
                string[] splited = orderedListText.Split('~');
                for (int i1 = 1; i1 < splited.Length; i1++)
                {
                    string item = splited[i1];
                    string itemNew = "\n " + i1 + ". " + item;
                    orderedList1 += itemNew;
                }
                orderedList = orderedList1;
            }
        }

        string unorderedListText = unorderedHTMLListText.Replace(unorderedListStartTag, "").Replace(unorderedListEndTag, "")
            .Replace(listElementStartTag, "\n - ").Replace(listElementEndTag, "");

        if (!string.IsNullOrEmpty(orderedHTMLListText))
        {
            text = text.Replace(orderedHTMLListText, orderedList);
        }

        if (!string.IsNullOrEmpty(unorderedHTMLListText))
        {
            text = text.Replace(unorderedHTMLListText, unorderedListText);
        }

        return text;
    }

    public static Vector2 SizeToParent(this RawImage image, float padding = 0)
    {
        var parent = image.transform.parent.GetComponentInParent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();

        if (!parent)
        {
            return imageTransform.sizeDelta;
        }

        padding = 1 - padding;
        float ratio = image.texture.width / (float)image.texture.height;
        var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);

        if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
        {
            bounds.size = new Vector2(bounds.height, bounds.width);
        }

        float height = bounds.height * padding;
        float width = height * ratio;

        if (width > bounds.width * padding)
        {
            width = bounds.width * padding;
            height = width / ratio;
        }

        imageTransform.sizeDelta = new Vector2(width, height);
        return imageTransform.sizeDelta;
    }

    public static RectTransform ApplySafeArea(this Rect rect, out Rect lastSafeArea, RectTransform panel)
    {
        lastSafeArea = rect;

        Vector2 anchorMin = Vector2.zero;
        Vector2 anchorMax = Vector2.zero;

        // TODO: Some research and fixes here (works great but code is ugly)
#if UNITY_IOS
        // Because iOS interprets safe area differently than Android and adds a extra margin under notch
        // iOS also cut area because of rounded corners (Android doesn't do that)

        if (rect.y > 0.0f) // If is notch present
        {
            // Move higher to eliminate extra margin (About 21 points from research)
            anchorMax = new Vector2(Screen.width, Screen.height - rect.y + 21.0f);
        }
        else
        {
            // Whole screen if there is no notch because iOS cut rounded corners area by default (Android doesn't do that)
            anchorMax = new Vector2(Screen.width, Screen.height);
        }
#elif UNITY_ANDROID
        anchorMin = rect.position;
        anchorMax = rect.position + rect.size;
#endif

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;

        return panel;
    }

    public static IEnumerator PlayVideoOnImage(this VideoPlayer videoPlayer, RawImage image, bool isFullScreenOn)
    {
        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);

        while (!videoPlayer.isPrepared)
        {
            yield return waitForSeconds;
            break;
        }
        image.gameObject.SetActive(true);
        image.texture = videoPlayer.texture;

        if (image.texture != null && isFullScreenOn)
        {
            image.SizeToParent();
        }

        videoPlayer.Play();
    }

    public static Texture2D GetCameraRenderAsTexture(this Camera camera)
    {
        RenderTexture.active = camera.targetTexture;
        camera.Render();

        Texture2D texture = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        texture.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        texture.Apply();

        return texture;
    }
}