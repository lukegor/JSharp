using JSharp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Models
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
