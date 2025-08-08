namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Collection;
using Exceptions;
using Microsoft.AspNetCore.Identity;
using Museum;

public class AppUser : IdentityUser<long>
{
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; private set; }
    public bool IsPro { get; set; } = false;
    public bool IsVerified { get; set; } = false;


    private readonly List<Collection> _collections = [];
    private readonly List<MarketplaceItem> _marketplaceItems = [];
    private readonly List<Friendship> _friendshipFriends = [];
    private readonly List<UserJournalEntry> _userJournalEntries = [];
    private readonly List<UserMuseumKey> _userMuseumKeys = [];
    private readonly List<UserObject> _userObjects = [];
    private readonly List<UserSeasonProgress> _userSeasonProgresses = [];
    private readonly List<UserStrike> _userStrikes = [];
    
    private readonly List<UserXp> _userXps = [];

    // Public read-only collections
    public virtual IReadOnlyCollection<Collection> Collections => _collections.AsReadOnly();
    public virtual IReadOnlyCollection<MarketplaceItem> MarketplaceItems => _marketplaceItems.AsReadOnly();


    public virtual IReadOnlyCollection<Friendship> FriendshipFriends => _friendshipFriends.AsReadOnly();
    public virtual IReadOnlyCollection<UserJournalEntry> UserJournalEntries => _userJournalEntries.AsReadOnly();
    public virtual IReadOnlyCollection<UserMuseumKey> UserMuseumKeys => _userMuseumKeys.AsReadOnly();
    public virtual IReadOnlyCollection<UserObject> UserObjects => _userObjects.AsReadOnly();
    public virtual IReadOnlyCollection<UserSeasonProgress> UserSeasonProgresses => _userSeasonProgresses.AsReadOnly();
    public virtual IReadOnlyCollection<UserStrike> UserStrikes => _userStrikes.AsReadOnly();
    
    public virtual IReadOnlyCollection<UserXp> UserXps => _userXps.AsReadOnly();
    public virtual ICollection<AppUserToken> Tokens { get; set; }

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


        public AppUser Build() => _user;
    }


    internal void UpdateProfile(string? displayName, string? avatarUrl, bool isPro = false)
    {
        DisplayName = displayName;
        AvatarUrl = avatarUrl;
        IsPro = isPro;
    }


 

    internal void AddFriendship(Friendship friendship)
    {
        _friendshipFriends.Add(friendship);
    }
}
