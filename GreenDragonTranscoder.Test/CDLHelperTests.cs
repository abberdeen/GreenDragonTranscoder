using GreenDragonTranscoder.CoreLib.Services.CDLService;
using Xunit;

namespace GreenDragonTranscoder.Test
{
    public class CDLHelperTests
    {
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
            string result = CDLHelper.ConvertCDLToGEQ(cdlInput);

            // Assert
            Assert.Equal(expectedGEQFilter, result);
        }

        [Fact]
        public void ConvertCDLToGEQ_WithAdditionalCDLFormat_ShouldReturnExpectedResult()
        {
            // Arrange
            string cdlInput = "<Slope>0.904771 0.931037 1.011883</Slope>" +
                              "<Offset>0.008296 0.017804 -0.026100</Offset> " +
                              "<Power>1.052651 1.005324 0.945201</Power> " +
                              "<Saturation>0.801050</Saturation>";

            string expectedGEQFilter = "geq=\\\n" +
                                       "r='pow((r(X,Y))*0.904771+0.008296,1.052651)+(1-0.801050)*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*0.801050':\\\n" +
                                       "g='pow((g(X,Y))*0.931037+0.017804,1.005324)+(1-0.801050)*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*0.801050':\\\n" +
                                       "b='pow((b(X,Y))*1.011883-0.026100,0.945201)+(1-0.801050)*(0.2126*r(X,Y)+0.7152*g(X,Y)+0.0722*b(X,Y))*0.801050'";

            // Act
            string result = CDLHelper.ConvertCDLToGEQ(cdlInput);

            // Assert
            Assert.Equal(expectedGEQFilter, result);
        }

        [Fact]
        public void ConvertCDLToGEQ_WithInvalidInput_ShouldReturnNull()
        {
            // Arrange
            string invalidCDLInput = "<CDL>InvalidInput</CDL>";

            // Act
            string result = CDLHelper.ConvertCDLToGEQ(invalidCDLInput);

            // Assert
            Assert.Null(result);
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
            Assert.Equal(0.5f, result.SaturationValue.Value);
        }

        [Fact]
        public void ParseCDLParameters_WithInvalidInput_ShouldReturnNull()
        {
            // Arrange
            string invalidCDLInput = "InvalidInput";

            // Act
            CDLParameters result = CDLHelper.ParseCDLParameters(invalidCDLInput);

            // Assert
            Assert.Null(result);
        }
    }
}
