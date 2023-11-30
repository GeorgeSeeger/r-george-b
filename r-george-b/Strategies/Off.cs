using OpenRGB.NET;
using OpenRGB.NET.Models;
using RGeorgeB;

public class Off : IRgbStrategy {
    public static bool IsRandomlySelectable() => false;

    public int MillisecondsToNextUpdate() {
        return 10_000;
    }

    public void UpdateDevices(OpenRGBClient client, Device[] devices) {
        for (int i = 0; i < devices.Length; i++) {
            var device = devices[i];
            if (device.ActiveMode.Name != "Off" && device.Modes.Any(m => m.Name == "Off")) {
                client.SetMode(i, device.Modes.IndexOf(m => m.Name == "Off"));
                devices = client.GetAllControllerData();
            }
        }
    }
}
