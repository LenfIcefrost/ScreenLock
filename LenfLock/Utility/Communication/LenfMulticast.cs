using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LenfLock.Utility.Communication {
    internal class LenfMulticast {
        private readonly string _multicastAddress = "224.0.0.1";
        private readonly int _multicastPort = 3141;
        private readonly int _gamePort;
        private readonly string _roomName;

        private readonly UdpClient _udp;
        private readonly IPAddress _mcastIp;
        private bool _running;
        private Task? _recvTask;
        private Task? _heartbeatTask;

        public LenfMulticast(string roomName = "screen_lock", int gamePort = 3141) {
            _roomName = roomName;
            _gamePort = gamePort;

            _mcastIp = IPAddress.Parse(_multicastAddress);

            var localEp = new IPEndPoint(IPAddress.Any, _multicastPort);

            _udp = new UdpClient();
            _udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udp.ExclusiveAddressUse = false;
            _udp.Client.Bind(localEp);

            _udp.JoinMulticastGroup(_mcastIp);
        }

        public void Start(bool enableHeartbeat = false) {
            if (_running) return;
            _running = true;

            _recvTask = Task.Run(ReceiveLoopAsync);
            Console.WriteLine("Multicasting Start");

            if (enableHeartbeat)
                _heartbeatTask = Task.Run(HeartbeatLoopAsync);
        }

        public void Stop() {
            if (!_running) return;
            _running = false;

            try { _udp.DropMulticastGroup(_mcastIp); } catch { }

            _udp.Close();

            try { _recvTask?.Wait(500); } catch { }
            try { _heartbeatTask?.Wait(500); } catch { }
        }

        private async Task ReceiveLoopAsync() {
            while (_running) {
                try {
                    var result = await _udp.ReceiveAsync();
                    var msg = Encoding.UTF8.GetString(result.Buffer);
                    var remoteEp = result.RemoteEndPoint;

                    JObject? json = null;

                    try {
                        json = JObject.Parse(msg);
                    } catch {
                        continue;
                    }

                    if (json.TryGetValue("type", out var typeToken) &&
                        typeToken.Type == JTokenType.String &&
                        typeToken.ToString() == "screenlock_discovery") {
                        var responseObj = new {
                            type = "screenlock_announce",
                            name = _roomName,
                            port = _gamePort,
                            version = "1.0"
                        };

                        string jsonResponse = JsonConvert.SerializeObject(responseObj);
                        byte[] data = Encoding.UTF8.GetBytes(jsonResponse);

                        await _udp.SendAsync(data, data.Length, remoteEp);
                    }
                } catch (ObjectDisposedException) {
                    break;
                } catch (Exception ex) {
                    Console.WriteLine($"[Host] Error: {ex.Message}");
                    if (!_running) break;
                }
            }
        }

        private async Task HeartbeatLoopAsync() {
            var ep = new IPEndPoint(_mcastIp, _multicastPort);

            while (_running) {
                try {
                    var heartbeatObj = new {
                        type = "screenlock_heartbeat",
                        roomName = _roomName,
                        port = _gamePort,
                        time = DateTime.UtcNow
                    };

                    string json = JsonConvert.SerializeObject(heartbeatObj);
                    byte[] data = Encoding.UTF8.GetBytes(json);

                    await _udp.SendAsync(data, data.Length, ep);
                    await Task.Delay(2000);
                } catch (ObjectDisposedException) {
                    break;
                } catch (Exception ex) {
                    Console.WriteLine($"[Host] Heartbeat Error: {ex.Message}");
                    if (!_running) break;
                }
            }
        }

        public void Dispose() {
            Stop();
            _udp.Dispose();
        }
    }
}
