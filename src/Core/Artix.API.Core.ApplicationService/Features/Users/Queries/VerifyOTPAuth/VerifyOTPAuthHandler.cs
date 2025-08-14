namespace Artix.API.Core.ApplicationService.Features.Users.Queries.VerifyOTPAuth;

using Contract.Features.OTPs.Commands;
using Contract.Features.OTPs.Queries;
using Contract.Features.OTPs.Queries.GetLatestByPhoneNumber;
using Primitives;
using Domain.Entities.User;
using Contract.Features.Users.Queries.VerifyOTPAuth;
using Domain.Entities.User.Enums;
using Exceptions;
using Infra.Identity.Interfaces.LoginHistory;
using Infra.Identity.Interfaces.TokenProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

// TODO: develop validator for this handler
internal sealed class VerifyOTPAuthHandler : QueryHandlerBase<GetVerifyOTPAuthQuery, VerifyOTPAuthDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IUserLoginHistoryService _userLoginHistoryService;
    private readonly IOTPCommandRepository _otpCommandRepository;
    private readonly IOTPQueryRepository _otpQueryRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public VerifyOTPAuthHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager,
        IUserLoginHistoryService userLoginHistoryService, IOTPCommandRepository otpCommandRepository,
        IOTPQueryRepository otpQueryRepository, IJwtTokenGenerator jwtTokenGenerator) : base(cache, httpContextAccessor,
        userManager)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._signInManager = signInManager;
        this._userLoginHistoryService = userLoginHistoryService;
        this._otpCommandRepository = otpCommandRepository;
        this._otpQueryRepository = otpQueryRepository;
        this._jwtTokenGenerator = jwtTokenGenerator;
        this._httpContextAccessor = httpContextAccessor;
    }

    public override async Task<VerifyOTPAuthDto> Handle(GetVerifyOTPAuthQuery query,
        CancellationToken cancellationToken)
    {
        const string ROLE = nameof(Role.Client);
        const string CLAIM = nameof(ClientType.Emerald);


        var otpDto = await this._otpQueryRepository.GetLatestByPhoneNumberAsync(
            new GetLatestOTPByPhoneNumberQuery { PhoneNumber = query.PhoneNumber, OtpCode = query.OtpCode },
            cancellationToken);

        var otp = await this._otpCommandRepository.GetByIdAsync(otpDto.Id, cancellationToken);
        if (otp == null)
        {
            throw ApplicationServiceNotFoundException.ForEntity(nameof(otp), otpDto.Id);
        }

        otp.IsUsed = true; // Mark OTP as used

        await _otpCommandRepository.UpdateAsync(otp, cancellationToken);


        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == query.PhoneNumber, cancellationToken);


        if (otp.Purpose == "Registration" && user == null)
        {
            var roleExists = await _roleManager.RoleExistsAsync(ROLE);
            if (!roleExists)
            {
                var roleCreateResult = await _roleManager.CreateAsync(new AppRole(ROLE));
                if (!roleCreateResult.Succeeded)
                    throw new ApplicationException("Failed to create Client role: " +
                                                   string.Join(", ",
                                                       roleCreateResult.Errors.Select(e => e.Description)));
            }

            // Create new user
            var newUser = new AppUser
            {
                UserName = $"user_{query.PhoneNumber}", // Generate username from phone number
                Email = $"{query.PhoneNumber}@example.com", // Placeholder email
                PhoneNumber = query.PhoneNumber,
                DisplayName = query.PhoneNumber // Use phone number as display name
            };

            var createResult = await _userManager.CreateAsync(newUser); // No password
            if (!createResult.Succeeded)
                throw new ApplicationException("User creation failed: " +
                                               string.Join(", ", createResult.Errors.Select(e => e.Description)));

            // Assign Client role
            var roleResult = await _userManager.AddToRoleAsync(newUser, ROLE);
            if (!roleResult.Succeeded)
                throw new ApplicationException("Role assignment failed: " +
                                               string.Join(", ", roleResult.Errors.Select(e => e.Description)));

            // Add ClientType claim for Emerald
            var claimResult =
                await _userManager.AddClaimAsync(newUser, new System.Security.Claims.Claim("ClientType", CLAIM));
            if (!claimResult.Succeeded)
                throw new ApplicationException("Claim assignment failed: " +
                                               string.Join(", ", claimResult.Errors.Select(e => e.Description)));

            // Record login history
            await _userLoginHistoryService.RecordLoginAsync(
                newUser,
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString()
            );

            // Generate JWT tokens
            var tokenResult = await _jwtTokenGenerator.GenerateTokensAsync(newUser, true, cancellationToken);

            // Send welcome SMS (uncomment when ready)
            var smsMessage = $"Welcome {newUser.DisplayName}! You are now registered.";
            // await _smsSender.SendAsync(newUser.PhoneNumber, smsMessage, cancellationToken);

            return new VerifyOTPAuthDto
            {
                IsNewUser = true,
                UserId = newUser.BusinessId,
                AccessToken = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                AccessTokenExpiresAt = tokenResult.AccessTokenExpiresAt,
                RefreshTokenExpiresAt = tokenResult.RefreshTokenExpiresAt
            };
        }
        else if (otp.Purpose == "Login" && user != null)
        {
            // Verify Client role and ClientType claim
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Client"))
                throw new ApplicationException("User does not have the required Client role.");

            var claims = await _userManager.GetClaimsAsync(user);
            if (!claims.Any(c => c.Type == "ClientType" && c.Value == CLAIM))
                throw new ApplicationException("User does not have the required ClientType claim: Emerald.");

            // Sign in user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Record login history
            await _userLoginHistoryService.RecordLoginAsync(
                user,
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString()
            );

            // Generate JWT tokens
            var tokenResult = await _jwtTokenGenerator.GenerateTokensAsync(user, true, cancellationToken);

            // Send welcome SMS (uncomment when ready)
            var smsMessage = $"Welcome back {user.DisplayName}! You are now logged in.";
            // await _smsSender.SendAsync(user.PhoneNumber, smsMessage, cancellationToken);

            return new VerifyOTPAuthDto
            {
                IsNewUser = false,
                UserId = user.BusinessId,
                AccessToken = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                AccessTokenExpiresAt = tokenResult.AccessTokenExpiresAt,
                RefreshTokenExpiresAt = tokenResult.RefreshTokenExpiresAt
            };
        }
        else
        {
            // TODO: clean exception
            throw new InvalidOperationException("Invalid OTP purpose or user state");
        }
    }
}
