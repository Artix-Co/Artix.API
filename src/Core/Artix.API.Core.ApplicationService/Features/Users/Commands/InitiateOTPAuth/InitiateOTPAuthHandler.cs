namespace Artix.API.Core.ApplicationService.Features.Users.Commands.InitiateOTPAuth;

using Contract.Features.Users.Commands.InitiateOTPAuth;
using Domain.Entities.OTP;
using Domain.Entities.User;
using Infra.Sql.Data.DbContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Primitives;

// TODO: develop validation for this handler
internal sealed class InitiateOTPAuthHandler : CommandHandlerBase<InitiateOTPAuthCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ArtixCommandDbContext _context;


    public InitiateOTPAuthHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        ArtixCommandDbContext context) : base(httpContextAccessor, userManager)
    {
        this._userManager = userManager;
        this._context = context;
    }

    public override async Task<Guid> Handle(InitiateOTPAuthCommand command, CancellationToken cancellationToken)
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

            return otp.BusinessId;
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

            return otp.BusinessId;
        }
    }
}
