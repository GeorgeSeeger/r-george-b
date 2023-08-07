using OpenRGB.NET;
using OpenRGB.NET.Models;

using System;

namespace RGeorgeB {
    public class Program {
        public static async Task Main(string[] args) {
            for (var i = 4; i >= 0; i--) {
                try {
                    await ManageLedsAsync(args);
                } catch {
                    if (i == 0) throw;
                    await Task.Delay(10_000);
                }
            }

            async Task ManageLedsAsync(string[] args) {
                using var client = new OpenRGBClient(name: "My OpenRGB Client", autoconnect: true, timeout: 1000);
                var strategy = new RgbStrategyFactory().Get(args);
                var devices = client.GetAllControllerData();
                var now = System.DateTime.UtcNow;
                
                while (true) {
                    strategy.UpdateDevices(client, devices);
                    await Task.Delay(strategy.MillisecondsToNextUpdate());

                    if (DateTime.UtcNow.Subtract(now) > TimeSpan.FromMinutes(3)) {
                        devices = UpdateDevices(client);
                    }
                }
            }

            Device[] UpdateDevices(OpenRGBClient client) {
                return client.GetAllControllerData();
            }
        }
    }
}