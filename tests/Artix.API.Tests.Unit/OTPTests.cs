using Artix.API.Core.Domain.Entities.OTP;
using Artix.API.Core.Domain.Entities.OTP.Enums;

namespace Artix.API.Tests.Unit;

public sealed class OtpTests
{
    [Fact]
    public void Generate_creates_a_random_six_digit_code()
    {
        var codes = Enumerable.Range(0, 100)
            .Select(_ => OTP.Generate("09120000000", PurposeType.Login).Code)
            .ToArray();

        Assert.All(codes, code =>
        {
            Assert.Equal(6, code.Length);
            Assert.True(code.All(char.IsDigit));
        });
        Assert.Contains(codes, code => code != "123456");
    }
}
