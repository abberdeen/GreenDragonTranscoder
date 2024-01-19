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
        public static string ConvertCDLToGEQ_FX1(string cdlInput) 
        {
            var cdlParamers = ParseCDLParameters(cdlInput);
            
            if (cdlParamers == null) 
            {
                return null;
            }

            var gecFilter = ConvertCDLToGEQ_FX1(cdlParamers);

            return gecFilter;
        }

        public static string ConvertCDLToGEQ_FX1(CDLParameters cdl)
        {
            if (cdl == null)
            {
                return null;
            }

            CultureInfo culture = CultureInfo.InvariantCulture; // Use InvariantCulture for dot as the decimal separator

            // Extract constants as strings
            string wR = "0.2126";  // weightRed
            string wG = "0.7152";  // weightGreen
            string wB = "0.0722";  // weightBlue

            // Extract static numbers as strings
            string sV = cdl.SaturationValue.Value.ToString(culture);  // saturationValue
            string sVC = $"1-{sV}";  // saturationValueComplement
            string rS = cdl.Slope.Red.ToString(culture);  // redSlope
            string rO = cdl.Offset.Red.ToString(culture);  // redOffset
            string rP = cdl.Power.Red.ToString(culture);  // redPower
            string gS = cdl.Slope.Green.ToString(culture);  // greenSlope
            string gO = cdl.Offset.Green.ToString(culture);  // greenOffset
            string gP = cdl.Power.Green.ToString(culture);  // greenPower
            string bS = cdl.Slope.Blue.ToString(culture);  // blueSlope
            string bO = cdl.Offset.Blue.ToString(culture);  // blueOffset
            string bP = cdl.Power.Blue.ToString(culture);  // bluePower

            // Adjust the sign of the offset based on its sign
            string operatorRSO = cdl.Offset.Red < 0 ? "" : "+";
            string operatorGSO = cdl.Offset.Green < 0 ? "" : "+";
            string operatorBSO = cdl.Offset.Blue < 0 ? "" : "+";

            var geqFilter = $"geq=\\\n" +
                     $"r='pow((r(X,Y))*{rS}{operatorRSO}{rO},{rP})+({sVC})*({wR}*r(X,Y)+{wG}*g(X,Y)+{wB}*b(X,Y))*{sV}':\\\n" +
                     $"g='pow((g(X,Y))*{gS}{operatorGSO}{gO},{gP})+({sVC})*({wR}*r(X,Y)+{wG}*g(X,Y)+{wB}*b(X,Y))*{sV}':\\\n" +
                     $"b='pow((b(X,Y))*{bS}{operatorBSO}{bO},{bP})+({sVC})*({wR}*r(X,Y)+{wG}*g(X,Y)+{wB}*b(X,Y))*{sV}'";

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
                throw new ArgumentException("Invalid CDL format");
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
