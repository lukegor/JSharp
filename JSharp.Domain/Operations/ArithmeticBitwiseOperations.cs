using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed record BinaryImageParams(Mat Second) : OperationParams;

    public sealed record BlendParams(Mat Second, double Weight1) : OperationParams;

    public abstract class BinaryOperationBase<TParams> : IImageOperation<TParams>
        where TParams : OperationParams
    {
        public abstract OperationDescriptor Descriptor { get; }

        public Mat Apply(Mat source, TParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            ApplyCore(source, parameters, result);
            return result;
        }

        protected abstract void ApplyCore(Mat source, TParams parameters, Mat destination);
    }

    public sealed class AddOperation : BinaryOperationBase<BinaryImageParams>
    {
        public override OperationDescriptor Descriptor { get; } =
            new("calc.add", "calculator", "Add Images", InputRequirement.Any, typeof(BinaryImageParams));

        protected override void ApplyCore(Mat source, BinaryImageParams parameters, Mat destination) =>
            Cv2.Add(source, parameters.Second, destination);
    }

    public sealed class SubtractOperation : BinaryOperationBase<BinaryImageParams>
    {
        public override OperationDescriptor Descriptor { get; } =
            new("calc.subtract", "calculator", "Subtract Images", InputRequirement.Any, typeof(BinaryImageParams));

        protected override void ApplyCore(Mat source, BinaryImageParams parameters, Mat destination) =>
            Cv2.Subtract(source, parameters.Second, destination);
    }

    public sealed class BlendOperation : BinaryOperationBase<BlendParams>
    {
        public override OperationDescriptor Descriptor { get; } =
            new("calc.blend", "calculator", "Blend Images", InputRequirement.Any, typeof(BlendParams));

        protected override void ApplyCore(Mat source, BlendParams parameters, Mat destination) =>
            Cv2.AddWeighted(source, parameters.Weight1, parameters.Second, 1 - parameters.Weight1, 0.0, destination);
    }

    public sealed class BitwiseAndOperation : BinaryOperationBase<BinaryImageParams>
    {
        public override OperationDescriptor Descriptor { get; } =
            new("calc.and", "calculator", "Bitwise AND", InputRequirement.Any, typeof(BinaryImageParams));

        protected override void ApplyCore(Mat source, BinaryImageParams parameters, Mat destination) =>
            Cv2.BitwiseAnd(source, parameters.Second, destination);
    }

    public sealed class BitwiseOrOperation : BinaryOperationBase<BinaryImageParams>
    {
        public override OperationDescriptor Descriptor { get; } =
            new("calc.or", "calculator", "Bitwise OR", InputRequirement.Any, typeof(BinaryImageParams));

        protected override void ApplyCore(Mat source, BinaryImageParams parameters, Mat destination) =>
            Cv2.BitwiseOr(source, parameters.Second, destination);
    }

    public sealed class BitwiseXorOperation : BinaryOperationBase<BinaryImageParams>
    {
        public override OperationDescriptor Descriptor { get; } =
            new("calc.xor", "calculator", "Bitwise XOR", InputRequirement.Any, typeof(BinaryImageParams));

        protected override void ApplyCore(Mat source, BinaryImageParams parameters, Mat destination) =>
            Cv2.BitwiseXor(source, parameters.Second, destination);
    }

    public sealed class BitwiseNotCalcOperation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("calc.not", "calculator", "Bitwise NOT", InputRequirement.Any, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.BitwiseNot(source, result);
            return result;
        }
    }
}
