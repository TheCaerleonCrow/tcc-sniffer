using NLog;
using PacketDotNet;
using SharpPcap;
using System.Text;
using TCC.Sniffer;
using TCC.Sniffer.Templates;

namespace TCC
{
    internal class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static PacketParser _parser;
        private static SocketServer _server;
        private static List<Thread> _deviceThreads;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="debug"></param>
        /// <param name="debugAllPackets"></param>
        /// <param name="stopOnDebug"></param>
        /// <param name="events"></param>
        private static async Task<int> Main(
            string ip = "127.0.0.1",
            int port = 9999,
            bool debug = false,
            bool debugAllPackets = false,
            bool stopOnDebug = false,
            string events = "",
            string requests = "",
            string responses = "")
        {
            Console.OutputEncoding = Encoding.UTF8;
            SetupLogger();

            logger.Info("Sniffer Started! Setting up...");

            SetupServer(ip, port);
            SetupParser(debug, debugAllPackets, stopOnDebug, 
                events.Split(',').Select(Int16.Parse).ToArray(),
                requests.Split(',').Select(Int16.Parse).ToArray(),
                responses.Split(',').Select(Int16.Parse).ToArray()
            );
            SetupDevices();

            logger.Info("Setup Complete!");

            return 0;
        }

        private static void SetupLogger()
        {
            NLog.LogManager.Setup().LoadConfiguration(builder =>
            {
                var layout = "[${level}][${time}] : ${message}";
                builder.ForLogger().WriteToConsole(layout: layout, encoding: Encoding.UTF8);
                builder.ForLogger().WriteToFile(fileName: "logs/sniffer.txt", layout: layout, encoding: Encoding.UTF8);
            });
        }

        private static void SetupServer(string ip, int port)
        {
            logger.Info("Setting up server...");
            _server = new SocketServer(ip, port);
        }

        private static void SetupParser(
            bool debug, bool debugAllCodes, bool stopOnDebug, 
            short[]? events, 
            short[]? requests, 
            short[]? responses)
        {
            logger.Info("Setting up parser...");
            _parser = new PacketParser();

            // Debugging settings, useful to find/learn about certain packets.
            _parser.Debug = debug;
            _parser.DebugAllCodes = debugAllCodes;
            _parser.StopOnDebug = stopOnDebug;
            //_parser.AddDebugCode(PacketType.EVENT, (short)EventCode.UpdateMoney);
            //_parser.AddDebugCode(PacketType.EVENT, (short)EventCode.ChatMessage);

            foreach (var code in events)
            {
                _parser.AddDebugCode(PacketType.EVENT, code);
            }

            foreach (var code in requests)
            {
                _parser.AddDebugCode(PacketType.REQUEST, code);
            }

            foreach (var code in responses)
            {
                _parser.AddDebugCode(PacketType.RESPONSE, code);
            }

            // Filter certain packets that we don't care about.
            // Mostly used for debugging, but this does filter them out completely.
            _parser.FilterCode(PacketType.EVENT, (short)EventCode.Move);
            _parser.FilterCode(PacketType.REQUEST, (short)OperationCode.Move);
            //_parser.FilterCode(PacketType.EVENT, (short)EventCode.ChatMessage);
            _parser.FilterCode(PacketType.EVENT, (short)EventCode.Unknown144);
            _parser.FilterCode(PacketType.EVENT, (short)EventCode.InCombatStateUpdate);
            _parser.FilterCode(PacketType.REQUEST, (short)OperationCode.ClientHardwareStats);

            // Register packets.
            // Later we should use reflection to automatically register everything in TCC.Sniffer.Templates.
            _parser.RegisterPacket(new EventChatMessage());
            _parser.RegisterPacket(new EventUpdateMoney());
            _parser.RegisterPacket(new EventGrabbedLoot());

            // 
            _parser.OnHandlePacket(_server.SendData);
        }

        private static void SetupDevices()
        {
            logger.Info("Setting up devices...");

            _deviceThreads = new List<Thread>();

            foreach (var device in CaptureDeviceList.Instance)
            {
                Thread thread = new Thread(() =>
                {
                    device.OnPacketArrival += new PacketArrivalEventHandler(PacketHandler);
                    device.Open(DeviceModes.Promiscuous, 1000);
                    device.StartCapture();

                    //Console.WriteLine($"Listening on {device.Description}");
                    logger.Info("Listening to device: {0}", device.Description);
                });

                thread.Start();
                _deviceThreads.Add(thread);
            }
        }

        private static void PacketHandler(object s, PacketCapture e)
        {
            var raw = e.GetPacket();
            var packet = Packet.ParsePacket(raw.LinkLayerType, raw.Data).Extract<UdpPacket>();

            if (packet == null) return;
            if (packet.SourcePort != 5056 && packet.DestinationPort != 5056 && packet.SourcePort != 4535) return;

            _parser.ReceivePacket(packet.PayloadData);
        }
    }
}
