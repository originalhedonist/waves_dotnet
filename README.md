# waves_dotnet

## To just run from the command line:

Download dotnet from here: https://dotnet.microsoft.com/download

(Click 'Download .NET Core Runtime' and then install it)

Then:
```
git clone https://github.com/originalhedonist/waves_dotnet
cd waves_dotnet
wavegenerator.bat
```


### To explain variance:
Variance is a measure of how much a value is dependent on randomness, and the progression through the track.

If you have Excel, you can open `VariationIllustration.xlsx` and tweak `Progression` and `Randomness` to see the spread of values.

If `Progression` is 1, the value will rise linearly as the track progresses.

If `Progression` is <1, the value will rise faster towards the start of the track, whereas if it is >1, the value will rise faster towards the end of the track.

If `Randomness` is high, the 'spread' of values around the progression will be wider.

These two images show the spread of values. The red line is the proportion of values that are at the maximum value.

![Randomness](https://github.com/originalhedonist/waves_dotnet/blob/master/randomness.png)

![Progression](https://github.com/originalhedonist/waves_dotnet/blob/master/progression.png)


The parameters are supplied in JSON format with the .json file being passed to the program on the command line.
An example of the parameters is as follows, but to make the program write out the default parameters so you can copy and edit it, just run the program with no command line arguments, and `default.parameters.json` will be saved.
```json
{
  "Randomization": false,
  "ConvertToMp3": true,
  "NumFiles": 1,
  "Naming": "RandomFemaleName",
  "TrackLength": "00:00:30",
  "Sections": {
    "TotalLength": "00:00:30",
    "MinFeatureLength": "00:00:05",
    "MaxFeatureLength": "00:00:20",
    "FeatureLengthVariance": {
      "Randomness": 1.0,
      "Progression": 0.5
    },
    "MinRampLength": "00:00:01",
    "MaxRampLength": "00:00:05",
    "RampLengthVariance": {
      "Randomness": 0.2,
      "Progression": 0.8
    },
    "ChanceOfFeature": 0.8
  },
  "PhaseShiftCarrier": true,
  "CarrierFrequency": {
    "Left": 600.0,
    "Right": 600.0
  },
  "PhaseShiftPulses": false,
  "PulseFrequency": {
    "Quiescent": 0.5,
    "Low": 0.2,
    "High": 1.2,
    "ChanceOfHigh": 0.8,
    "Variation": {
      "Randomness": 0.5,
      "Progression": 0.8
    }
  },
  "Wetness": {
    "LinkToFeature": true,
    "Minimum": 0.4,
    "Maximum": 0.9,
    "Variation": {
      "Randomness": 0.2,
      "Progression": 1.5
    }
  },
  "Breaks": {
    "MinTimeSinceStartOfTrack": "00:10:00",
    "MinTimeBetweenBreaks": "00:05:00",
    "MaxTimeBetweenBreaks": "00:30:00",
    "MinLength": "00:00:02",
    "MaxLength": "00:00:25",
    "RampLength": "00:00:05"
  }
}
```

## To develop using Visual Studio:

* Download any version of Visual Studio from Microsoft
* Open wavegenerator.sln
* Edit code as you like
* Press F5 to run
* Submit pull request? (If you've added anything interesting!)
