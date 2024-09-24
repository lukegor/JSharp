using JSharp.Utility;

namespace JSharp.Models.SimpleDataModels
{
    public class OperationData
    {
        public OperationType Operation { get; set; }
        public double? BlendFactor1 { get; set; }

        public OperationData(OperationType operation)
        {
            Operation = operation;
            BlendFactor1 = null;
        }

        public OperationData(OperationType operation, double blendFactor1)
        {
            Operation = operation;
            BlendFactor1 = blendFactor1;
        }
    }
}
