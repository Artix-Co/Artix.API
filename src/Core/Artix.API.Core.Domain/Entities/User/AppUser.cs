namespace Artix.API.Core.Domain.Entities.User;

using _primitives;
using Collection;
using MarketPlace;
using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser<long>
{
    public Guid BusinessId { get; private set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; private set; }
    public bool? IsPro { get; set; }

    public virtual ICollection<Collection> Collections { get; private set; } = new List<Collection>();
    public virtual ICollection<Friendship> FriendshipFriends { get; private set; } = new List<Friendship>();
    public virtual ICollection<Friendship> FriendshipUsers { get; private set; } = new List<Friendship>();
    public virtual ICollection<MarketplaceItem> MarketplaceItems { get; private set; } = new List<MarketplaceItem>();

    public virtual ICollection<UserJournalEntry> UserJournalEntries { get; private set; } =
        new List<UserJournalEntry>();

    public virtual ICollection<UserMuseumKey> UserMuseumKeys { get; private set; } = new List<UserMuseumKey>();
    public virtual ICollection<UserObject> UserObjects { get; private set; } = new List<UserObject>();

    public virtual ICollection<UserSeasonProgress> UserSeasonProgresses { get; private set; } =
        new List<UserSeasonProgress>();

    public virtual ICollection<UserStrike> UserStrikes { get; private set; } = new List<UserStrike>();
    public virtual ICollection<UserTrack> UserTracks { get; private set; } = new List<UserTrack>();
    public virtual ICollection<UserXp> UserXps { get; private set; } = new List<UserXp>();

    
    
    public void UpdateProfile(string? displayName, string? avatarUrl, bool? isPro)
    {
        DisplayName = displayName;
        AvatarUrl = avatarUrl;
        IsPro = isPro;
        SetModified();
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        SetModified();
    }

    public void SetModified()
    {
        ModifiedAt = DateTime.UtcNow;
    }
}
