using BitMiracle.LibTiff.Classic;
using GreenDragonTranscoder.CoreLib.Services.CDLService;
using Xunit;

namespace GreenDragonTranscoder.Test
{
    public class CDLHelperTests
    {
        private const string validCDLInput = "(0.904771,0.931037, 1.011883)(0.008296, 0.017804, -0.026100)(1.052651, 1.005324, 0.945201)0.801050";

        private const string expectedGEQFilter = "geq=\\\n" +
                                   "r='pow((r(X,Y))*0.904771+0.008296,1.052651)+(1-0.80105)*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*0.80105':\\\n" +
                                   "g='pow((g(X,Y))*0.931037+0.017804,1.005324)+(1-0.80105)*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*0.80105':\\\n" +
                                   "b='pow((b(X,Y))*1.011883-0.0261,0.945201)+(1-0.80105)*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*0.80105'";
    
        private const string cdlInput = "<Slope>0.904771 0.931037 1.011883</Slope>" +
                            "<Offset>0.008296 0.017804 -0.026100</Offset> " +
                            "<Power>1.052651 1.005324 0.945201</Power> " +
                            "<Saturation>0.801050</Saturation>";
        [Fact]
        public void ConvertCDLToGEQ_WithValidInput_ShouldReturnExpectedResult()
        {
            // Arrange
            string cdlInput = "<CDL><Slope>1 1 1</Slope><Offset>0 0 0</Offset><Power>1 1 1</Power><Saturation>0.5</Saturation></CDL>";
            string expectedGEQFilter = "geq=\\\n" +
                                       "r='pow((r(X,Y))*1+0,1)+(1-0.5)*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*0.5':\\\n" +
                                       "g='pow((g(X,Y))*1+0,1)+(1-0.5)*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*0.5':\\\n" +
                                       "b='pow((b(X,Y))*1+0,1)+(1-0.5)*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*0.5'";

            // Act
            string result = CDLHelper.ConvertCDLToGECFilterV1(cdlInput);

            // Assert
            Assert.Equal(expectedGEQFilter, result);
        }

        [Fact]
        public void ConvertCDLToGEQ_WithAdditionalCDLFormat_ShouldReturnExpectedResult()
        {
            // Arrange
            // ...consts

            // Act
            string result = CDLHelper.ConvertCDLToGECFilterV1(cdlInput);

            // Assert
            Assert.Equal(expectedGEQFilter, result);
        }

        [Fact]
        public void ConvertCDLToGEQ_WithInvalidInput_ShouldReturnNull()
        {
            // Arrange
            string invalidCDLInput = "<CDL>InvalidInput</CDL>";
             
            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                string result = CDLHelper.ConvertCDLToGECFilterV1(invalidCDLInput);
            });
        }

        [Fact]
        public void ParseCDLParameters_WithValidInput_ShouldReturnCDLParametersObject()
        {
            // Arrange
            string validCDLInput = "<CDL><Slope>1 1 1</Slope><Offset>0 0 0</Offset><Power>1 1 1</Power><Saturation>0.5</Saturation></CDL>";

            // Act
            CDLParameters result = CDLHelper.ParseCDLParameters(validCDLInput);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Slope.Red);
            Assert.Equal(1, result.Slope.Green);
            Assert.Equal(1, result.Slope.Blue);
            Assert.Equal(0, result.Offset.Red);
            Assert.Equal(0, result.Offset.Green);
            Assert.Equal(0, result.Offset.Blue);
            Assert.Equal(1, result.Power.Red);
            Assert.Equal(1, result.Power.Green);
            Assert.Equal(1, result.Power.Blue);
            Assert.Equal(0.5f, result.Saturation);
        }

        [Fact]
        public void ParseCDLParameters_WithInvalidInput_ShouldReturnNull()
        {
            // Arrange
            string invalidCDLInput = "InvalidInput";

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                CDLParameters result = CDLHelper.ParseCDLParameters(invalidCDLInput);
            });
        }

        [Fact]
        public void ParseInlineCDLParameters_WithValidInput_ShouldReturnCDLParametersObject()
        {
            // Arrange
            // ..consts 

            // Act
            CDLParameters result = CDLHelper.ParseInlineCDLParameters(validCDLInput);

            // Assert
            Assert.NotNull(result);

            // Slope
            Assert.Equal(0.904771, result.Slope.Red, 6 );
            Assert.Equal(0.931037, result.Slope.Green, 6);
            Assert.Equal(1.011883, result.Slope.Blue, 6);

            // Offset
            Assert.Equal(0.008296, result.Offset.Red, 6);
            Assert.Equal(0.017804, result.Offset.Green, 6);
            Assert.Equal(-0.026100, result.Offset.Blue, 6);

            // Power
            Assert.Equal(1.052651, result.Power.Red, 6);
            Assert.Equal(1.005324, result.Power.Green, 6);
            Assert.Equal(0.945201, result.Power.Blue, 6);

            // Saturation
            Assert.Equal(0.801050, result.Saturation, 6);
        }

        [Fact]
        public void ConvertCDLToGEQ_WithInlineCDLFormat_ShouldReturnExpectedResult()
        {
            // Arrange
         
            CDLParameters cdlParameters = CDLHelper.ParseInlineCDLParameters(validCDLInput);


            // Act
            string result = CDLHelper.ConvertCDLToGECFilterV1(cdlParameters);

            // Assert
            Assert.Equal(expectedGEQFilter, result);
        }
    }
}

