using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GreenDragonTranscoder.CoreLib.Services.CDLService
{
    public static class CDLHelper
    {
        public static string ConvertCDLToGEQ(string cdlInput) 
        {
            var cdlParamers = ParseCDLParameters(cdlInput);
            
            if (cdlParamers == null) 
            {
                return null;
            }

            var gecFilter = ConvertCDLToGEQ(cdlParamers);

            return gecFilter;
        }

        public static string ConvertCDLToGEQ(CDLParameters cdl)
        {
            if (cdl == null)
            {
                return null;
            }

            CultureInfo culture = CultureInfo.InvariantCulture; // Use InvariantCulture for dot as the decimal separator

            var geqFilter = $"geq=\\\n" +
                            $"r='pow((r(X,Y))*{cdl.Slope.Red.ToString(culture)}+{cdl.Offset.Red.ToString(culture)},{cdl.Power.Red.ToString(culture)})+(1-{cdl.SaturationValue.Value.ToString(culture)})*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*{cdl.SaturationValue.Value.ToString(culture)}':\\\n" +
                            $"g='pow((g(X,Y))*{cdl.Slope.Green.ToString(culture)}+{cdl.Offset.Green.ToString(culture)},{cdl.Power.Green.ToString(culture)})+(1-{cdl.SaturationValue.Value.ToString(culture)})*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*{cdl.SaturationValue.Value.ToString(culture)}':\\\n" +
                            $"b='pow((b(X,Y))*{cdl.Slope.Blue.ToString(culture)}+{cdl.Offset.Blue.ToString(culture)},{cdl.Power.Blue.ToString(culture)})+(1-{cdl.SaturationValue.Value.ToString(culture)})*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*{cdl.SaturationValue.Value.ToString(culture)}'";

            return geqFilter; 
        }
         
        public static CDLParameters ParseCDLParameters(string cdlInput)
        {
            var patterns = new[]
            {
                @"<Slope>(.*?)<\/Slope>",
                @"<Offset>(.*?)<\/Offset>",
                @"<Power>(.*?)<\/Power>",
                @"<Saturation>(.*?)<\/Saturation>"
            };

            var matches = patterns.Select(pattern => Regex.Match(cdlInput, pattern, RegexOptions.IgnoreCase)).ToArray();

            if (matches.Any(match => !match.Success))
            {
                Console.WriteLine("Invalid CDL format");
                return null;
            }

            var values = matches.Select(match => match.Groups[1].Value).ToArray();

            // Specify the culture with the desired decimal separator
            CultureInfo culture = CultureInfo.InvariantCulture; // Use InvariantCulture for a dot as the decimal separator

            var slopeArray = values[0].Split(' ').Select(s => float.Parse(s, culture)).ToArray();
            var offsetArray = values[1].Split(' ').Select(s => float.Parse(s, culture)).ToArray();
            var powerArray = values[2].Split(' ').Select(s => float.Parse(s, culture)).ToArray();

            var saturationValue = float.Parse(values[3], culture);

            return new CDLParameters
            {
                Slope = new CDLParameters.ColorAdjustment
                {
                    Red = slopeArray[0],
                    Green = slopeArray[1],
                    Blue = slopeArray[2]
                },
                Offset = new CDLParameters.ColorAdjustment
                {
                    Red = offsetArray[0],
                    Green = offsetArray[1],
                    Blue = offsetArray[2]
                },
                Power = new CDLParameters.ColorAdjustment
                {
                    Red = powerArray[0],
                    Green = powerArray[1],
                    Blue = powerArray[2]
                },
                SaturationValue = new CDLParameters.Saturation
                {
                    Value = saturationValue
                }
            };
        } 
    }
}
