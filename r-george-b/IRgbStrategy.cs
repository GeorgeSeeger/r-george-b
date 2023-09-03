using OpenRGB.NET;
using OpenRGB.NET.Models;

public interface IRgbStrategy {
    static abstract bool IsRandomlySelectable();

    public void UpdateDevices(OpenRGBClient client, Device[] devices);

    public int MillisecondsToNextUpdate();
}
