using OpenRGB.NET;
using OpenRGB.NET.Models;
using System.Linq;
using System.Text.RegularExpressions;

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
    var keyboardDefault = KeyboardConfig();
    
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
                var keyboardLeds = keyboardDefault.Select(c => c.Equals(new Color(0, 255, 0)) ? nextColour : c).ToArray();
                client.UpdateLeds(i, keyboardLeds);
            } else
                client.UpdateLeds(i, leds);
        }

        await Task.Delay(updateTimeMs);
    }
}

bool isGpu(Device device) => device.Type == OpenRGB.NET.Enums.DeviceType.Gpu;

bool isKeyboard(Device device) => device.Type == OpenRGB.NET.Enums.DeviceType.Keyboard;

Color[] KeyboardConfig() {
    var keyboardColourString = @"R:255,G:0,B:0},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:255,G:255,B:0},{R:0,G:12,B:255},{R:255,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:255},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:0,B:0},{R:0,G:12,B:255},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:255},{R:255,G:145,B:0},{R:255,G:0,B:0},{R:255,G:0,B:0},{R:255,G:145,B:0},{R:0,G:12,B:255},{R:0,G:255,B:0},{R:0,G:12,B:255},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:255},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:0,B:0},{R:255,G:145,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:255},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:0,G:12,B:255},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:255},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:0,G:255,B:0},{R:0,G:12,B:255},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:255},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:255},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:255},{R:255,G:145,B:0},{R:255,G:145,B:0},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:255,B:255},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:255,G:255,B:255},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:0,G:12,B:255},{R:0,G:12,B:255},{R:0,G:255,B:0},{R:0,G:255,B:0},{R:255,G:0,B:0";
    return keyboardColourString
        .Split("},{")
        .Select(s => Regex.Match(s, @"R:(\d+),G:(\d+),B:(\d+)"))
        .Select(m =>
        {
            return new Color(byte.Parse(m.Groups[1].Value), byte.Parse(m.Groups[2].Value), byte.Parse(m.Groups[3].Value));
        })
        .ToArray();
} 

public static class EnumerableExtensions {
    public static int IndexOf<T>(this T[] array, Func<T, bool> predicate) {
        for (var i = 0; i < array.Length; i++) {
            if (predicate(array[i])) return i;
        }

        return -1;
    }
}
