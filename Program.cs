using System.CommandLine;
using System.Globalization;
using System.IO;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

var inputOption = new Option<string>("--input")
{
    IsRequired = true,
    Description = "Specifies the path of the Audacity label .txt file.",

};

inputOption.AddAlias("-i");

var outputOption = new Option<string>("--output")
{
    Description = "Specifies the path of the SubRip subtitle .srt file."
};
outputOption.AddAlias("-o");

var minimumDurationOption = new Option<uint>("--minduration")
{
    Description = "Specifies the minimum duration for each subtitle in milliseconds."
};
minimumDurationOption.AddAlias("-md");
minimumDurationOption.SetDefaultValue(0);

var rootCommand = new RootCommand();
rootCommand.Add(inputOption);
rootCommand.Add(outputOption);
rootCommand.Add(minimumDurationOption);

rootCommand.SetHandler((input, output, minDuration) =>
{
    // Test input path
    if (!File.Exists(input))
    {
        Console.Error.WriteLine("The specified input file is not found");
        return;
    }

    // If the output is empty, derive it from the input
    if (string.IsNullOrEmpty(output))
    {
        var inputDirectory = Path.GetDirectoryName(input);
        var inputFileName = Path.GetFileNameWithoutExtension(input);
        output = Path.Join(inputDirectory, $"{inputFileName}.srt");
    }

    // Read all lines from the input
    var labelLines = File.ReadAllLines(input);

    int i = 0;

    // Try to write the output

    try
    {
        using (var outputFileStr = File.OpenWrite(output))
        {
            using (var outputWriter = new StreamWriter(outputFileStr))
            {
                for (i = 0; i < labelLines.Length; i++)
                {
                    var currentLine = labelLines[i];
                    var subtitleObj = Subtitle.FromAudacityLabel(currentLine);

                    if (minDuration > 0)
                    {
                        if (subtitleObj.EnsureDuration(TimeSpan.FromMilliseconds(minDuration)))
                        {
                            Console.WriteLine($"Prolonged subtitle @ line {i + 1}");
                        }
                    }

                    // Write index
                    outputWriter.WriteLine(i + 1);

                    // Write timestamp
                    outputWriter.WriteLine(subtitleObj.GetSubRipTimeString());

                    // Write text with extra line
                    outputWriter.WriteLine($"{subtitleObj.Text}\n");
                }
            }
        }
    }
    catch (Exception ex)
    {
#if DEBUG
        throw ex;
#else
        Console.Error.WriteLine($"Failed to write output @ line {i} ({ex.Message})");
#endif
    }
}, inputOption, outputOption, minimumDurationOption);

rootCommand.Invoke(args);