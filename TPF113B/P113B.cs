namespace TPF113B
{
    internal class P113B
    {
        [CHProtocol(0)]
        public float 温度 { get; set; }

        [CHProtocol(2)]
        public float 压力 { get; set; }

        [CHProtocol(4)]
        public float 流速 { get; set; }

        [CHProtocol(14)]
        public float 压力量程上限 { get; set; }

        [CHProtocol(16)]
        public float 压力量程下限 { get; set; }

        [CHProtocol(18)]
        public float 不要 { get; set; }

        [CHProtocol(20)]
        public float 不要1 { get; set; }

        [CHProtocol(22)]
        public float 流速量程上限 { get; set; }

        [CHProtocol(34)]
        public float 温度量程上限 { get; set; }

        [CHProtocol(85)]
        public ushort 状态 { get; set; }
    }
}
