using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenDragonTranscoder.CoreLib.Services.CDLService
{
    public class CDLParameters
    {
        public class ColorAdjustment
        {
            public float Red { get; set; }
            public float Green { get; set; }
            public float Blue { get; set; }
        }

        public class Saturation
        {
            public float Value { get; set; }
        }

        public ColorAdjustment Slope { get; set; }
        public ColorAdjustment Offset { get; set; }
        public ColorAdjustment Power { get; set; }
        public Saturation SaturationValue { get; set; }
    }
}
