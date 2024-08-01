using System;
using Unity.Sentis;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    public GameObject screen;
    public Text predictionText;
    public GameObject[] lights;
    public GameObject[] clue;

    // code pad where the digit is drawn onto
    private Texture2D drawableTexture;
    private const int imageWidth = 28; //width and height of input image
    private byte[] zeroes = new byte[imageWidth * imageWidth * 3]; // blank screen
    private Vector3 lastCoord; // last position of mouse on screen

    // digit recognition
    private int predictedIndex;
    private float probability;
    private float timeOfLastEntry = float.MaxValue;
    private float clearTime = 2f; // time digit is on screen before it is cleared

    public Action<Door, int, float> callback;

    private void Start()
    {
        // code pad texture which will be drawn into:
        drawableTexture = new Texture2D(imageWidth, imageWidth, TextureFormat.RGB24, false);
        drawableTexture.wrapMode = TextureWrapMode.Clamp;
        drawableTexture.filterMode = FilterMode.Point;

        ClearTexture();

        // emission map for glowing digits
        screen.GetComponent<Renderer>().material.SetTexture("_EmissionMap", drawableTexture);
    }

    private void ClearTexture()
    {
        drawableTexture.LoadRawTextureData(zeroes);
        drawableTexture.Apply();
        predictionText.text = "?";
    }

    // Calls the neural network to get the probabilities of different digits then selects the most likely
    private void Infer()
    {
        (float, int) probabilityAndPredictedIndex = MNISTEngine.Instance.GetResults(drawableTexture);

        probability = probabilityAndPredictedIndex.Item1;
        predictedIndex = probabilityAndPredictedIndex.Item2;

        char predictedResult = (char)MNISTEngine.Instance._map[predictedIndex];

        predictionText.text = predictedResult.ToString();
    }

    // Draws a line on the panel by simply drawing a sequence of pixels
    private void DrawLine(Vector3 startp, Vector3 endp)
    {
        int steps = (int)((endp - startp).magnitude * 2 + 1);
        for (float a = 0; a <= steps; a++)
        {
            float t = a * 1f / steps;
            DrawPoint(startp * (1 - t) + endp * t, 2, Color.white);
        }
    }

    // Draws either a single pixel or a 2x2 pixel for a thicker line
    private void DrawPoint(Vector3 coord, int thickness, Color color)
    {
        //clamp the values so it doesn't touch the border
        float x = Mathf.Clamp(coord.x, thickness, imageWidth - thickness);
        float y = Mathf.Clamp(coord.y, thickness, imageWidth - thickness);

        switch (thickness)
        {
            case 1:
                DrawPixel((int)x, (int)y, color);
                break;
            case 2:
            default:
                int x0 = Mathf.Max(0, (int)(x - 0.5f));
                int x1 = Mathf.Min(imageWidth - 1, (int)(x + 0.5f));
                int y0 = Mathf.Max(0, (int)(y - 0.5f));
                int y1 = Mathf.Min(imageWidth - 1, (int)(y + 0.5f));
                DrawPixel(x0, y0, color);
                DrawPixel(x1, y0, color);
                DrawPixel(x0, y1, color);
                DrawPixel(x1, y1, color);
                break;
        }
    }

    private void DrawPixel(int x, int y, Color color)
    {
        drawableTexture.SetPixel(x, y, color);
    }

    public void ScreenMouseDown(RaycastHit hit)
    {
        Vector2 uv = hit.textureCoord;
        Vector3 coords = uv * imageWidth;
        lastCoord = coords;
        timeOfLastEntry = Time.time;
    }

    public void ScreenGetMouse(RaycastHit hit)
    {
        Vector2 uv = hit.textureCoord;
        Vector3 coords = uv * imageWidth;

        DrawLine(lastCoord, coords);
        lastCoord = coords;
        drawableTexture.Apply();

        timeOfLastEntry = Time.time;
        // Run the inference every frame since it is very fast
        Infer();
    }

    private void Update()
    {
        // After a certain time we want to clear the panel:
        if ((Time.time - timeOfLastEntry) > clearTime)
        {
            callback?.Invoke(Door.Instance, predictedIndex, probability);
            ClearTexture();
            timeOfLastEntry = float.MaxValue;
        }
    }
}
