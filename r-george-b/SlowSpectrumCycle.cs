using OpenRGB.NET;
using OpenRGB.NET.Models;

namespace RGeorgeB {
    public class SlowSpectrumCycle : RgbStrategy {
        protected const int CycleTimeMs = 600_000;

        protected Color[] Spectrum = Color.GetHueRainbow(CycleTimeMs / 500).ToArray();

        public override int MillisecondsToNextUpdate() => 500;

        protected int counter = 0;

        public override void UpdateDevices(OpenRGBClient client, Device[] devices) {
            var nextColour = GetNextColour();

            var gpuDevice = devices.IndexOf(IsGpu);

            Func<int, bool> shouldUpdateLedOnKeyboard = _ => true;
            if (devices.Any(IsKeyboard)) {
                shouldUpdateLedOnKeyboard = IsKeyNumpadArrowOrControl(devices.First(IsKeyboard));
            }

            for (int i = 0; i < devices.Length; i++) {
                var device = devices[i];

                if (!device.ActiveMode.Name.Contains("Direct")) {
                    var directMode = device.Modes.IndexOf(m => m.Name == "Direct");
                    client.SetMode(i, directMode);
                }
                
                var leds = Enumerable.Range(0, device.Colors.Length)
                    .Select(_ => nextColour)
                    .ToArray();
                if (!IsKeyboard(device)) {
                    client.UpdateLeds(i, leds);
                } else {
                    var keyboardLeds = device.Colors.Select((c, i) => shouldUpdateLedOnKeyboard(i) ? nextColour : c).ToArray();
                    client.UpdateLeds(i, keyboardLeds);
                }
            }
        }

        protected virtual Color GetNextColour() {
            this.counter = (this.counter + 1) % Spectrum.Length;
            return Spectrum[this.counter];
        }
    }
}