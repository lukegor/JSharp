using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Models
{
    public class TwoParamsVMInfo
    {
        public SliderProperties Slider1Properties { get; set; }
        public SliderProperties Slider2Properties { get; set; }
        public string TxbText { get; set; }

        public TwoParamsVMInfo(SliderProperties slider1Properties, SliderProperties slider2Properties, string txbText)
        {
            Slider1Properties = slider1Properties;
            Slider2Properties = slider2Properties;
            TxbText = txbText;
        }
    }
}
