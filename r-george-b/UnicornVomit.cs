using System.Linq;
using OpenRGB.NET;
using OpenRGB.NET.Models;
using RGeorgeB;

public class UnicornVomit : RgbStrategy {

    public override int MillisecondsToNextUpdate() => 100;

    private const int SpiralHueDensity = 180;

    private const int NumpadCycleTimeMs = 8_000;

    private int counter = 0;

    private readonly DateTime started = DateTime.UtcNow;

    private readonly Color[] spectrum = Color.GetHueRainbow(SpiralHueDensity).ToArray();

    private Func<int, bool> shouldUpdateKey = _ => true;

    private Color[] keyboardsLeds = Array.Empty<Color>();

    public override void UpdateDevices(OpenRGBClient client, Device[] devices) {
        for (int i = 0; i < devices.Length; i++) {
            Device? device = devices[i];
            if (!IsKeyboard(device)) {
                if (ShouldUpdateModes()) {
                    client.SetMode(i, GetRainbowMode(device));
                }

                continue;
            }

            var keyboard = device;

            if (ShouldUpdateModes()) {
                var numpadIndexes = keyboard.Leds
                                .Select((key, i) => (key, i))
                                .Where(k => NumpadArrowsAndControl.ContainsKey(k.key.Name))
                                .Select(v => v.i)
                                .ToArray();
                shouldUpdateKey = numpadIndexes.Contains;
                keyboardsLeds = new Color[keyboard.Leds.Length];
            }

            var cycleOffset = (int)Math.Round((SpiralHueDensity * (DateTime.UtcNow.Subtract(started).TotalMilliseconds % NumpadCycleTimeMs)) / NumpadCycleTimeMs);

            for (int j = 0; j < keyboard.Colors.Length; j++) {
                var c = keyboard.Colors[j];
                this.keyboardsLeds[j] = shouldUpdateKey(j) ? GetRainbowSpiralFor(keyboard, j, cycleOffset) : c;
            }

            client.UpdateLeds(i, this.keyboardsLeds);
        }

        counter = counter + 1 % 10000;

        bool ShouldUpdateModes() => counter == 0;
    }

    private Color GetRainbowSpiralFor(Device keyboard, int i, int spectrumShift) {
        if (NumpadArrowsAndControl.TryGetValue(keyboard.Leds[i].Name, out var position)) {
            var (y, x) = position;
            var angle = (int)Math.Round(Math.Atan(y / x) * (SpiralHueDensity / (2 * Math.PI)));
            var offset = (x < 0) 
                ? 3 * SpiralHueDensity / 4 
                : SpiralHueDensity / 4;
            var spectrumIndex = spectrumShift + offset + angle;
            return spectrum[spectrumIndex % spectrum.Length];
        }

        return spectrum.First();
    }

    private int GetRainbowMode(Device device) {
        var modeIndex = device.Modes.IndexOf(m => m.Name.Contains("Rainbow"));
        if (modeIndex > 0) return modeIndex;
        modeIndex = device.Modes.IndexOf(m => m.Name.Contains("Spectrum"));
        if (modeIndex > 0) return modeIndex;

        throw new ArgumentOutOfRangeException($"Cannot find appropriate Rainbow mode for {device.Name}");
    }

    private Dictionary<string, (double y, double x)> NumpadArrowsAndControl = new Dictionary<string, (double y, double x)> {
        { "Key: Media Mute",        (y: 3.5, x:  0) },
        { "Key: Print Screen",      (y: 2.5, x: -3) },
        { "Key: Scroll Lock",       (y: 2.5, x: -2) },
        { "Key: Pause/Break",       (y: 2.5, x: -1) },
        { "Key: Media Stop",        (y: 2.5, x:  0) },
        { "Key: Media Previous",    (y: 2.5, x:  1) },
        { "Key: Media Play/Pause",  (y: 2.5, x:  2) },
        { "Key: Media Next",        (y: 2.5, x:  3) },
        { "Key: Insert",            (y: 1.5, x: -3) },
        { "Key: Home",              (y: 1.5, x: -2) },
        { "Key: Page Up",           (y: 1.5, x: -1) },
        { "Key: Num Lock",          (y: 1.5, x:  0) },
        { "Key: Number Pad /",      (y: 1.5, x:  1) },
        { "Key: Number Pad *",      (y: 1.5, x:  2) },
        { "Key: Number Pad -",      (y: 1.5, x:  3) },
        { "Key: Delete",            (y: 0.5, x: -3) },
        { "Key: End",               (y: 0.5, x: -2) },
        { "Key: Page Down",         (y: 0.5, x: -1) },
        { "Key: Number Pad 7",      (y: 0.5, x:  0) },
        { "Key: Number Pad 8",      (y: 0.5, x:  1) },
        { "Key: Number Pad 9",      (y: 0.5, x:  2) },
        { "Key: Number Pad +",      (y:   1, x:  3) },
        { "Key: Number Pad 4",      (y:-0.5, x:  0) },
        { "Key: Number Pad 5",      (y:-0.5, x:  1) },
        { "Key: Number Pad 6",      (y:-0.5, x:  2) },
        { "Key: Up Arrow",          (y:-1.5, x: -2) },
        { "Key: Number Pad 1",      (y:-1.5, x:  0) },
        { "Key: Number Pad 2",      (y:-1.5, x:  1) },
        { "Key: Number Pad 3",      (y:-1.5, x:  2) },
        { "Key: Number Pad Enter",  (y:  -2, x:  3) },
        { "Key: Left Arrow",        (y:-2.5, x: -3) },
        { "Key: Down Arrow",        (y:-2.5, x: -2) },
        { "Key: Right Arrow",       (y:-2.5, x: -1) },
        { "Key: Number Pad 0",      (y:-2.5, x:0.5) },
        { "Key: Number Pad .",      (y:-2.5, x:  2) },
    };
}