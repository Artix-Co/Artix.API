namespace Artix.API.Core.Domain.Entities.User;

using File;

public class UserImage
{
    public long UserId { get; private set; }
    public virtual AppUser AppUser { get; private set; }

    public long FileId { get; private set; }
    public virtual FileEntity FileEntity { get; private set; }


    protected UserImage()
    {
    }

    private UserImage(long userId, long fileId)
    {
        this.UserId = userId;
        this.FileId = fileId;
    }

    public static UserImage Create(long userId, long fileId)
    {
        return new UserImage(userId, fileId);
    }
}
