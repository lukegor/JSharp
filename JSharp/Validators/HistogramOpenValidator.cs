﻿using Emgu.CV;
using JSharp.Resources;
using JSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Validators
{
    internal class HistogramOpenValidator : IValidator
    {
        private HistogramWindowViewModel? histogramWindowViewModel;

        public HistogramOpenValidator(HistogramWindowViewModel? histogramWindowViewModel)
        {
            this.histogramWindowViewModel = histogramWindowViewModel;
        }

        public string Validate()
        {
            if (this.histogramWindowViewModel == null)
            {
                return Strings.HistogramNotOpen;
            }
            return null; // No error message if condition is met
        }
    }
}
