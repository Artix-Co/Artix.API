namespace Artix.API.Core.ApplicationService.Features.Users.Commands.InitiateOTPAuth;

using Contract.Features.Users.Commands.InitiateOTPAuth;
using Domain.Entities.User;
using Infra.Sql.Data.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Primitives;

internal sealed class InitiateOTPAuthHandler : CommandHandlerBase<InitiateOTPAuthCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ArtixCommandDbContext _context;
    // private readonly ISmsSender _smsSender;

    public InitiateOTPAuthHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ArtixCommandDbContext context
        // ISmsSender smsSender
        
        )
        : base(httpContextAccessor)
    {
        _userManager = userManager;
        _context = context;
        // _smsSender = smsSender;
    }

    public override async Task<long> Handle(InitiateOTPAuthCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == command.PhoneNumber, cancellationToken);

        if (user == null)
        {
            // New user: send registration OTP
            var otp = OTP.Create(command.PhoneNumber, "Registration");
            _context.OTPs.Add(otp);
            await _context.SaveChangesAsync(cancellationToken);

            var smsMessage = $"Your registration OTP is {otp.Code}. It expires in 5 minutes.";
            // await _smsSender.SendAsync(command.PhoneNumber, smsMessage, cancellationToken);

            return otp.Id;
        }
        else
        {
            // Existing user: check for Client role and send login OTP
            var roles = await _userManager.GetRolesAsync(user);
       
            var otp = OTP.Create(command.PhoneNumber, "Login");
            _context.OTPs.Add(otp);
            await _context.SaveChangesAsync(cancellationToken);

            var smsMessage = $"Your login OTP is {otp.Code}. It expires in 5 minutes.";
            // await _smsSender.SendAsync(command.PhoneNumber, smsMessage, cancellationToken);

            return otp.Id;
        }
    }
}
