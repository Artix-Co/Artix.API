namespace Artix.API.Infra.Sql.Data.Seed;

using DbContexts;
using Microsoft.EntityFrameworkCore;

public class SqlDataRemover
{
    private readonly ArtixCommandDbContext _context;
    public SqlDataRemover(ArtixCommandDbContext context)
    {
        this._context = context;
    }
    public async Task Remove()
    {
        await this._context.Database.ExecuteSqlRawAsync(@"
    SET NOCOUNT ON;

 
    EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
 
    DELETE FROM dbo.AppVersions; DBCC CHECKIDENT ('dbo.AppVersions', RESEED, 0);
    DELETE FROM dbo.AspNetRoleClaims;
    DELETE FROM dbo.AspNetRoles; DBCC CHECKIDENT ('dbo.AspNetRoles', RESEED, 0);
    DELETE FROM dbo.AspNetUserClaims;
    DELETE FROM dbo.AspNetUserLogins;
    DELETE FROM dbo.AspNetUserRoles;
    DELETE FROM dbo.AspNetUsers; DBCC CHECKIDENT ('dbo.AspNetUsers', RESEED, 0);
    DELETE FROM dbo.AspNetUserTokens;
  
    
 
  
    DELETE FROM dbo.JournalEntries;
    DELETE FROM dbo.MarketplaceItems;
    
    
    DELETE FROM dbo.Notifications;
 
 
 
    DELETE FROM dbo.OTPs;
    DELETE FROM dbo.OutboxMessages;
  
 
    DELETE FROM dbo.UserImages;
    DELETE FROM dbo.UserJournalEntries;
    DELETE FROM dbo.UserSessions;
    DELETE FROM dbo.UserMuseumKeys;
    DELETE FROM dbo.UserNotification;
    DELETE FROM dbo.UserScans;
    DELETE FROM dbo.UserSeasonProgresses;
    DELETE FROM dbo.UserStrikes;
    DELETE FROM dbo.UserXps;
    DELETE FROM dbo.VoiceTrackFiles;
    DELETE FROM dbo.VoiceTracks;

 
    EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
");
    }
}
