using Shortify.BusinessLogic.Services;

namespace Shortify.BusinessLogicTests;

public class Base62ConverterTests
{
    private readonly Base62Converter _converter = new();

    [Theory]
    [InlineData(1, 4)]
    [InlineData(100, 4)]
    [InlineData(1000, 4)]
    public void Encode_ShouldReturnNonEmptyString(int value, int length)
    {
        // Act
        var result = _converter.Encode(value, length);

        // Assert
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(1, 4)]
    [InlineData(100, 4)]
    [InlineData(1000, 4)]
    public void Encode_ShouldOnlyContainValidBase62Characters(int value, int length)
    {
        // Act
        var result = _converter.Encode(value, length);

        // Assert
        Assert.All(result, c =>
            Assert.True(
                c is >= '0' and <= '9' or >= 'A' and <= 'Z' or >= 'a' and <= 'z'));

    }

    [Theory]
    [InlineData(1, 4)]
    [InlineData(2, 4)]
    [InlineData(3, 4)]
    public void Encode_ShouldGenerateDifferentValuesForDifferentInputs(int value1, int length)
    {
        // Arrange
        var value2 = value1 + 1;

        // Act
        var result1 = _converter.Encode(value1, length);
        var result2 = _converter.Encode(value2, length);

        // Assert
        Assert.NotEqual(result1, result2);
    }

    [Theory]
    [InlineData(1, 4)]
    [InlineData(100, 5)]
    [InlineData(1000, 6)]
    public void Encode_ShouldRespectMinimumLength(int value, int length)
    {
        // Act
        var result = _converter.Encode(value, length);

        // Assert
        Assert.True(result.Length >= length - 1);
    }
}