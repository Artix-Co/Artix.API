namespace Artix.API.Core.ApplicationService.Features.Users.Queries.VerifyOTPAuth;

using Primitives;
using Domain.Entities.User;
using Infra.Sql.Data.DbContexts;
using Contract.Features.Users.Queries.VerifyOTPAuth;
using DomainService.Users;
using DomainService.Users.LoginHistory;
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
    private readonly ArtixCommandDbContext _context;
    private readonly IUserLoginHistoryService _userLoginHistoryService;
    // private readonly ISmsSender _smsSender;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public VerifyOTPAuthHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, ArtixCommandDbContext context, IUserLoginHistoryService userLoginHistoryService, IJwtTokenGenerator jwtTokenGenerator) : base(cache, httpContextAccessor, userManager)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._signInManager = signInManager;
        this._context = context;
        this._userLoginHistoryService = userLoginHistoryService;
        this._jwtTokenGenerator = jwtTokenGenerator;
        this._httpContextAccessor = httpContextAccessor;
    }

    public override async Task<VerifyOTPAuthDto> Handle(GetVerifyOTPAuthQuery query,
        CancellationToken cancellationToken)
    {
        // Find the latest OTP (Registration or Login)
        var otp = await _context.OTPs
            .Where(o => o.PhoneNumber == query.PhoneNumber)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp == null || !otp.IsValid(query.OtpCode))
            throw new InvalidOperationException("Invalid or expired OTP");

        otp.IsUsed = true; // Mark OTP as used
        await _context.SaveChangesAsync(cancellationToken);

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == query.PhoneNumber, cancellationToken);

        if (otp.Purpose == "Registration" && user == null)
        {
            // Ensure Client role exists
            const string clientRole = "Zomorrod_Client";
            var roleExists = await _roleManager.RoleExistsAsync(clientRole);
            if (!roleExists)
            {
                var roleCreateResult = await _roleManager.CreateAsync(new AppRole(clientRole));
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

            var roleResult = await _userManager.AddToRoleAsync(newUser, clientRole);
            if (!roleResult.Succeeded)
                throw new ApplicationException("Role assignment failed: " +
                                               string.Join(", ", roleResult.Errors.Select(e => e.Description)));
          
            
            await _userLoginHistoryService.RecordLoginAsync(
                newUser,
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString()
            );
            var tokenString = await _jwtTokenGenerator.GenerateTokenAsync(newUser);
            
            var smsMessage = $"Welcome {newUser.DisplayName}! You are now registered.";
            // await _smsSender.SendAsync(newUser.PhoneNumber, smsMessage, cancellationToken);
            
  
            
            return new VerifyOTPAuthDto { IsNewUser = true, UserId = newUser.Id, Token = tokenString };
        }
        else if (otp.Purpose == "Login" && user != null)
        {
            // Verify Client role
            var roles = await _userManager.GetRolesAsync(user);
           
            // Sign in and generate JWT token
            await _signInManager.SignInAsync(user, isPersistent: false);
            
            
            
            await _userLoginHistoryService.RecordLoginAsync(
                user,
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString()
            );
            
            
            var tokenString = await _jwtTokenGenerator.GenerateTokenAsync(user);
            
            var smsMessage = $"Welcome {user.DisplayName}! You are now registered.";
            // await _smsSender.SendAsync(newUser.PhoneNumber, smsMessage, cancellationToken);
            
            
            return new VerifyOTPAuthDto { IsNewUser = false, UserId = user.Id, Token = tokenString };
        }
        else
        {
            throw new InvalidOperationException("Invalid OTP purpose or user state");
        }
    }
}
