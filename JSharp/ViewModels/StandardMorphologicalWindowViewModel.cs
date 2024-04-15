using Emgu.CV.CvEnum;
using JSharp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSharp.Resources;
using Prism.Mvvm;

namespace JSharp.ViewModels
{
    public class StandardMorphologicalWindowViewModel : BindableBase
    {
        private ShapeType _shape;
        public ShapeType Shape
        {
            get { return _shape; }
            set { SetProperty(ref _shape, value); }
        }

        private string _borderPixelsOption;
        public string BorderPixelsOption
        {
            get { return _borderPixelsOption; }
            set { SetProperty(ref _borderPixelsOption, value); }
        }

        public StandardMorphologicalWindowViewModel()
        {
            _shape = ShapeType.Rhombus;
            _borderPixelsOption = Kernels.BorderTypeIsolated;
        }

        public IEnumerable<string> GetEdgePixelsHandlingOptions()
        {
            IEnumerable<BorderType> borderTypes = [BorderType.Isolated, BorderType.Reflect, BorderType.Replicate,
                                                    BorderType.Reflect101, BorderType.Wrap];
            return BorderTypeLocalizationHelper.GetLocalizedEdgePixelsHandlingOptions(borderTypes);
        }
    }
}
