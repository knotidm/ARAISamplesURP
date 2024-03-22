public static class Conversions
{
    private const float MILES_PER_SECOND_TO_MILES_PER_HOUR = 2.23694f;
    private const float METERS_PER_SECOND_TO_KNOTS = 1.943846171789349f;

    private const float KILOMETERS_TO_MILES = 0.621371192f;
    private const float METERS_TO_FEETS = 3.280839895013123f;

    private static string CachedTimeResult { get; set; }
    private static int CachedTimeIntValue { get; set; }

    private static string TimeSeparator
    {
        get { return ":"; }
    }

    public static float MilesPerSecondToMilesPerHour(float value)
    {
        return value * MILES_PER_SECOND_TO_MILES_PER_HOUR;
    }

    public static float MilesPerHourToMilesPerSecond(float value)
    {
        return value / MILES_PER_SECOND_TO_MILES_PER_HOUR;
    }

    public static float ConvertMetersPerSecondToKnots(float value)
    {
        return value * METERS_PER_SECOND_TO_KNOTS;
    }

    public static float ConvertKnotsToMetersPerSecond(float value)
    {
        return value / METERS_PER_SECOND_TO_KNOTS;
    }

    public static float KilometersToMiles(float value)
    {
        return value * KILOMETERS_TO_MILES;
    }

    public static float MilesToKilometers(float value)
    {
        return value / KILOMETERS_TO_MILES;
    }

    public static float ConvertMetersToFeets(float value)
    {
        return value * METERS_TO_FEETS;
    }

    public static float ConvertFeetsToMeters(float value)
    {
        return value / METERS_TO_FEETS;
    }

    public static float ConvertNormalizedValueToPercentage(float value)
    {
        return value * 100.0f;
    }

    public static float ConvertMegabyteValueToGigabyte(float value)
    {
        return value / 1024.0f;
    }

    public static string ConvertSecondsValueToMinutesFormat(float seconds)
    {
        seconds = (int)seconds;
        CachedTimeResult = "";

        CachedTimeIntValue = (int)(seconds / 60.0f);
        CachedTimeResult += string.Format("{0:00}", CachedTimeIntValue);

        CachedTimeResult += TimeSeparator;

        CachedTimeIntValue = (int)(seconds % 60.0f);
        CachedTimeResult += string.Format("{0:00}", CachedTimeIntValue);

        return CachedTimeResult;
    }
}