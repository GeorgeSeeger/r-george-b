namespace RGeorgeB {
    using System;
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.Text;
    using Microsoft.Extensions.Configuration;

    public class Program {
        const int port = 13000;

        private static IConfigurationRoot _config;

        private static IConfiguration Config {
            get {
                if (_config != null) return _config;
                    _config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.user.json", optional: false)
                        .Build();
                return _config;
            }
        }

        public static async Task Main(string[] args) {
            try {
                using var tcp = new TcpClient();
                await tcp.ConnectAsync("127.0.0.1", port);

                var message = string.Join(' ', args);
                do {
                    if (string.IsNullOrWhiteSpace(message)) {
                        message = EmptyMessage();
                    }

                    var data = Encoding.UTF8.GetBytes(message);

                    var stream = tcp.GetStream();
                    await stream.WriteAsync(data);

                    data = new byte[256];
                    var responseLen = await stream.ReadAsync(data);
                    var response = Encoding.UTF8.GetString(data, 0, responseLen);
                    Console.WriteLine(response);

                    Console.WriteLine("Press enter to exit, or retype your command");
                    message = Console.ReadLine();
                } while (!string.IsNullOrWhiteSpace(message));
            } catch (SocketException e) {
                if (e.ErrorCode == 10061) {
                    // actively refused connection, RGeorgeB likely not running. 
                    Console.WriteLine("R-George-B isn't running. Would you like to start it? [y/n]");
                    if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase)) {
                        StartNewRgbClient();
                    }
                    
                    await Main(args);
                }

                Console.WriteLine($"SocketException: {e}");
            }

        }

        private static void StartNewRgbClient() {
            var rgbExeLocation = Config.GetSection("r-george-b-location").Value;
            if (string.IsNullOrWhiteSpace(rgbExeLocation) || !File.Exists(rgbExeLocation)) {
                throw new InvalidOperationException($"Cannot find file at \"{rgbExeLocation}\".");
            }

            var rgbProgram = Process.Start(new ProcessStartInfo {
                FileName = rgbExeLocation,
                CreateNoWindow = true,
            });
            
            if (rgbProgram == null || rgbProgram.HasExited) {
                throw new InvalidOperationException("It's exited already, strange...");
            }
        }

        private static string EmptyMessage() {
            Console.WriteLine("Please specify a message for client:");
            var msg = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(msg)) {
                msg = EmptyMessage();
            }

            return msg;
        }
    }
}