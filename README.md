# SharpRhythms

Common Rhythm game file format parser for dotnet

[![Build status](https://ci.appveyor.com/api/projects/status/xp5sogvacxd2h5xa?svg=true)](https://ci.appveyor.com/project/senritsu/sharprhythms)
[![Test status](http://46.101.111.212:3002/senritsu/sharprhythms)]
(https://ci.appveyor.com/project/senritsu/sharprhythms/build/tests)

## Current Features

- Common abstractions for rhythm game tracks/notes/events
- Parsing toolkit for MSD files
- Specific parsers for various MSD dialects. Currently only `.sm` files (Stepmania format pre-5.0) are supported.

## Usage

There are parsers available for common formats. They are located in the `SharpRhythms.Parsers.*` namespaces.

Most parsers require a `double AudioLengthAccessor(string audioFilePath)` delegate as a constructor parameter. This function gets the path to the music file for the beatmap as specified in the simfile, and should return the song length in seconds.

After instantiating a specific parser, beatmaps can be parsed from a variety of sources.

```
var track1 = parser.LoadFromFile(@"relative\path\to\file.sm");
var track1 = parser.LoadFromFile(@"C:\absolute\path\to\file.sm");
var track2 = parser.Load(fileContentsAsString);
var track3 = parser.Load(fileContentsAsStream);
```

### .sm

```
using SharpRhythms.Parsers.Msd;

private double GetSongLength(string songFilePath) {
// ...
}

// ...

var parser = new SMParser(GetSongLength);
```

