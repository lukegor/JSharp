namespace JSharp.Domain.Operations
{
    public enum InputRequirement
    {
        Any,
        Grayscale,
        Color
    }

    public sealed record OperationDescriptor(
        string Id,
        string CategoryKey,
        string DisplayNameKey,
        InputRequirement Input,
        Type ParamsType);
}
