using System.Drawing;
using OpenRGB.NET;
using OpenRGB.NET.Models;

namespace RGeorgeB {
    public class Fixed : SlowSpectrumCycle {
        private OpenRGB.NET.Models.Color fixedColor;

        public Fixed(string? color) {
            var systemColour = typeof(System.Drawing.Color).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).FirstOrDefault(fi => string.Equals(fi.Name, color, StringComparison.OrdinalIgnoreCase))
            ?.GetValue(null) as System.Drawing.Color?;

            if (systemColour.HasValue) {
                this.fixedColor = new OpenRGB.NET.Models.Color(systemColour.Value.R, systemColour.Value.G, systemColour.Value.B);
            } 
            else if (!string.IsNullOrWhiteSpace(color) && color.TryParseFromHexString(out var fixedColour)) {
                this.fixedColor = fixedColour;
            } else {
                this.fixedColor = new OpenRGB.NET.Models.Color(0xf0, 0x60, 0x0); // Personal preference, but I like amber as a default
            }
        }

        public static new bool IsRandomlySelectable() => false;

        public override int MillisecondsToNextUpdate() => 60 * 60_000;

        protected override OpenRGB.NET.Models.Color GetNextColour() {
            return this.fixedColor;
        }
    }
}