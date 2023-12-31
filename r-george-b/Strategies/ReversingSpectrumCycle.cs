using OpenRGB.NET.Models;

namespace RGeorgeB {
    public class ReversingSpectrumCycle : SlowSpectrumCycle {
        private System.Random random = new System.Random();

        public ReversingSpectrumCycle() {
            this.Spectrum = Color.GetHueRainbow(300_000 / 500).ToArray();
            this.counter = this.random.Next(this.Spectrum.Length);
        }

        private int increment = 1;

        protected override Color GetNextColour() {
            if (this.random.Next(5 * this.Spectrum.Length) == 0) {
                increment *= -1;
            }

            this.counter = (this.counter + this.increment) % Spectrum.Length;
            if (this.counter < 0) this.counter = Spectrum.Length - 1;
            return Spectrum[this.counter];
        }
    }
}