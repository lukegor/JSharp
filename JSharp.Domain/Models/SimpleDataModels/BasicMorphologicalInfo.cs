using JSharp.Shared.Imaging;
using JSharp.Utility.Utility;

namespace JSharp.Domain.Models.SimpleDataModels
{
    public class BasicMorphologicalInfo
    {
        public MorphologicalOperationType MorphologicalOperationType { get; set; }
        public ShapeType ElementShape { get; set; }
        public BorderMode BorderType { get; set; }
        public int? ElementSize { get; set; }

        public BasicMorphologicalInfo(MorphologicalOperationType morphologicalOperationType, ShapeType elementShape, BorderMode borderType, int? elementSize)
        {
            MorphologicalOperationType = morphologicalOperationType;
            ElementShape = elementShape;
            BorderType = borderType;
            ElementSize = elementSize;
        }
    }
}
