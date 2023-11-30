using OpenRGB.NET;
using OpenRGB.NET.Models;

namespace RGeorgeB {
    public class DaylightSpectrum : SlowSpectrumCycle {
        public override int MillisecondsToNextUpdate() => 60_000;

        private const int MinutesInADay = 24 * 60;

        protected new Color[] Spectrum = Color.GetHueRainbow(1 * MinutesInADay / 6, 300, 0.1666) // my preference, "midnight" is purple, so take the last 60 degrees of the HSV circle
        .Concat(Color.GetHueRainbow(5 * MinutesInADay / 6, 0, 0.83333)) .ToArray(); // and add the first 300 degrees.

        protected override Color GetNextColour() {
            var minutesSinceMidnight = ((int)Math.Floor((DateTime.Now - DateTime.Now.Date).TotalMinutes)) % MinutesInADay;
            var nextColour = Spectrum[minutesSinceMidnight];
            return nextColour;
        }
    }
}