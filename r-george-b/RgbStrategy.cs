namespace RGeorgeB {
    using OpenRGB.NET;
    using OpenRGB.NET.Models;


    public abstract class RgbStrategy : IRgbStrategy {
        public static bool IsRandomlySelectable() => true;

        public abstract int MillisecondsToNextUpdate();
        public abstract void UpdateDevices(OpenRGBClient client, Device[] devices);

        protected bool IsGpu(Device device) => device.Type == OpenRGB.NET.Enums.DeviceType.Gpu;

        protected bool IsKeyboard(Device device) => device.Type == OpenRGB.NET.Enums.DeviceType.Keyboard;

        protected Func<int, bool> IsKeyNumpadArrowOrControl(Device keyboard) {
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
}
