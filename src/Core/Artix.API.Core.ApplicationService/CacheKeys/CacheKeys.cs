namespace Artix.API.Core.ApplicationService.CacheKeys;

internal static class CacheKeys
{
    public static string RecentMuseums(Guid userId)
        => $"recent-museums:v2:user:{userId}";

    public static string RecentObjects(Guid userId)
        => $"recent-objects:v2:user:{userId}";
}
