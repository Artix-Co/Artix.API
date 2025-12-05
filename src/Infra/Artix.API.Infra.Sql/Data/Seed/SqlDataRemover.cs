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
    DELETE FROM dbo.CollectionItems;
    DELETE FROM dbo.Collections; DBCC CHECKIDENT ('dbo.Collections', RESEED, 0);
    DELETE FROM dbo.Files;
    DELETE FROM dbo.Friendships;
    DELETE FROM dbo.HistoricalPeriods; DBCC CHECKIDENT ('dbo.HistoricalPeriods', RESEED, 0);
    DELETE FROM dbo.JournalEntries;
    DELETE FROM dbo.MarketplaceItems;
    DELETE FROM dbo.MuseumImages;
    DELETE FROM dbo.MuseumObjects;
    DELETE FROM dbo.Museums; DBCC CHECKIDENT ('dbo.Museums', RESEED, 0);
    DELETE FROM dbo.Notifications;
    DELETE FROM dbo.ObjectHistoricalPeriods;
    DELETE FROM dbo.ObjectImages;
    DELETE FROM dbo.ObjectModels;
    DELETE FROM dbo.Objects; DBCC CHECKIDENT ('dbo.Objects', RESEED, 0);
    DELETE FROM dbo.ObjectTypes;
    DELETE FROM dbo.OTPs;
    DELETE FROM dbo.OutboxMessages;
    DELETE FROM dbo.Seasons;
    DELETE FROM dbo.SeasonTasks;
    DELETE FROM dbo.TierConfigs; DBCC CHECKIDENT ('dbo.TierConfigs', RESEED, 0);
    DELETE FROM dbo.Types; DBCC CHECKIDENT ('dbo.Types', RESEED, 0);
    DELETE FROM dbo.UserImages;
    DELETE FROM dbo.UserJournalEntries;
    DELETE FROM dbo.UserLoginHistories;
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
