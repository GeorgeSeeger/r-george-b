using OpenRGB.NET;
using OpenRGB.NET.Models;

namespace RGeorgeB {
    public class SlowUniformWave : RgbStrategy {
        public Color[] Spectrum = Color.GetHueRainbow(12000).ToArray();
        public override int MillisecondsToNextUpdate() => 500;

        private int counter = 0;
        public override void UpdateDevices(OpenRGBClient client, Device[] devices) {
            this.counter = (this.counter + 1) % Spectrum.Length;
            var nextColour = Spectrum[this.counter];

            var gpuDevice = devices.IndexOf(IsGpu);
            var directMode = 0;
            if (devices.Any(IsGpu)) {
                var gpu = devices[gpuDevice];
                directMode = ((Mode[])gpu.Modes).IndexOf(m => m.Name == "Direct");
                client.SetMode(gpuDevice, directMode);
            }

            Func<int, bool> shouldUpdateLedOnKeyboard = _ => true;
            if (devices.Any(IsKeyboard)) {
                shouldUpdateLedOnKeyboard = IsKeyNumpadArrowOrControl(devices.First(IsKeyboard));
            }

            for (int i = 0; i < devices.Length; i++) {
                var device = devices[i];
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
    }
}