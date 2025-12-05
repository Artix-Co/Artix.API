namespace Artix.API.Utils;

public static class FileNameHelper
{
    private static readonly HashSet<char> InvalidFileNameChars = 
        Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).ToHashSet();

    public static string SanitizeFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "file";

        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName) ?? string.Empty;

        var sanitizedName = string.Concat(nameWithoutExtension
            .Where(c => !InvalidFileNameChars.Contains(c)));

        var sanitizedExt = string.Concat(extension
            .Where(c => !InvalidFileNameChars.Contains(c)));

        if (string.IsNullOrWhiteSpace(sanitizedName))
            sanitizedName = "file";

        return $"{sanitizedName}{sanitizedExt}";
    }

    public static string GenerateUniqueFileName(string originalFileName)
    {
        var sanitized = SanitizeFileName(originalFileName);
        var guid = Guid.NewGuid().ToString();
        var extension = Path.GetExtension(sanitized);

        var nameWithoutExt = string.IsNullOrEmpty(extension) 
            ? sanitized 
            : sanitized[..^extension.Length];

        return $"{nameWithoutExt}_{guid}{extension}";
    }
}
