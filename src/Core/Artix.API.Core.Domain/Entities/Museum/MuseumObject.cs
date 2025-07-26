namespace Artix.API.Core.Domain.Entities.Museum;

using Common;
using Exceptions;

public sealed class MuseumObject : BaseEntity
{
    public string Name { get; private set; }
    public string QRCode { get; private set; }
    public string? Description { get; private set; }
    public int? Version { get; private set; }
    public int? Tier { get; private set; }
    public bool IsSpecial { get; private set; }
    public bool IsHidden { get; private set; }
    public long MuseumId { get; private set; }
    public Museum Museum { get; private set; }
    private readonly List<MuseumObjectCategory> _museumObjectCategories = new();
    public IReadOnlyCollection<MuseumObjectCategory> MuseumObjectCategories => _museumObjectCategories.AsReadOnly();

    private MuseumObject()
    {
    }


    private MuseumObject(string name, string qrCode, Museum museum, bool isSpecial, bool isHidden)
    {
        ValidateName(name);
        ValidateQRCode(qrCode);
        Name = name;
        QRCode = qrCode;
        SetMuseum(museum);
        IsSpecial = isSpecial;
        IsHidden = isHidden;
    }

    public static MuseumObject Create(string name, string qrCode, Museum museum, bool isSpecial = false,
        bool isHidden = false)
    {
        return new MuseumObject(name, qrCode, museum, isSpecial, isHidden);
    }


    public void UpdateDetails(string? description, int? version, int? tier)
    {
        if (version is < 0)
            throw new ArgumentException("Version cannot be negative.");
        if (tier is < 0)
            throw new ArgumentException("Tier cannot be negative.");

        Description = description;
        Version = version;
        Tier = tier;
    }

    public void AssignVersionAndTier(int version, int tier)
    {
        if (version < 0)
            throw new ArgumentException("Version cannot be negative.");
        if (tier < 0)
            throw new ArgumentException("Tier cannot be negative.");

        Version = version;
        Tier = tier;
    }

    public void Rename(string newName)
    {
        ValidateName(newName);
        Name = newName;
    }

    public void ChangeQRCode(string newQRCode)
    {
        ValidateQRCode(newQRCode);
        QRCode = newQRCode;
    }

    public void UpdateMuseum(Museum newMuseum)
    {
        if (newMuseum == null)
            throw DomainException.InvalidValue(nameof(newMuseum));

        SetMuseum(newMuseum);
    }

    public void MarkAsSpecial()
    {
        IsSpecial = true;
    }

    public void UnmarkAsSpecial()
    {
        IsSpecial = false;
    }

    public void Hide()
    {
        IsHidden = true;
    }

    public void Visible()
    {
        IsHidden = false;
    }

    public bool IsVisible()
    {
        return !IsHidden;
    }

    public bool IsEligibleForDisplay()
    {
        return !IsHidden && IsSpecial;
    }

    public bool IsValidForExhibition()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(QRCode) &&
               !IsHidden;
    }

    public void AddCategory(Category category)
    {
        if (category == null)
            throw DomainException.InvalidValue(nameof(category));

        if (_museumObjectCategories.Any(c => c.CategoryId == category.Id))
            return;

        var link = MuseumObjectCategory.Create(this, category);
        _museumObjectCategories.Add(link);
    }

    public void RemoveCategory(Category category)
    {
        if (category == null)
            throw DomainException.InvalidValue(nameof(category));

        var link = _museumObjectCategories.FirstOrDefault(c => c.CategoryId == category.Id);
        if (link != null)
            _museumObjectCategories.Remove(link);
    }

    public void ClearCategories()
    {
        _museumObjectCategories.Clear();
    }

    public bool HasCategory(long categoryId)
    {
        return _museumObjectCategories.Any(c => c.CategoryId == categoryId);
    }

    private void SetMuseum(Museum museum)
    {
        Museum = museum ?? throw DomainException.InvalidValue(nameof(museum));
        ;
        MuseumId = museum.Id;
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));
    }

    private void ValidateQRCode(string qrCode)
    {
        if (string.IsNullOrWhiteSpace(qrCode))
            throw DomainException.InvalidValue(nameof(qrCode));
    }
}
