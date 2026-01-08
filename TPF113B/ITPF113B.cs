using ProtocolInterface;

namespace TPF113B
{
    public interface ITPF113B : IProtocol
    {
        Task<Dictionary<string, string>?> Read(string addr, int tryCount = 0, CancellationToken cancelToken = default);
    }
}
