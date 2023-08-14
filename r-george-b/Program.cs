using OpenRGB.NET;
using OpenRGB.NET.Models;

using System;
using System.Net;
using System.Net.Sockets;

namespace RGeorgeB {
    public class Program {
        public static async Task Main(string[] args) {
            using var cts = new CancellationTokenSource();
            var rgbThread = new Thread(new ParameterizedThreadStart(async (obj) => await RgbControl(obj)));
            var tcpThread = new Thread(new ParameterizedThreadStart(async (obj) => await TcpServer(obj as CancellationTokenSource)));

            tcpThread.Start(cts);
            rgbThread.Start((args, cts.Token));
            tcpThread.Join();

        }

        private static async Task RgbControl(object? obj) {
            if (obj is not (string[] args, CancellationToken token)) {
                throw new ArgumentException();
            }

            await RgbControl(args, token);
        }

        private static async Task RgbControl(string[] args, CancellationToken token) {
            for (var i = 4; i >= 0; i--) {
                try {
                    await ManageLedsAsync(args, token);
                } catch {
                    if (i == 0) throw;
                    await Task.Delay(10_000);
                }
            }
        }

        public static async Task TcpServer(CancellationTokenSource? cancellationTokenSource) {
            if (cancellationTokenSource == null) { throw new ArgumentNullException(); }

            var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 13000);
            server.Start();
            var bytes = new byte[256];
            while (true) {
                using var tcpClient = server.AcceptTcpClient();
                
                var stream = tcpClient.GetStream();
                var i = 0;
                while((i = stream.Read(bytes, 0, bytes.Length))!=0) {
                    var newArgs = System.Text.Encoding.UTF8.GetString(bytes, 0, i).Split(' ');
                    
                    byte[] msg = System.Text.Encoding.UTF8.GetBytes("ok");
                    stream.Write(msg, 0, msg.Length);

                    cancellationTokenSource.Cancel();
                    cancellationTokenSource = new CancellationTokenSource();

                    new Thread(new ParameterizedThreadStart(async (obj) => await RgbControl(obj))).Start((args: newArgs, cancellationToken: cancellationTokenSource.Token));
                }

            }
        }

        private static async Task ManageLedsAsync(string[] args, CancellationToken cancellationToken) {
            using var client = new OpenRGBClient(name: "R-George-B Client", autoconnect: true, timeout: 1000);
            var strategy = new RgbStrategyFactory().Get(args);
            var devices = client.GetAllControllerData();
            var now = System.DateTime.UtcNow;
            var threeMinutes = TimeSpan.FromMinutes(3);
            
            while (!cancellationToken.IsCancellationRequested) {
                strategy.UpdateDevices(client, devices);
                await Task.Delay(strategy.MillisecondsToNextUpdate());

                if (DateTime.UtcNow.Subtract(now) > threeMinutes) {
                    now = DateTime.UtcNow;
                    devices = UpdateDevices(client);
                }
            }

            Device[] UpdateDevices(OpenRGBClient client) {
                return client.GetAllControllerData();
            }
        }

    }
}