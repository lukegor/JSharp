using Emgu.CV.CvEnum;
using JSharp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSharp.Resources;
using Prism.Mvvm;
using Prism.Commands;

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

        private BorderType _borderPixelsOption;
        public BorderType BorderPixelsOption
        {
            get { return _borderPixelsOption; }
            set { SetProperty(ref _borderPixelsOption, value); }
        }

        private int _borderValue;
        public int BorderValue
        {
            get { return _borderValue; }
            set { SetProperty(ref _borderValue, value); }
        }

        private int _elementSize;
        public int ElementSize
        {
            get { return _elementSize; }
            set { SetProperty(ref _elementSize, value); }
        }

        public DelegateCommand BtnConfirm_ClickCommand { get; }

        public StandardMorphologicalWindowViewModel()
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirm_Click);
            Shape = ShapeType.Rhombus;
            BorderPixelsOption = BorderType.Isolated;
            BorderValue = 0;
            ElementSize = 3;
        }

        private void BtnConfirm_Click()
        {
            var window = App.Current.Windows.OfType<StandardMorphologicalWindow>().FirstOrDefault(x => x.DataContext == this);
            window.DialogResult = true;
        }

        public IEnumerable<string> GetEdgePixelsHandlingOptions()
        {
            IEnumerable<BorderType> borderTypes = [BorderType.Isolated, BorderType.Reflect, BorderType.Replicate,
                                                    BorderType.Reflect101, BorderType.Wrap];
            return BorderTypeHelper.GetLocalizedEdgePixelsHandlingOptions(borderTypes);
        }

        public IEnumerable<string> GetShapeTypeOptions()
        {
            IEnumerable<ShapeType> shapeTypes = [ShapeType.Rhombus, ShapeType.Rectangle];
            return ShapeTypeHelper.GetLocalizedShapeTypes(shapeTypes);
        }
    }
}
