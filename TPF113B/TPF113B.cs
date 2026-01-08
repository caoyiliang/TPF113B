using Communication;
using Communication.Bus.PhysicalPort;
using Communication.Exceptions;
using LogInterface;
using Modbus;
using Modbus.Parameter;
using TopPortLib.Interfaces;
using Utils;

namespace TPF113B
{
    public class TPF113B : ITPF113B
    {
        private static readonly ILogger _logger = Logs.LogFactory.GetLogger<TPF113B>();
        private readonly IModBusMaster _modBusMaster;
        private bool _isConnect = false;
        public bool IsConnect => _isConnect;

        /// <inheritdoc/>
        public event DisconnectEventHandler? OnDisconnect { add => _modBusMaster.OnDisconnect += value; remove => _modBusMaster.OnDisconnect -= value; }
        /// <inheritdoc/>
        public event ConnectEventHandler? OnConnect { add => _modBusMaster.OnConnect += value; remove => _modBusMaster.OnConnect -= value; }

        public TPF113B(SerialPort serialPort, int defaultTimeout = 5000)
        {
            _modBusMaster = new ModBusMaster(serialPort, ModbusType.RTU, defaultTimeout)
            {
                IsHighByteBefore_Req = true,
                IsHighByteBefore_Rsp = false
            };
            _modBusMaster.OnSentData += CrowPort_OnSentData;
            _modBusMaster.OnReceivedData += CrowPort_OnReceivedData;
            _modBusMaster.OnConnect += CrowPort_OnConnect;
            _modBusMaster.OnDisconnect += CrowPort_OnDisconnect;
        }

        public TPF113B(ICrowPort crowPort)
        {
            _modBusMaster = new ModBusMaster(crowPort)
            {
                IsHighByteBefore_Req = true,
                IsHighByteBefore_Rsp = false
            };
            _modBusMaster.OnConnect += CrowPort_OnConnect;
            _modBusMaster.OnDisconnect += CrowPort_OnDisconnect;
        }

        private async Task CrowPort_OnDisconnect()
        {
            _isConnect = false;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnConnect()
        {
            _isConnect = true;
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnReceivedData(byte[] data)
        {
            _logger.Trace($"FlueGasAnalyzer Rec:<-- {StringByteUtils.BytesToString(data)}");
            await Task.CompletedTask;
        }

        private async Task CrowPort_OnSentData(byte[] data)
        {
            _logger.Trace($"FlueGasAnalyzer Send:--> {StringByteUtils.BytesToString(data)}");
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task OpenAsync()
        {
            _isConnect = _modBusMaster.IsConnect;
            return _modBusMaster.OpenAsync();
        }

        /// <inheritdoc/>
        public async Task CloseAsync(bool closePhysicalPort)
        {
            if (closePhysicalPort) await _modBusMaster.CloseAsync(closePhysicalPort);
        }

        public async Task<Dictionary<string, string>?> Read(string addr, int tryCount = 0, CancellationToken cancelToken = default)
        {
            if (!_isConnect) throw new NotConnectedException();
            var b = new BlockList();
            b.Add(new P113B());
            Func<Task<P113B>> func = () => _modBusMaster.GetAsync<P113B>(addr, b);
            var rs = await func.ReTry(tryCount, cancelToken);
            return rs == null ? null : new Dictionary<string, string>()
            {
                { "0", rs.温度.ToString() },
                { "2", rs.压力.ToString() },
                { "4", rs.流速.ToString() },
                { "状态", rs.状态 switch
                {
                    0 => "N",
                    1 => "C",
                    2 => "M",
                    3 => "D",
                    7 => "P",
                    _ => "N"
                }},
                { "S03-FullRange", rs.温度量程上限.ToString() },
                { "S08-FullRange", rs.压力量程上限.ToString() },
                { "S08-FullRangeLow", rs.压力量程下限.ToString() },
                { "S02-FullRange", rs.流速量程上限.ToString() }
            };
        }
    }
}
