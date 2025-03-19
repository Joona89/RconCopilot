using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RconCopilot
{
    public class RconClient
    {
        private readonly string _ip;
        private readonly int _port;
        private readonly string _password;

        private TaskCompletionSource<string> _pendingResponseTcs;
        private readonly object _readLock = new object();

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private CancellationTokenSource _cts;

        // Event raised when a message is received from the server.
        public event EventHandler<string> OnMessageReceived;

        public RconClient(string ip, int port, string password)
        {
            _ip = ip;
            _port = port;
            _password = password;
        }

        // Connects and attempts to authenticate.
        public async Task ConnectAsync()
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_ip, _port);
            _stream = _tcpClient.GetStream();
            _cts = new CancellationTokenSource();

            // Send authentication packet (using type 3 for auth).
            var authPacket = CreatePacket(3, _password);
            await _stream.WriteAsync(authPacket, 0, authPacket.Length);

            // Read the authentication response (simplified).
            var response = await ReadResponseAsync(_cts.Token);

            _ = StartListeningAsync();
            //if (!response.Contains("Authentication successful"))
            //{
            //    throw new Exception("Authentication failed.");
            //}
        }

        // Sends a command and awaits its response via the background listener.
        public async Task<string> SendCommandAsync(string command, CancellationToken token)
        {
            // Ensure only one command is pending at any time.
            lock (_readLock)
            {
                if (_pendingResponseTcs != null)
                    throw new InvalidOperationException("A command is already pending.");
                _pendingResponseTcs = new TaskCompletionSource<string>();
            }

            // Send the command packet.
            var commandPacket = CreatePacket(2, command);
            await _stream.WriteAsync(commandPacket, 0, commandPacket.Length, token);

            // Register the cancellation token to cancel this pending TCS if needed.
            using (token.Register(() =>
            {
                lock (_readLock)
                {
                    _pendingResponseTcs?.TrySetCanceled();
                    _pendingResponseTcs = null;
                }
            }))
            {
                // Wait for the response to be set by the listening loop.
                return await _pendingResponseTcs.Task;
            }
        }

        // Continuously listens for server messages.
        public async Task StartListeningAsync()
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var message = await ReadResponseAsync(_cts.Token);
                    // Use the lock to decide where to route this message.
                    lock (_readLock)
                    {
                        if (_pendingResponseTcs != null)
                        {
                            // If a command is pending, complete its TaskCompletionSource.
                            _pendingResponseTcs.SetResult(message);
                            _pendingResponseTcs = null;
                        }
                        else
                        {
                            // Otherwise, fire the event for unsolicited messages.
                            OnMessageReceived?.Invoke(this, message);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Listener canceled gracefully.
            }
            catch (Exception ex)
            {
                OnMessageReceived?.Invoke(this, "Listening error: " + ex.Message);
            }
        }

        // Disconnects from the server.
        public void Disconnect()
        {
            _cts?.Cancel();
            _stream?.Close();
            _tcpClient?.Close();
        }

        // Helper: Creates a packet following the (simplified) RCON protocol.
        // Packet structure: 
        //   [4 bytes: packet size] + [4 bytes: RequestID (set to 1)] + [4 bytes: Type] +
        //   [Payload bytes (UTF-8) with terminating null] + [An extra null terminator]
        // Type 3 = Authentication, Type 2 = Command execution.
        private byte[] CreatePacket(int type, string payload)
        {
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
            // 4 bytes for RequestID, 4 bytes for Type, payload bytes and 2 null terminators.
            int packetSize = 4 + 4 + payloadBytes.Length + 2;
            byte[] packet = new byte[4 + packetSize]; // 4 extra bytes for packet length.

            // Write packet length.
            Array.Copy(BitConverter.GetBytes(packetSize), 0, packet, 0, 4);
            // Write RequestID (using 1).
            Array.Copy(BitConverter.GetBytes(1), 0, packet, 4, 4);
            // Write packet Type.
            Array.Copy(BitConverter.GetBytes(type), 0, packet, 8, 4);
            // Write payload.
            Array.Copy(payloadBytes, 0, packet, 12, payloadBytes.Length);
            // Two null terminators.
            packet[12 + payloadBytes.Length] = 0;
            packet[12 + payloadBytes.Length + 1] = 0;

            return packet;
        }

        // Helper: Reads an entire RCON response from the server.
        private async Task<string> ReadResponseAsync(CancellationToken token)
        {
            // First, read 4 bytes to determine the packet length.
            byte[] lengthBuffer = new byte[4];
            int read = await _stream.ReadAsync(lengthBuffer, 0, 4, token);
            if (read < 4)
                throw new Exception("Failed to read packet length.");
            int packetLength = BitConverter.ToInt32(lengthBuffer, 0);

            byte[] responseBuffer = new byte[packetLength];
            int totalRead = 0;
            while (totalRead < packetLength)
            {
                int r = await _stream.ReadAsync(responseBuffer, totalRead, packetLength - totalRead, token);
                if (r <= 0)
                    break;
                totalRead += r;
            }

            // Skip the first 8 bytes (RequestID and Type) to get the payload.
            if (packetLength > 8)
            {
                string response = Encoding.UTF8.GetString(responseBuffer, 8, packetLength - 8);
                return response.TrimEnd('\0');
            }
            return string.Empty;
        }
    }
}
