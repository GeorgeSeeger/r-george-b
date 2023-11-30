using OpenRGB.NET;
using OpenRGB.NET.Models;

namespace RGeorgeB {
    public class Fixed : SlowSpectrumCycle {
        private OpenRGB.NET.Models.Color fixedColor;
        public Fixed(string? color) {
           this.fixedColor = string.IsNullOrWhiteSpace(color) 
           ? new Color(255, 0, 0) 
           : color.FromHexString();
        }

        public static new bool IsRandomlySelectable() => false;

        public override int MillisecondsToNextUpdate() => 60 * 60_000;

        protected override Color GetNextColour() {
            return this.fixedColor;
        }
    }
}