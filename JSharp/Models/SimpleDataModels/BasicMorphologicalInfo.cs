using Emgu.CV.CvEnum;
using JSharp.Utility;

namespace JSharp.Models.SimpleDataModels
{
    public class BasicMorphologicalInfo
    {
        public MorphologicalOperationType MorphologicalOperationType { get; set; }
        public ShapeType ElementShape { get; set; }
        public BorderType BorderType { get; set; }
        public int? ElementSize { get; set; }

        public BasicMorphologicalInfo(MorphologicalOperationType morphologicalOperationType, ShapeType elementShape, BorderType borderType, int? elementSize)
        {
            MorphologicalOperationType = morphologicalOperationType;
            ElementShape = elementShape;
            BorderType = borderType;
            ElementSize = elementSize;
        }
    }
}
