namespace Artix.API.Core.Domain.Entities.User;

using Collection;
using File;
using Microsoft.AspNetCore.Identity;
using Object;

public class AppUser : IdentityUser<long>
{
    public string? DisplayName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsPro { get; set; } = false;
    public bool IsVerified { get; set; } = false;


    private readonly List<Collection> _collections = [];
    private readonly List<MarketplaceItem> _marketplaceItems = [];
    private readonly List<Friendship> _friendshipFriends = [];
    private readonly List<UserJournalEntry> _userJournalEntries = [];
    private readonly List<UserMuseumKey> _userMuseumKeys = [];
    private readonly List<UserScan> _userScans = [];
    private readonly List<UserSeasonProgress> _userSeasonProgresses = [];
    private readonly List<UserStrike> _userStrikes = [];
    private readonly List<UserImage> _userImages = [];

    private readonly List<UserXp> _userXps = [];
    private readonly List<UserSession> _userSessions = [];

    // Public read-only collections
    public virtual IReadOnlyCollection<UserSession> UserSessions => this._userSessions.AsReadOnly();
    public virtual IReadOnlyCollection<Collection> Collections => _collections.AsReadOnly();
    public virtual IReadOnlyCollection<MarketplaceItem> MarketplaceItems => _marketplaceItems.AsReadOnly();


    public virtual IReadOnlyCollection<Friendship> FriendshipFriends => _friendshipFriends.AsReadOnly();
    public virtual IReadOnlyCollection<UserJournalEntry> UserJournalEntries => _userJournalEntries.AsReadOnly();
    public virtual IReadOnlyCollection<UserMuseumKey> UserMuseumKeys => _userMuseumKeys.AsReadOnly();
    public virtual IReadOnlyCollection<UserScan> UserScans => this._userScans.AsReadOnly();
    public virtual IReadOnlyCollection<UserSeasonProgress> UserSeasonProgresses => _userSeasonProgresses.AsReadOnly();
    public virtual IReadOnlyCollection<UserStrike> UserStrikes => _userStrikes.AsReadOnly();
    public virtual IReadOnlyCollection<UserImage> UserImages => this._userImages.AsReadOnly();

    public virtual IReadOnlyCollection<UserXp> UserXps => _userXps.AsReadOnly();
    public virtual ICollection<AppUserToken> Tokens { get; set; }
    public Guid BusinessId { get; protected set; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;


    #region Builder

    public class AppUserBuilder
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

        public AppUserBuilder WithProfileImage(long fileId, string[] allowedMimeTypes)
        {
            _user.AssignProfileImage(fileId, allowedMimeTypes);
            return this;
        }

        public AppUser Build() => _user;
    }

    #endregion

    public void AssignProfileImage(long fileId, string[] allowedMimeTypes)
    {
        var userImage = UserImage.Create(this.Id, fileId);
        this._userImages.Add(userImage);
    }


    public void RemoveProfileImage()
    {
        var existingModel = this._userImages.FirstOrDefault(of => Is3DModel(of.FileEntity));

        if (existingModel is not null)
            this._userImages.Remove(existingModel);
    }

    public FileEntity? GetProfileImage()
    {
        return this._userImages
            .FirstOrDefault(of => Is3DModel(of.FileEntity))
            ?.FileEntity;
    }


    public bool HasProfileImage()
    {
        return this._userImages.Exists(of => Is3DModel(of.FileEntity));
    }

    private static bool Is3DModel(FileEntity fileEntity)
    {
        var allowedMimeTypes =
            new[] { "image/jpeg", "image/png", "image/webp", "image/gif", "image/bmp", "image/tiff" };

        return allowedMimeTypes.Contains(fileEntity.MimeType);
    }


    internal void UpdateProfile(string? displayName, bool isPro = false)
    {
        DisplayName = displayName;
        IsPro = isPro;
    }


    internal void AddFriendship(Friendship friendship)
    {
        _friendshipFriends.Add(friendship);
    }

    public void AddUserXp(UserXp userXp)
    {
        if (userXp == null)
            throw new ArgumentNullException(nameof(userXp));
        if (this._userXps.All(ux => ux.UserId != userXp.UserId))
            _userXps.Add(userXp);
    }

    public void AddUserSeasonProgress(UserSeasonProgress seasonProgress)
    {
        if (seasonProgress == null)
            throw new ArgumentNullException(nameof(seasonProgress));
        if (!_userSeasonProgresses.Any(sp =>
                sp.UserId == seasonProgress.UserId && sp.SeasonId == seasonProgress.SeasonId))
            _userSeasonProgresses.Add(seasonProgress);
    }

    public void ProcessScan(Object @object)
    {
        var userScan = this.UserScans.FirstOrDefault(uo => uo.UserId == this.Id && uo.ObjectId == @object.Id);

        if (userScan == null)
        {
            @object.FirstTimeUserScan(this.Id);
        }
        else
        {
            @object.RepeatUserScan(userScan);
        }
    }
}
