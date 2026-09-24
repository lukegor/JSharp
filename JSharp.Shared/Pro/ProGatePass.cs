namespace JSharp.Shared.Pro;

public sealed class ProGatePass : IProGate
{
    public bool IsPro => true;
    public string ProMessage => string.Empty;
    public void ThrowIfOpen(string feature)
    {
        // Pro context: every feature is licensed, never throw.
    }
}
