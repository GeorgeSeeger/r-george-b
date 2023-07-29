using OpenRGB.NET;
using OpenRGB.NET.Models;

public interface IRgbStrategy {
    public void UpdateDevices(OpenRGBClient client, Device[] devices);

    public int MillisecondsToNextUpdate();
}
