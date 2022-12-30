using OpenRGB.NET;
using OpenRGB.NET.Models;
using  System.Linq;

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
    const int updateTimeMs = 100;


    var spectrum = Color.GetSinRainbow(cycleTimeMs / updateTimeMs, 130, 125, 1, 0).ToArray();
    var c = 0;

    while (true) {
        c = (c + 1) % spectrum.Length;
        var nextColour = spectrum[c];

        for (int i = 0; i < devices.Length; i++) {
            var device = devices[i];
            var leds = Enumerable.Range(0, device.Colors.Length)
                .Select(_ => nextColour)
                .ToArray();
            client.UpdateLeds(i, leds);
        }

        await Task.Delay(updateTimeMs);
    }
}