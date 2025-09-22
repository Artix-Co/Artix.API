namespace Artix.API.Tests.Integration.Core.DomainService.XPRules;

using Artix.API.Core.Contract.Features.Objects.Commands;
using Artix.API.Core.Domain.Entities.User;
using Artix.API.Core.DomainService.Interfaces.TierCalculator;
using Artix.API.Core.DomainService.Interfaces.XPRules;
using Artix.API.Core.DomainService.Services.XPRules;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Object = Artix.API.Core.Domain.Entities.Object.Object;

[TestFixture]
public class XpRulesServiceTests
{
    private Mock<IObjectCommandRepository> _objectRepoMock;
    private Mock<UserManager<AppUser>> _userManagerMock;
    private Mock<ITierCalculatorService> _tierCalcMock;
    private IXpRulesService _xpRulesService;

    [SetUp]
    public void Setup()
    {
        this._objectRepoMock = new Mock<IObjectCommandRepository>();
        
        var store = new Mock<IUserStore<AppUser>>().Object;
        _userManagerMock = new Mock<UserManager<AppUser>>(
            store,
            null,    // IOptions<IdentityOptions>
            null,    // IPasswordHasher<AppUser>
            new IUserValidator<AppUser>[0],
            new IPasswordValidator<AppUser>[0],
            null,    // ILookupNormalizer
            new IdentityErrorDescriber(),
            null,    // IServiceProvider
            new Mock<ILogger<UserManager<AppUser>>>().Object
        );        this._tierCalcMock = new Mock<ITierCalculatorService>();
        this._xpRulesService = new XpRulesService(this._objectRepoMock.Object, this._userManagerMock.Object, this._tierCalcMock.Object);
    }

    // Arrange, Act, Assert comments above each test
    // Test 1: Regular object first scan
    [Test]
    public async Task CalculateXpForFirstScanAsync_RegularObject_AddsExpectedXpToUserXp()
        // Arrange
    {
        var userId = 8L;
        var objectId = 6L;
        var user = new AppUser { Id = userId };
        // user.ProcessScan(objectId);
       
        
        this._userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        
        // this._objectRepoMock.Setup(r => r.GetByIdAsync(new Guid("01990A2F-8557-76CB-AABE-9CB22CA47B28"), It.IsAny<CancellationToken>()))
        //     .ReturnsAsync(obj);
        
        this._tierCalcMock.Setup(t => t.CalculateTierAsync(It.IsAny<UserScan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((1, 1.5));
        
        this._userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await this._xpRulesService.CalculateXpForFirstScanAsync(userId, new Guid("01990A2F-8557-76CB-AABE-9CB22CA47B28"));

        // Assert
        Assert.Equals((long)((100 + 2 * 10) * 1.5), user.UserXps.First().TotalXp);
        
        Assert.That(user.UserXps.First().LastUpdated, Is.GreaterThan(DateTime.MinValue));

    }

    // Test 2: Special object first scan
    [Test]
    public async Task CalculateXpForFirstScanAsync_SpecialObject_AddsExpectedXpToUserXp()
        // Arrange
    {
        var userId = 1L;
        var objectId = 2L;
        var user = new AppUser { Id = userId };
        // user.ProcessScan(objectId);
        var obj = Object.Create("Special", "QR002");


        this._userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        this._objectRepoMock.Setup(r => r.GetByIdAsync(Guid.NewGuid(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(obj);
        this._tierCalcMock.Setup(t => t.CalculateTierAsync(It.IsAny<UserScan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((2, 2.0));
        this._userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await this._xpRulesService.CalculateXpForFirstScanAsync(userId, Guid.NewGuid());

        // Assert
        Assert.Equals((long)((150 + 1 * 10) * 2.0), user.UserXps.First().TotalXp);
        Assert.That(user.UserXps.First().LastUpdated, Is.GreaterThan(DateTime.MinValue));
    }

    // Test 3: Golden level repeat scan
    [Test]
    public async Task CalculateXpForRepeatScanAsync_GoldenLevel_UpgradesScanAndAddsXp()
        // Arrange
    {
        var userId = 1L;
        var objectId = 2L;
        var user = new AppUser { Id = userId };
        var scan = UserScan.Create(userId, objectId);
        // user.ProcessScan(objectId);
        var obj = Object.Create("Test", "QR003");


        this._userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        this._objectRepoMock.Setup(r => r.GetByIdAsync(Guid.NewGuid(), It.IsAny<CancellationToken>())).ReturnsAsync(obj);
        this._tierCalcMock.Setup(t => t.CalculateTierAsync(It.IsAny<UserScan>(), It.IsAny<CancellationToken>())).ReturnsAsync((1, 1.2));
        this._userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await this._xpRulesService.CalculateXpForRepeatScanAsync(userId, Guid.NewGuid(), isGoldenLevel: true);

        // Assert
        var userXp = user.UserXps.First();
        Assert.Equals(((200 + 3 * 5) * 1.2), userXp.TotalXp);
        Assert.Equals(2, scan.ScanCount);
    }

    // Test 4: User not found on first scan
    [Test]
    public void CalculateXpForFirstScanAsync_UserNotFound_ThrowsException()
        // Arrange
    {
        this._userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync((AppUser)null!);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await this._xpRulesService.CalculateXpForFirstScanAsync(1, Guid.NewGuid()));
        Assert.Equals("User not found.", ex.Message);
    }

    // Test 5: First scan with season updates season progress
    [Test]
    public async Task CalculateXpForFirstScanAsync_WithSeason_AddsXpToSeasonProgress()
        // Arrange
    {
        var userId = 1L;
        var objectId = 2L;
        var seasonId = 1L;
        var user = new AppUser { Id = userId };
        // user.ProcessScan(objectId);
        var obj = Object.Create("Regular", "QR004");
        this._userManagerMock.Setup(u => u.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        this._objectRepoMock.Setup(r => r.GetByIdAsync(Guid.NewGuid(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(obj);
        this._tierCalcMock.Setup(t => t.CalculateTierAsync(It.IsAny<UserScan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((1, 1.5));
        this._userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await this._xpRulesService.CalculateXpForFirstScanAsync(userId, Guid.NewGuid(), seasonId);

        // Assert
        Assert.Equals((int)((100 + 2 * 10) * 1.5), user.UserSeasonProgresses.First().TotalXp);
    }
}
