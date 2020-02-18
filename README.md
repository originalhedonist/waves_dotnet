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

To link the channels, have only one `ChannelSettings` section, even if NumberOfChannels is 2.
To have independent channels, have two `ChannelSettings` sections - even if they are both the same (the will almost certainly be different if `Randomization` is true)
Examples can be found in `Settings.IndependentChannels.json` and `Settings.LinkedChannels.json`.

### WaveformExpressionParams
This is a function that will be evaluated at runtime. It can use the following parameters:
* *x* - this is 'pseudo-time', or pulse-frequency-stretched time. If PulseFrequency is 1, then sin(x) = sin(2*pi*f*t). The frequency of a sin(x) wave will match the pulse frequency.

## To develop using Visual Studio:

* Download any version of Visual Studio from Microsoft
* Open wavegenerator.sln
* Edit code as you like
* Press F5 to run
* Submit pull request? (If you've added anything interesting!)
