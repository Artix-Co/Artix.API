using Artix.API.Core.Domain.Entities.OTP;
using Artix.API.Core.Domain.Entities.OTP.Enums;

namespace Artix.API.Tests.Unit;

public sealed class OtpTests
{
    [Fact]
    public void Generate_creates_unique_hashed_codes()
    {
        var codes = Enumerable.Range(0, 100)
            .Select(_ => OTP.Generate("09120000000", PurposeType.Login).Code)
            .ToArray();

        Assert.All(codes, code =>
        {
            Assert.Equal(64, code.Length);
            Assert.True(code.All(Uri.IsHexDigit));
        });
        Assert.Equal(codes.Length, codes.Distinct(StringComparer.Ordinal).Count());
    }
}
