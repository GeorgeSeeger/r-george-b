namespace RGeorgeB {
    using System;

    public class RgbStrategyFactory {
        private Random random = new Random();
        public IRgbStrategy Get(string[]? args = null) {
            var strategies = typeof(RgbStrategyFactory).Assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(IRgbStrategy)) && !t.IsAbstract && t.IsClass).ToArray();

            var strategy = strategies[random.Next(strategies.Length)];
            if (strategies.Any(s => s.Name == args?.FirstOrDefault())) {
                strategy = strategies.Single(s => s.Name == args?.First());
            }
            
            return (IRgbStrategy)(Activator.CreateInstance(strategy) ?? throw new Exception());
        }
    }

}