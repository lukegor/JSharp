namespace JSharp.Shared.Pro;

public interface IProGate
{
    bool IsPro { get; }
    string ProMessage { get; }
    void ThrowIfOpen(string feature);
}
