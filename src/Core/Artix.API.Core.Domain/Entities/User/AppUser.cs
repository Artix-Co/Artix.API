namespace Artix.API.Core.Domain.Entities.User;

using Collection;
using MarketPlace;
using Microsoft.AspNetCore.Identity;

public sealed class AppUser : IdentityUser<long>
{
    private readonly List<Collection> _collections = new();
    private readonly List<Friendship> _friendshipFriends = new();
    private readonly List<Friendship> _friendshipUsers = new();
    private readonly List<MarketplaceItem> _marketplaceItems = new();
    private readonly List<UserJournalEntry> _userJournalEntries = new();
    private readonly List<UserMuseumKey> _userMuseumKeys = new();
    private readonly List<UserObject> _userObjects = new();
    private readonly List<UserSeasonProgress> _userSeasonProgresses = new();
    private readonly List<UserStrike> _userStrikes = new();
    private readonly List<UserTrack> _userTracks = new();
    private readonly List<UserXp> _userXps = new();

    public Guid BusinessId { get; private set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; private set; }
    public bool IsPro { get; set; }

    public IReadOnlyCollection<Collection> Collections => _collections.AsReadOnly();
    public IReadOnlyCollection<Friendship> FriendshipFriends => _friendshipFriends.AsReadOnly();
    public IReadOnlyCollection<Friendship> FriendshipUsers => _friendshipUsers.AsReadOnly();
    public IReadOnlyCollection<MarketplaceItem> MarketplaceItems => _marketplaceItems.AsReadOnly();
    public IReadOnlyCollection<UserJournalEntry> UserJournalEntries => _userJournalEntries.AsReadOnly();
    public IReadOnlyCollection<UserMuseumKey> UserMuseumKeys => _userMuseumKeys.AsReadOnly();
    public IReadOnlyCollection<UserObject> UserObjects => _userObjects.AsReadOnly();
    public IReadOnlyCollection<UserSeasonProgress> UserSeasonProgresses => _userSeasonProgresses.AsReadOnly();
    public IReadOnlyCollection<UserStrike> UserStrikes => _userStrikes.AsReadOnly();
    public IReadOnlyCollection<UserTrack> UserTracks => _userTracks.AsReadOnly();
    public IReadOnlyCollection<UserXp> UserXps => _userXps.AsReadOnly();

    
    
    public sealed class AppUserBuilder
    {
        private readonly AppUser _user;

        public AppUserBuilder(AppUser user)
        {
            _user = user;
        }

        public AppUserBuilder WithUsername(string? username)
        {
            if (!string.IsNullOrWhiteSpace(username))
                _user.UserName = username;

            return this;
        }

        public AppUserBuilder WithEmail(string? email)
        {
            if (!string.IsNullOrWhiteSpace(email))
                _user.Email = email;

            return this;
        }

        public AppUserBuilder WithPhoneNumber(string? phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber))
                _user.PhoneNumber = phoneNumber;

            return this;
        }

        public AppUserBuilder WithDisplayName(string? displayName)
        {
            if (!string.IsNullOrWhiteSpace(displayName))
                _user.DisplayName = displayName;

            return this;
        }

        public AppUserBuilder WithModifiedAt(DateTime? modifiedAt = null)
        {
            _user.GetType()
                .GetProperty(nameof(ModifiedAt))?
                .SetValue(_user, modifiedAt ?? DateTime.UtcNow);

            return this;
        }

        public AppUser Build() => _user;
    }


    public void UpdateProfile(string? displayName, string? avatarUrl, bool isPro = false)
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
