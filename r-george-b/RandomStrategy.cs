namespace RGeorgeB
{
    using System;
    using System.Reflection;
    using OpenRGB.NET;
    using OpenRGB.NET.Models;

    public class Random : IRgbStrategy {
        private System.Random random = new();

        public static bool IsRandomlySelectable() => false;

        private IRgbStrategy actualStrategy;

        public Random() {
            var strategies = typeof(RgbStrategySelector).Assembly.GetTypes()
            .Where(t => {
                return t.IsAssignableTo(typeof(IRgbStrategy))
                                && !t.IsAbstract
                                && t.IsClass
                                && (bool)(t.GetMethod(nameof(IRgbStrategy.IsRandomlySelectable), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.Invoke(null, null) ?? throw new DidntImplementIsRandomlyException());
            })
            .ToArray();

            var strategy = strategies.Where(s => s.Name != nameof(Off)).ToArray()[random.Next(strategies.Length - 1)];
            
            this.actualStrategy = (IRgbStrategy)(Activator.CreateInstance(strategy) ?? throw new Exception());
        }

        public int MillisecondsToNextUpdate() {
            return this.actualStrategy.MillisecondsToNextUpdate();
        }

        public void UpdateDevices(OpenRGBClient client, Device[] devices) {
            this.actualStrategy.UpdateDevices(client, devices);
        }

        class DidntImplementIsRandomlyException : InvalidOperationException { }
    }
}