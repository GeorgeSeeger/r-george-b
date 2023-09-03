namespace RGeorgeB
{
    using System;
    using System.Collections;

    public class RgbStrategySelector {
        public IRgbStrategy Get(string[]? args = null) {
            if (args == null || !args.Any()) return new Random();

            var arg = args.First();
            var (strategy, closeMatches) = Search(arg);
            if (strategy == null) { throw new InvalidOperationException(); }

            return (IRgbStrategy)(Activator.CreateInstance(strategy) ?? throw new Exception());
        }

        public static (Type? strategy, string[]? closeMatches) Search(string? arg) {
            var strategies = typeof(RgbStrategySelector).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IRgbStrategy)) && !t.IsAbstract && t.IsClass)
            .ToArray();

            switch (arg?.ToLower()) {
                case null:
                case "":
                case "help":
                case "--help":
                case "-h":
                    return (null, strategies.Select(s => s.Name).ToArray());
                    
                default: break;
            }

            var strategy = strategies.SingleOrDefault(s => string.Equals(s.Name, arg, StringComparison.InvariantCultureIgnoreCase));
            string[]? closeMatches = null;
            if (strategy == null) {
                closeMatches = strategies
                .Select(s => (strat: s, distance: s.Name.ToLower().LevensteinDistance(arg.ToLower())))
                .OrderBy(s => s.distance)
                .Where(s => s.distance < 5)
                .Select(s => s.strat.Name)
                .ToArray();
            }

            return (strategy, closeMatches);
        }
    }
}