using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

public class Program {
    const int port = 13000;

    public static async Task Main(string[] args) {
        try {
            using var tcp = new TcpClient();
            await tcp.ConnectAsync("127.0.0.1", port);

            var message = string.Join(' ', args);
            if (string.IsNullOrWhiteSpace(message)) {
                message = EmptyMessage();
            }

            var data = Encoding.UTF8.GetBytes(message);

            var stream = tcp.GetStream();
            await stream.WriteAsync(data, 0, data.Length);

            data = new byte[256];
            var responseLen = await stream.ReadAsync(data, 0, data.Length);
            var response = Encoding.UTF8.GetString(data, 0, responseLen);
            Console.WriteLine(response);
        } catch (SocketException e) {
            Console.WriteLine($"SocketException: {e}");
        }

        Console.WriteLine("\n Press any key to exit");
        Console.Read();
    }

    private static string EmptyMessage() {
        Console.WriteLine("Please specify a message for client:");
        var msg =  Console.ReadLine();
        if (string.IsNullOrWhiteSpace(msg)) {
            msg = EmptyMessage();
        }

        return msg;
    }
}