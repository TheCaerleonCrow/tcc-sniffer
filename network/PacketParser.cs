using PhotonPackageParser;
using TCC.Sniffer.Templates;

namespace TCC.Sniffer
{
    public class PacketParser : PhotonParser
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // These are the special dictionary keys in the packet data.
        public static byte EventSig = 252;
        public static byte OperationSig = 253;

        public bool Debug;
        public bool DebugAllCodes = false;
        public bool StopOnDebug = true;
        public List<short> DebugEventCodes = new List<short>();
        public List<short> DebugRequestCodes = new List<short>();
        public List<short> DebugResponseCodes = new List<short>();

        private Dictionary<PacketType, List<short>> _filteredCodes;
        private Dictionary<PacketType, Dictionary<short, Type>> _registeredPackets;
        private List<Action<PacketTemplate>> _handlePacketCallbacks;

        private SocketServer _server;

        public PacketParser() : base()
        {
            _filteredCodes = new Dictionary<PacketType, List<short>>();
            _registeredPackets = new Dictionary<PacketType, Dictionary<short, Type>>();
            _handlePacketCallbacks = new List<Action<PacketTemplate>>();

            // Creates the dictionaries and lists for each packet type.
            foreach (PacketType type in Enum.GetValues(typeof(PacketType)))
            {
                _filteredCodes[type] = new List<short>();
                _registeredPackets[type] = new Dictionary<short, Type>();
            }
        }

        public void FilterCode(PacketType packetType, short code)
        {
            _filteredCodes[packetType].Add(code);
        }

        public void RegisterPacket(PacketTemplate packet)
        {
            _registeredPackets[packet.Type].Add(packet.Code, packet.GetType());
        }

        public void OnHandlePacket(Action<PacketTemplate> callback)
        {
            _handlePacketCallbacks.Add(callback);
        }

        public void AddDebugCode(PacketType packetType, short code)
        {
            switch(packetType)
            {
                case PacketType.EVENT: DebugEventCodes.Add(code); break;
                case PacketType.REQUEST: DebugRequestCodes.Add(code); break;
                case PacketType.RESPONSE: DebugResponseCodes.Add(code); break;
            }
        }

        protected override void OnEvent(byte code, Dictionary<byte, object> rawData)
        {
            // Move packets work a little differently. We have to add the event code manually to them...
            if (code == 3)
                rawData.Add(EventSig, (short)3);

            HandlePacket(EventSig, PacketType.EVENT, rawData);
        }

        protected override void OnRequest(byte code, Dictionary<byte, object> rawData)
        {
            HandlePacket(OperationSig, PacketType.REQUEST, rawData);
        }

        protected override void OnResponse(byte code, short returnCode, string debugMessage, Dictionary<byte, object> rawData)
        {
            // We could extend responses somehow with these extra parameters...
            HandlePacket(OperationSig, PacketType.RESPONSE, rawData);
        }

        private void HandlePacket(byte sig, PacketType packetType, Dictionary<byte, object> rawData)
        {
            // If an event/op code is not found, do not continue.
            if (!rawData.TryGetValue(sig, out object rawCode))
                return;

            // If we are filtering this code, do not continue.
            if (_filteredCodes[packetType].Contains((short)rawCode))
                return;

            // If debugging, just print the packet data.
            if (Debug)
            {
                bool debuggedPacket = DebugPacket((short)rawCode, packetType, rawData);

                if (StopOnDebug && debuggedPacket) return;
            }

            // If this packet is not registered, do not continue.
            if (!_registeredPackets[packetType].ContainsKey((short)rawCode))
                return;

            // Get the class type of this packet, create an instance of it, and send it out to listeners.
            var dataType = _registeredPackets[packetType][(short)rawCode];
            var dataInstance = (PacketTemplate)Activator.CreateInstance(dataType, rawData);

            foreach (Action<PacketTemplate> callback in _handlePacketCallbacks)
            {
                callback(dataInstance);
            }

            logger.Debug("[{0}][{1}] Packet Processed", Enum.GetName(typeof(PacketType), packetType), (short)rawCode);
        }

