namespace JSharp.Domain.Operations
{
    public abstract record OperationParams;

    public sealed record NoParams : OperationParams
    {
        private NoParams()
        {
        }

        public static readonly NoParams Default = new();
    }
}
