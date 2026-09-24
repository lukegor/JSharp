using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Operations;
using JSharp.Shared.Imaging;
using JSharp.ViewModels.Abstractions;
using System.Collections.ObjectModel;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="DoubleConvolverWindow"/>.
    /// </summary>
    internal partial class DoubleConvolverWindowViewModel : DialogViewModel<DoubleConvolutionParams>
    {
        public ObservableCollection<int> FirstMatrix
        {
            get => field;
            set => SetProperty(ref field, value);
        } = new ObservableCollection<int>();
        public ObservableCollection<int> SecondMatrix
        {
            get => field;
            set => SetProperty(ref field, value);
        } = new ObservableCollection<int>();
        public ObservableCollection<int> ResultMatrix
        {
            get => field;
            set => SetProperty(ref field, value);
        } = new ObservableCollection<int>();
        public string CurrentKernel
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;
        public BorderMode BorderPixelsOption
        {
            get => field;
            set => SetProperty(ref field, value);
        }
        public int MatrixSize
        {
            get => field;
            set => SetProperty(ref field, value);
        } = 3;

        private readonly bool _isInitialized;

        public DoubleConvolverWindowViewModel()
        {
            List<int> firstList = new List<int>()
            {
                // default matrix 1
                1, 1, 1,
                1, 1, 1,
                1, 1, 1
            };
            List<int> secondList = new List<int>()
            {
                // default matrix 2
                1, -2, 1,
                -2, 4, -2,
                1, -2, 1
            };

            FirstMatrix = new ObservableCollection<int>(firstList);
            SecondMatrix = new ObservableCollection<int>(secondList);

            Update5x5Kernel(firstList, secondList);
            _isInitialized = true;
        }

        internal void KernelInputCell_TextChanged()
        {
            if (_isInitialized)
            {
                Update5x5Kernel(FirstMatrix.ToList(), SecondMatrix.ToList());
            }
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

        [RelayCommand]
        private void Confirm() => Complete(
            new DoubleConvolutionParams(
                ToMatrix(FirstMatrix.ToList(), 3),
                ToMatrix(SecondMatrix.ToList(), 3),
                0));

        private static float[,] ToMatrix(List<int> values, int size)
        {
            float[,] matrix = new float[size, size];
            int index = 0;
            foreach (int value in values)
            {
                matrix[index / size, index % size] = value;
                index++;
            }

            return matrix;
        }
    }
}