        private bool DebugPacket(short rawCode, PacketType packetType, Dictionary<byte, object> rawData)
        {
            if (!DebugAllCodes)
            {
                var packetSelected = packetType switch
                {
                    PacketType.EVENT => DebugEventCodes.Contains(rawCode),
                    PacketType.REQUEST => DebugRequestCodes.Contains(rawCode),
                    PacketType.RESPONSE => DebugResponseCodes.Contains(rawCode),
                    _ => false,
                };
                if (!packetSelected) return false;
            }

            // Get the packet data.
            string s = "";
            foreach (KeyValuePair<byte, object> value in rawData)
            {
                Type valueType = value.Value.GetType();

                if (valueType.IsArray)
                {
                    var array = ParseArray(value.Value);
                    if (array != null)
                        s += $"\t({value.Key}:{string.Join(",", array)}) ({valueType.Name})\n";
                }
                else
                {
                    s += $"\t({value.Key}:{value.Value}) ({valueType.Name})\n";
                }
            }

            // Get the name of the packet.
            string codeName = "";
            switch (packetType)
            {
                case PacketType.EVENT: codeName = Enum.GetName(typeof(EventCode), rawCode); break;
                case PacketType.REQUEST: codeName = Enum.GetName(typeof(OperationCode), rawCode); break;
                case PacketType.RESPONSE: codeName = Enum.GetName(typeof(OperationCode), rawCode); break;
            }

            logger.Debug("[{0}][{1}] : {2}\n{3}", Enum.GetName(typeof(PacketType), packetType), rawCode, codeName, s);

            return true;
        }

        public static bool ParseBool(object value) =>
           value as bool? ?? false;

        public static byte ParseByte(object value) =>
            value as byte? ?? 0;

        public static short ParseShort(object value) =>
            value as byte? ?? value as short? ?? 0;

        public static int ParseInt(object value) =>
            (value as byte? ?? value as short? ?? value as int? ?? 0) / 10000;

        public static long ParseLong(object value) =>
            (value as byte? ?? value as short? ?? value as int? ?? value as long? ?? 0) / 10000L;

        public static float ParseFloat(object value) =>
            value as float? ?? 0f;

        public static double ParseDouble(object value) =>
            value as float? ?? value as double? ?? 0d;

        public static Byte[] ParseByteArray(object value) => (Byte[])value;
        public static Int16[] ParseShortArray(object value) => (Int16[])value;
        public static Int32[] ParseIntArray(object value) => (Int32[])value;
        public static Int64[] ParseLongArray(object value) => (Int64[])value;
        public static Boolean[] ParseBoolArray(object value) => (Boolean[])value;
        public static String[] ParseStringArray(object value) => (String[])value;
        public static Single[] ParseSingleArray(object value) => (Single[])value;
        public static Double[] ParseDoubleArray(object value) => (Double[])value;

        /// <summary>
        /// This is used when we don't know or care what the value type in the array is.
        /// We simply want to iterate over it to do things like printing the array.
        /// </summary>
        public static IEnumerable<object> ParseArray(object value)
        {
            if (!value.GetType().IsArray)
                return null;

            Array array = null;

            switch (value.GetType().GetElementType().Name)
            {
                case nameof(Byte): array = ParseByteArray(value); break;
                case nameof(Int16): array = ParseShortArray(value); break;
                case nameof(Int32): array = ParseIntArray(value); break;
                case nameof(Int64): array = ParseLongArray(value); break;
                case nameof(Boolean): array = ParseBoolArray(value); break;
                case nameof(String): array = ParseStringArray(value); break;
                case nameof(Single): array = ParseSingleArray(value); break;
                case nameof(Double): array = ParseDoubleArray(value); break;
            }

            if (array == null)
                return null;

            return array.Cast<object>();
        }
    }
}
