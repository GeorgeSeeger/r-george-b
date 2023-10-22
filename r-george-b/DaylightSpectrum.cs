using OpenRGB.NET;
using OpenRGB.NET.Models;

namespace RGeorgeB {
    public class DaylightSpectrum : SlowSpectrumCycle {
        public override int MillisecondsToNextUpdate() => 60_000;

        private const int MinutesInADay = 24 * 60;

        protected new Color[] Spectrum = Color.GetHueRainbow(MinutesInADay)
                                            .OrderByDescending(c => c.ToHsv().h)
                                            .ToArray(); // 1440 colours

        protected override Color GetNextColour() {
            var minutesSinceMidnight = ((int)Math.Floor((DateTime.Now - DateTime.Now.Date).TotalMinutes)) % MinutesInADay;
            var nextColour = Spectrum[minutesSinceMidnight];
            return nextColour;
        }
    }
}