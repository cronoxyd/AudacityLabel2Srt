using System.Globalization;

class Subtitle
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
    public string Text { get; set; }

    public Subtitle(TimeSpan start, TimeSpan end, string text)
    {
        StartTime = start;
        EndTime = end;
        Text = text;
    }

    public static Subtitle FromAudacityLabel(string labelLine)
    {
        var subtitleText = "";
        var tokens = labelLine.Split("\t");
        var startTime = TimeSpan.FromSeconds(double.Parse(tokens[0].Replace(",", "."), CultureInfo.InvariantCulture));
        var endTime = TimeSpan.FromSeconds(double.Parse(tokens[1].Replace(",", "."), CultureInfo.InvariantCulture));

        // FIXME: This doesn't allow for tabs inside the subtitle text
        if (tokens.Length == 3)
        {
            if (tokens[2].IndexOf("+") == 0)
            {
                subtitleText = tokens[2].Substring(1, tokens[2].Length - 1);
            }
            else
            {
                subtitleText = tokens[2];
            }
        }

        return new(startTime, endTime, subtitleText);
    }

    public string GetSubRipTimeString() =>
        $"{StartTime.ToString(@"hh\:mm\:ss\,fff")} --> {EndTime.ToString(@"hh\:mm\:ss\,fff")}";

    /// <summary>
    /// Compares the current <see cref="Duration"/> to the specified <paramref name="minDuration"/> and modifies the <see cref="EndTime"/> if necessary.
    /// </summary>
    /// <param name="minDuration">Specifies the minimum allowed duration.</param>
    /// <returns><see langword="true"/> if the <see cref="EndTime"/> was modified, <see langword="false"/> otherwise.</returns>
    public bool EnsureDuration(TimeSpan minDuration)
    {
        if (Duration < minDuration)
        {
            EndTime = StartTime + minDuration;
            return true;
        }
        else
        {
            return false;
        }
    }
}