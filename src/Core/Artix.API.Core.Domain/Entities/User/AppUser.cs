namespace Artix.API.Core.Domain.Entities.User;

using Collection;
using MarketPlace;
using Microsoft.AspNetCore.Identity;

public sealed class AppUser : IdentityUser<long>
{
  public Guid BusinessId { get; private set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; private set; }
    public bool IsPro { get; set; }

    // Backing fields initialized as List<T> to support IReadOnlyCollection<T>
    private readonly List<Collection> _collections = [];
    private readonly List<MarketplaceItem> _marketplaceItems = [];
    private readonly List<Friendship> _friendshipFriends = [];
    private readonly List<UserJournalEntry> _userJournalEntries = [];
    private readonly List<UserMuseumKey> _userMuseumKeys = [];
    private readonly List<UserObject> _userObjects = [];
    private readonly List<UserSeasonProgress> _userSeasonProgresses = [];
    private readonly List<UserStrike> _userStrikes = [];
    private readonly List<UserTrack> _userTracks = [];
    private readonly List<UserXp> _userXps = [];

    // Public read-only collections
    public IReadOnlyCollection<Collection> Collections => _collections.AsReadOnly();
    public IReadOnlyCollection<MarketplaceItem> MarketplaceItems => _marketplaceItems.AsReadOnly();
    
    

    
    public IReadOnlyCollection<Friendship> FriendshipFriends => _friendshipFriends.AsReadOnly();
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


    internal void UpdateProfile(string? displayName, string? avatarUrl, bool isPro = false)
    {
        DisplayName = displayName;
        AvatarUrl = avatarUrl;
        IsPro = isPro;
        SetModified();
    }

    internal void MarkAsDeleted()
    {
        IsDeleted = true;
        SetModified();
    }

    internal void SetModified()
    {
        ModifiedAt = DateTime.UtcNow;
    }
    
    internal void AddFriendship(Friendship friendship)
    {
        _friendshipFriends.Add(friendship);
    }
}
