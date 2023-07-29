using System.Linq;
using System.Text.RegularExpressions;
using OpenRGB.NET;
using OpenRGB.NET.Models;
namespace RGeorgeB {
    public class Program {
        public static async Task Main(string[] args) {
            for (var i = 4; i >= 0; i--) {
                try {
                    await ManageLedsAsync();
                } catch {
                    if (i == 0) throw;
                    await Task.Delay(10_000);
                }
            }

            async Task ManageLedsAsync() {
                using var client = new OpenRGBClient(name: "My OpenRGB Client", autoconnect: true, timeout: 1000);
                var strategy = new RgbStrategyFactory().Get();
                var deviceCount = client.GetControllerCount();
                var devices = client.GetAllControllerData();

                while (true) {
                    strategy.UpdateDevices(client, devices);
                    await Task.Delay(strategy.MillisecondsToNextUpdate());
                }
            }
        }
    }
    public static class EnumerableExtensions {
        public static int IndexOf<T>(this T[] array, Func<T, bool> predicate) {
            for (var i = 0; i < array.Length; i++) {
                if (predicate(array[i])) return i;
            }

            return -1;
        }
    }
}