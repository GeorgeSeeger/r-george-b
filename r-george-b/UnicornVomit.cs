using OpenRGB.NET;
using OpenRGB.NET.Models;
using System.Linq;

using RGeorgeB;

public class UnicornVomit : RgbStrategy {
    
    public override int MillisecondsToNextUpdate() => 150;

    private const int SpiralHueDensity = 360;

    private int counter = 0;

    private DateTime started = DateTime.UtcNow;

    private int NumpadCycleTimeMs = 8_000;

    public override void UpdateDevices(OpenRGBClient client, Device[] devices) {
        var spectrum = Color.GetHueRainbow(SpiralHueDensity).ToArray();
        Func<int, bool> shouldUpdateKey = _ => true;

        for (int i = 0; i < devices.Length; i++) {
            Device? device = devices[i];
            if (!IsKeyboard(device)) {
                if (ShouldUpdateModes()) {
                    client.SetMode(i, GetRainbowMode(device));
                }

                continue;
            }

            var keyboard = device;
            var numpadIndexes = keyboard.Leds
                            .Select((key, i) => (key, i))
                            .Where(k => NumpadArrowsAndControl.ContainsKey(k.key.Name))
                            .Select(v => v.i)
                            .ToArray();
            shouldUpdateKey = numpadIndexes.Contains;

            var cycleOffset = (int) Math.Round((SpiralHueDensity * (DateTime.UtcNow.Subtract(started).TotalMilliseconds % NumpadCycleTimeMs)) / NumpadCycleTimeMs);
            var leds = keyboard.Colors.Select((c, i) => shouldUpdateKey(i) ? GetRainbowSpiralFor(keyboard, i, spectrum, cycleOffset) : c).ToArray();

            client.UpdateLeds(i, leds);
        }

        counter = counter + 1 % 10000;
    }

    private Color GetRainbowSpiralFor(Device keyboard, int i, Color[] spectrum, int spectrumShift) {
        if (NumpadArrowsAndControl.ContainsKey(keyboard.Leds[i].Name)) {
            var (y, x) = NumpadArrowsAndControl[keyboard.Leds[i].Name];
            var angle = (int) Math.Round(Math.Atan(y /x) * (180 / Math.PI));
            var offset = (x < 0) ? 270 : 90;
            var spectrumIndex = spectrumShift + offset + angle;
            return spectrum[spectrumIndex % 360];
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

    private bool ShouldUpdateModes() {
        return counter == 0;
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