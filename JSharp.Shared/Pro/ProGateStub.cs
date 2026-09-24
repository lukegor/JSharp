namespace JSharp.Shared.Pro;

public sealed class ProGateStub : IProGate
{
    public bool IsPro => false;
    public string ProMessage => "Available in Pro trial exe — single-image ops work here; batch/validate/measurements/CLI are Pro.";
    public void ThrowIfOpen(string feature) =>
        throw new NotSupportedException($"{feature}: {ProMessage}");
}
