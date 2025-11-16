using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using LenfLock.Utility.Communication;

namespace LenfLock {
    class LenfServer {
        Socket serverSocket;
        List<Socket> clients;
        public LenfServer() {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.ReceiveTimeout = 10000;
            clients = new List<Socket>();
        }
        public void Start() {
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 3141));
            serverSocket.Listen(100);
            new Thread(Accept) { IsBackground = true }.Start();
            Console.WriteLine("Server Start");
        }
        public void Accept() {
            while(true) {
                Socket client = serverSocket.Accept();
                clients.Add(client);

                new Thread(() => Receive(client)) { IsBackground = true }.Start();
                Console.WriteLine(((IPEndPoint)client.RemoteEndPoint).Address + " Connect");
            }
        }
        public void Receive(Socket Client) {
            while(Client.Connected) {
                try {
                    ArraySegment<byte> ByteReceive = new ArraySegment<byte>(new byte[2048]);
                    Task<int> ReceiveLenght = Client.ReceiveAsync(ByteReceive, SocketFlags.None);
                    string msg = Encoding.UTF8.GetString(ByteReceive.Array, 0, ReceiveLenght.Result);
                    if (msg.Length == 0) break;

                    Console.WriteLine((Client.RemoteEndPoint as IPEndPoint).Address + ":" + msg);
                    CommandHanlder.HanlderPack hanlderPack = CommandHanlder.Command(msg);

                    switch (hanlderPack.code) {
                        case CommandHanlder.ActionCode.None:
                            break;
                        case CommandHanlder.ActionCode.OpenApp:
                            MainInterface.instance.Invoke((MethodInvoker)delegate {
                                MainInterface.instance.show();
                            });
                            break;
                        case CommandHanlder.ActionCode.HideApp:
                            MainInterface.instance.Invoke((MethodInvoker)delegate {
                                MainInterface.instance.hide();
                            });
                            break;
                        case CommandHanlder.ActionCode.FreezeApp:
                            MainInterface.instance.Invoke((MethodInvoker)delegate {
                                MainInterface.instance.show(freeze: true);
                            });
                            break;
                        default:
                            break;
                    }

                    if (hanlderPack.msg.Length > 0) {
                        Client.Send(Encoding.UTF8.GetBytes(hanlderPack.msg));
                    }
                    //Client.Send(Encoding.UTF8.GetBytes(hanlderPack.msg));
                } catch(Exception e) {
                    try {
                        Client.Shutdown(SocketShutdown.Both);
                    } catch (Exception ee) {
                        Console.WriteLine(ee.StackTrace);
                    }
                    Console.WriteLine(e.StackTrace);
                    break;
                }
            }

            Client.Disconnect(true);
            Client.Close();
            Console.WriteLine($"{((IPEndPoint)Client.RemoteEndPoint).Address} Disconnect");
            clients.Remove(Client);
        }
    }
}
