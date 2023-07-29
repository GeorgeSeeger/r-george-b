namespace RGeorgeB {
    public class RgbStrategyFactory {
        public IRgbStrategy Get() {
            return new SlowUniformWave();
        }
    }

}