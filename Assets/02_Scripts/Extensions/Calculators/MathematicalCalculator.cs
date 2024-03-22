using UnityEngine;

public static class MathematicalCalculator
{
    private static System.Random CurrentSystemRandom { get; set; }

    static MathematicalCalculator()
    {
        CurrentSystemRandom = new System.Random();
    }

    /// <summary> Returns a random integer number between min [inclusive] and max [exclusive] </summary>
    public static int UnityRandom(int min, int max)
    {
        return Random.Range(min, max);
    }

    /// <summary> Returns a random float number between and min [inclusive] and max [exclusive] </summary>
    public static float UnityRandom(float min, float max)
    {
        return Random.Range(min, max);
    }

    /// <summary> Returns a random float number between and min [inclusive] and max [exclusive] </summary>
    public static float UnityRandom(Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }

    /// <summary> Returns a random integer number between min [inclusive] and max [inclusive] </summary>
    public static int SystemRandom(int min, int max)
    {
        return CurrentSystemRandom.Next(min, max);
    }

    /// <summary> Returns a random float number between and min [inclusive] and max [exclusive] </summary>
    public static float SystemRandom(float min, float max)
    {
        float result = (float)CurrentSystemRandom.NextDouble();
        result *= (max - min);
        result += min;

        return result;
    }

    /// <summary> Returns a random float number between and min [inclusive] and max [exclusive] </summary>
    public static float SystemRandom(Vector2 range)
    {
        return SystemRandom(range.x, range.y);
    }
}