namespace RGeorgeB {
    using System;

    public class RgbStrategyFactory {
        private Random random = new Random();
        public IRgbStrategy Get(string[]? args = null) { // todo implement args
            var strategies = typeof(RgbStrategyFactory).Assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(IRgbStrategy)) && !t.IsAbstract && t.IsClass).ToArray();
            return (IRgbStrategy)(Activator.CreateInstance(strategies[random.Next(strategies.Length)]) ?? throw new Exception());
        }
    }

}