# AudacityLabel2Srt
Command-line utility that converts Audacity label files to the SubRip file format.

```text
AudacityLabel2Srt [options]

Options:
  -i, --input <input> (REQUIRED)    Specifies the path of the Audacity label .txt file.
  -o, --output <output>             Specifies the path of the SubRip subtitle .srt file.
  -md, --minduration <minduration>  Specifies the minimum duration for each subtitle in milliseconds. [default: 0]
  --version                         Show version information
  -?, -h, --help                    Show help and usage information
```

## Examples
```text
AudacityLabel2Srt -i labels.txt -o subtitles.srt -md 2000
```

Converts the labels exported from Audacity (`labels.txt`) to the SubRip format and
stores the subtitles in `subtitles.srt`. Additionally, the `-md` option specifies
the minimum duration for a subtitle. If the duration is shorter, the times are
automatically adjusted to ensure the minimum duration.
