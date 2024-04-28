using Emgu.CV.CvEnum;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.ViewModels
{
    public class DoubleConvolverWindowViewModel : BindableBase
    {
        private ObservableCollection<int> _firstMatrix;
        public ObservableCollection<int> FirstMatrix
        {
            get { return _firstMatrix; }
            set
            {
                SetProperty(ref _firstMatrix, value);
            }
        }
        private ObservableCollection<int> _secondMatrix;
        public ObservableCollection<int> SecondMatrix
        {
            get { return _secondMatrix; }
            set { SetProperty(ref _secondMatrix, value); }
        }
        private ObservableCollection<int> _resultMatrix;
        public ObservableCollection<int> ResultMatrix
        {
            get { return _resultMatrix; }
            set { SetProperty(ref _resultMatrix, value); }
        }
        private string _currentKernel;
        public string CurrentKernel
        {
            get { return _currentKernel; }
            set { SetProperty(ref _currentKernel, value); }
        }
        private BorderType _borderPixelsOption;
        public BorderType BorderPixelsOption
        {
            get { return _borderPixelsOption; }
            set { SetProperty(ref _borderPixelsOption, value); }
        }
        private int _matrixSize = 3;
        public int MatrixSize
        {
            get { return _matrixSize; }
            set { SetProperty(ref _matrixSize, value); }
        }

        private bool isInitialized = false;

        public DelegateCommand BtnConfirm_ClickCommand { get; }
        public DoubleConvolverWindowViewModel()
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirm_Click);
            List<int> firstList = new List<int>()
            {
                // Add your values here
                1, 1, 1,
                1, 1, 1,
                1, 1, 1
            };
            List<int> secondList = new List<int>()
            {
                // Add your values here
                1, -2, 1,
                -2, 4, -2,
                1, -2, 1
            };

            // Create the ObservableCollection using the list
            FirstMatrix = new ObservableCollection<int>(firstList);
            SecondMatrix = new ObservableCollection<int>(secondList);

            Update5x5Kernel(firstList, secondList);
            isInitialized = true;
        }

        internal void KernelInputCell_TextChanged()
        {
            if (isInitialized)
                Update5x5Kernel(FirstMatrix.ToList(), SecondMatrix.ToList());
        }

        private void Update5x5Kernel(List<int> firstList, List<int> secondList)
        {
            float[,] kernel5x5 = Calculate5x5Kernel(firstList, secondList);

            ResultMatrix = ConvertToObservableCollection(kernel5x5);
        }

        private float[,] Calculate5x5Kernel(List<int> firstList, List<int> secondList)
        {
            float[,] kernel1 = ConvertListToMatrix(firstList);
            float[,] kernel2 = ConvertListToMatrix(secondList);
            float[,] kernel5x5 = new float[5, 5];

            // Convolve two 3x3 kernels to generate a 5x5 kernel
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            kernel5x5[i + k, j + l] += kernel1[i, j] * kernel2[k, l];
                        }
                    }
                }
            }

            return kernel5x5;
        }

        private float[,] ConvertListToMatrix(List<int> list)
        {
            float[,] matrix = new float[3, 3];
            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    matrix[i, j] = list[index++];
                }
            }
            return matrix;
        }

        private ObservableCollection<int> ConvertToObservableCollection(float[,] kernel)
        {
            ObservableCollection<int> collection = new ObservableCollection<int>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    collection.Add((int)kernel[i, j]);
                }
            }
            return collection;
        }

        private void BtnConfirm_Click()
        {
            var window = App.Current.Windows.OfType<DoubleConvolverWindow>().FirstOrDefault(x => x.DataContext == this);
            window.DialogResult = true;
        }

        //private float[,] Calculate5x5Kernel(float[,] kernel1, float[,] kernel2)
        //{
        //    float[,] kernel5x5 = new float[5, 5];

        //    // Wykonanie konwolucji 3x3 dwóch jąder, zapisanie wyniku w kernel5x5
        //    using (Matrix<float> mF = new Matrix<float>(kernel1))
        //    using (Matrix<float> mG = new Matrix<float>(kernel2))
        //    using (Matrix<float> mH = mG * mF)
        //    {
        //        for (int i = 0; i < mH.Rows; i++)
        //        {
        //            for (int j = 0; j < mH.Cols; j++)
        //            {
        //                kernel5x5[i, j] = mH[i, j];
        //            }
        //        }
        //    }

        //    return kernel5x5;
        //}
    }
}
