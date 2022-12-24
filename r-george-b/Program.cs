using OpenRGB.NET;
using OpenRGB.NET.Models;

await Task.Delay(3000);
using var client = new OpenRGBClient(name: "My OpenRGB Client", autoconnect: true, timeout: 1000);


var deviceCount = client.GetControllerCount();
var devices = client.GetAllControllerData();
var cycleTimeMs = args.Any() && int.TryParse(args[0], out var cycle) ? cycle : 16_000;
const int updateTimeMs = 100;


var rainbow = Color.GetHueRainbow(cycleTimeMs / updateTimeMs).ToArray();
var c = 0;

while (true) {
    c = (c + 1) % rainbow.Length;
    var nextColour = rainbow[c];

    for (int i = 0; i < devices.Length; i++) {
        var device = devices[i];
        var leds = Enumerable.Range(0, device.Colors.Length)
            .Select(_ => nextColour)
            .ToArray();
        client.UpdateLeds(i, leds);
    }

    await Task.Delay(updateTimeMs);
}