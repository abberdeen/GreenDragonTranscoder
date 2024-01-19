using System;
using System.Globalization;

namespace GreenDragonTranscoder.CoreLib.Services.CDLService
{
    public class CDLParameters
    {
        public class ColorAdjustment
        {
            public float Red { get; set; }
            public float Green { get; set; }
            public float Blue { get; set; }

            public ColorAdjustment()
            {
            }

            public ColorAdjustment(float r, float g, float b)
            {
                Red = r;
                Green = g;
                Blue = b;
            }
        }

        public ColorAdjustment Slope { get; set; } = new ColorAdjustment();
        public ColorAdjustment Offset { get; set; } = new ColorAdjustment();
        public ColorAdjustment Power { get; set; } = new ColorAdjustment();
        public float Saturation { get; set; } 
    }
}
