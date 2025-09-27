using HotkeyTyper;
using Xunit;

namespace HotkeyTyper.Tests;

public class EscapeForSendKeysTests
{
    [Theory]
    [InlineData('+', "{+}")]
    [InlineData('^', "{^}")]
    [InlineData('%', "{%}")]
    [InlineData('~', "{~}")]
    [InlineData('(', "{(}")]
    [InlineData(')', "{)}")]
    [InlineData('{', "{{}")]
    [InlineData('}', "{}}")]
    [InlineData('\t', "{TAB}")]
    [InlineData('\n', "{ENTER}")]
    [InlineData('A', "A")]
    public void Escapes_Expected(char input, string expected)
    {
        var result = Form1.EscapeForSendKeys(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CarriageReturn_Ignored()
    {
        var result = Form1.EscapeForSendKeys('\r');
        Assert.Equal(string.Empty, result);
    }
}
