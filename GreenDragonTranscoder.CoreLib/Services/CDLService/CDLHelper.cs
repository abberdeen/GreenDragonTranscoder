using System.Globalization;
using System.Text.RegularExpressions;
using static GreenDragonTranscoder.CoreLib.Services.CDLService.CDLParameters;
namespace GreenDragonTranscoder.CoreLib.Services.CDLService
{
    public static class CDLHelper
    {
        public static string? ConvertCDLToGECFilterV1(string cdlInput)
        {
            var cdlParamers = ParseXmlCDLParameters(cdlInput);

            if (cdlParamers == null)
            {
                return null;
            }

            var gecFilter = ConvertCDLToGECFilterV1(cdlParamers);

            return gecFilter;
        }

        public static string? ConvertCDLToGECFilterV1(CDLParameters cdl)
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
            string sV = cdl.Saturation.ToString(culture);  // saturationValue
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

            // Adjust the sign of the offset based on its sign.
            // Offset (any number, nominal value is 0)
            string operatorRSO = cdl.Offset.Red < 0 ? "" : "+";
            string operatorGSO = cdl.Offset.Green < 0 ? "" : "+";
            string operatorBSO = cdl.Offset.Blue < 0 ? "" : "+";

            // https://docs.red.com/955-0196_v1.6/Content/4_Menus/Image_LUT/CDL/01_Intro_CDL.htm
            // The formula for ASC CDL color correction: pow((r(X,Y))*{rS}{operatorRSO}{rO},{rP})
            //
            // https://ffmpeg.org/ffmpeg-filters.html#geq
            var geqFilter = $"geq=\\\n" +
                     $"r='pow((r(X,Y))*{rS}{operatorRSO}{rO},{rP})+({sVC})*({wR}*r(X,Y)+{wG}*g(X,Y)+{wB}*b(X,Y))*{sV}':\\\n" +
                     $"g='pow((g(X,Y))*{gS}{operatorGSO}{gO},{gP})+({sVC})*({wR}*r(X,Y)+{wG}*g(X,Y)+{wB}*b(X,Y))*{sV}':\\\n" +
                     $"b='pow((b(X,Y))*{bS}{operatorBSO}{bO},{bP})+({sVC})*({wR}*r(X,Y)+{wG}*g(X,Y)+{wB}*b(X,Y))*{sV}'";

            return geqFilter;
        }

        public static CDLParameters ParseXmlCDLParameters(string cdlInput)
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

            return MapToCDLParameters(slopeArray, offsetArray, powerArray, saturationValue);
        }

        public static CDLParameters ParseInlineCDLParameters(string cdlInput)
        {
            var patterns = new[]
            {
              @"\((.*?)\)",
              @"(\d+\.\d+)"
            };

            var matches = patterns.SelectMany(pattern => Regex.Matches(cdlInput, pattern, RegexOptions.IgnoreCase)
                                                                .Cast<Match>()
                                                                .Select(m => m.Groups[1].Value))
                .ToArray();

            if (matches.Length != 13)
            {
                throw new ArgumentException("Invalid CDL format");
            }

            // Specify the culture with the desired decimal separator
            CultureInfo culture = CultureInfo.InvariantCulture; // Use InvariantCulture for a dot as the decimal separator

            var parseValues = matches.SelectMany(s => s.Split(',').Select(value => float.Parse(value, culture))).ToArray();

            var slopeArray = parseValues.Take(3).ToArray();
            var offsetArray = parseValues.Skip(3).Take(3).ToArray();
            var powerArray = parseValues.Skip(6).Take(3).ToArray();

            var saturationValue = parseValues.Last();

            return MapToCDLParameters(slopeArray, offsetArray, powerArray, saturationValue);
        }

        private static CDLParameters MapToCDLParameters(float[] slopeArray, float[] offsetArray, float[] powerArray, float saturationValue)
        {
            return new CDLParameters
            {
                Slope = new ColorAdjustment(slopeArray[0], slopeArray[1], slopeArray[2]),
                Offset = new ColorAdjustment(offsetArray[0], offsetArray[1], offsetArray[2]),
                Power = new ColorAdjustment(powerArray[0], powerArray[1], powerArray[2]),
                Saturation = saturationValue
            };
        }

        public static string ConvertToXmlCDL(CDLParameters cdl)
        {
            // XML template string
            string xmlTemplate = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ColorDecisionList xmlns=""urn:ASC:CDL:v1.01"">
    <ColorDecision>
        <ColorCorrection>
            <SOPNode>
                <Description />
                <Slope>{0} {1} {2}</Slope>
                <Offset>{3} {4} {5}</Offset>
                <Power>{6} {7} {8}</Power>
            </SOPNode>
            <SATNode>
                <Saturation>{9}</Saturation>
            </SATNode>
        </ColorCorrection>
    </ColorDecision>
</ColorDecisionList>";

            return FormatOutput(xmlTemplate, cdl);
        }

        public static string ConvertToInlineCDL(CDLParameters cdl)
        {
            // inline template string
            string inlineTemplate = @"({0}, {1}, {2})({3}, {4}, {5})({6}, {7}, {8}){9}";

            return FormatOutput(inlineTemplate, cdl);
        }

        private static string FormatOutput(string template, CDLParameters cdl) 
        {
            // Replace placeholders with actual values, using InvariantCulture for dot as decimal separator
            string ouput = string.Format(CultureInfo.InvariantCulture, template,
                cdl.Slope.Red.ToString("F6", CultureInfo.InvariantCulture),
                cdl.Slope.Green.ToString("F6", CultureInfo.InvariantCulture),
                cdl.Slope.Blue.ToString("F6", CultureInfo.InvariantCulture),
                cdl.Offset.Red.ToString("F6", CultureInfo.InvariantCulture),
                cdl.Offset.Green.ToString("F6", CultureInfo.InvariantCulture),
                cdl.Offset.Blue.ToString("F6", CultureInfo.InvariantCulture),
                cdl.Power.Red.ToString("F6", CultureInfo.InvariantCulture),
                cdl.Power.Green.ToString("F6", CultureInfo.InvariantCulture),
                cdl.Power.Blue.ToString("F6", CultureInfo.InvariantCulture),
                cdl.Saturation.ToString("F6", CultureInfo.InvariantCulture));

            return ouput;
        }
    }
}
