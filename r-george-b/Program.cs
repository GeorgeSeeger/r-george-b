using OpenRGB.NET;
using OpenRGB.NET.Models;
using System.Linq;
using System.Text.RegularExpressions;

public class Program {
    public static async Task Main(string[] args) {
        for (var i = 4; i >= 0; i--) {
            try {
                await ManageLedsAsync();
            }
            catch {
                if (i == 0) throw;
                await Task.Delay(10_000);
            }
        }

        async Task ManageLedsAsync() {
            using var client = new OpenRGBClient(name: "My OpenRGB Client", autoconnect: true, timeout: 1000);

            var deviceCount = client.GetControllerCount();
            var devices = client.GetAllControllerData();
            
            var cycleTimeMs = args.Any() && int.TryParse(args[0], out var cycle) ? cycle : 16_000;
            const int updateTimeMs = 500;

            var spectrum = Color.GetHueRainbow(cycleTimeMs / updateTimeMs).ToArray();
            // var spectrum = Color.GetSinRainbow(cycleTimeMs / updateTimeMs, 130, 125, 1, 0).ToArray();
            var c = 0;

            while (true) {
                c = (c + 1) % spectrum.Length;
                var nextColour = spectrum[c];

                var gpuDevice = devices.IndexOf(isGpu);
                var directMode = 0;
                if (devices.Any(isGpu)) {
                    directMode = ((Mode[])devices[gpuDevice].Modes).IndexOf(m => m.Name == "Direct");
                }

                Func<int, bool> shouldUpdateLedOnKeyboard = _ => true;
                if (devices.Any(isKeyboard)) {
                    shouldUpdateLedOnKeyboard = ShouldUpdateLedOnKeyboard(devices.First(isKeyboard));
                }

                for (int i = 0; i < devices.Length; i++) {
                    var device = devices[i];
                    var leds = Enumerable.Range(0, device.Colors.Length)
                        .Select(_ => nextColour)
                        .ToArray();
                    if (isGpu(device)) {
                        client.UpdateLeds(i, leds);
                        client.SetMode(i, directMode);
                        // client.SaveMode(i);
                    }
                    else if (isKeyboard(device)) {
                        var keyboardLeds = device.Colors.Select((c, i) => shouldUpdateLedOnKeyboard(i) ? nextColour : c).ToArray();
                        client.UpdateLeds(i, keyboardLeds);
                    } else
                        client.UpdateLeds(i, leds);
                }

                await Task.Delay(updateTimeMs);
            }

            bool isGpu(Device device) => device.Type == OpenRGB.NET.Enums.DeviceType.Gpu;

            bool isKeyboard(Device device) => device.Type == OpenRGB.NET.Enums.DeviceType.Keyboard;

            Func<int, bool> ShouldUpdateLedOnKeyboard(Device keyboard) {
                var keyIndexes = keyboard.Leds.Select((key, i) => (key, i))
                                .Where(t => t.key.Name.Contains("Number Pad")
                                    || t.key.Name.Contains("Arrow")
                                    || t.key.Name.Contains("Media")
                                    || t.key.Name.Contains("Logo")
                                    || NamedKeys.Contains(t.key.Name))
                                .Select(t => t.i)
                                .ToHashSet();
                return keyIndexes.Contains;
            } 
        }
    }

    private static HashSet<string> NamedKeys = new HashSet<string> {
        "Key: Print Screen",
        "Key: Scroll Lock",
        "Key: Num Lock",
        "Key: Pause/Break",
        "Key: Insert",
        "Key: Delete",
        "Key: Home",
        "Key: End",
        "Key: Page Up",
        "Key: Page Down",
    };
}

public static class EnumerableExtensions {
    public static int IndexOf<T>(this T[] array, Func<T, bool> predicate) {
        for (var i = 0; i < array.Length; i++) {
            if (predicate(array[i])) return i;
        }

        return -1;
    }
}
