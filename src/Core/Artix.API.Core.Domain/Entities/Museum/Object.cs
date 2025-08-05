namespace Artix.API.Core.Domain.Entities.Museum;

using Common;
using Exceptions;

public class Object : BaseEntity
{
    public string Name { get; private set; }
    public string? QrCode { get; private set; }
    public string? GeneralInformation { get; private set; }
    public string? SpecialInformation { get; private set; }
    public int? Version { get; private set; }
    public int? Tier { get; private set; }
    public bool IsSpecial { get; private set; }
    public bool IsHidden { get; private set; }

    private readonly List<ObjectType> _objectTypes = new();
    public virtual IReadOnlyCollection<ObjectType> ObjectTypes => _objectTypes.AsReadOnly();

    private readonly List<ObjectHistoricalPeriod> _objectHistoricalPeriods = new();

    public virtual IReadOnlyCollection<ObjectHistoricalPeriod> ObjectHistoricalPeriods =>
        _objectHistoricalPeriods.AsReadOnly();

    protected Object()
    {
    }

    private Object(string name, string? qrCode, bool isSpecial = false, bool isHidden = false)
    {
        ValidateName(name);
        ValidateQrCode(qrCode);

        Name = name;
        QrCode = qrCode;
        IsSpecial = isSpecial;
        IsHidden = isHidden;
    }

    public static Object Create(string name, string? qrCode, bool isSpecial = false, bool isHidden = false)
    {
        return new Object(name, qrCode, isSpecial, isHidden);
    }

    public void UpdateDetails(string? generalInformation, string? specialInformation, int? version, int? tier)
    {
        if (version is < 0)
            throw DomainException.InvalidValue("Version cannot be negative.");
        if (tier is < 0)
            throw DomainException.InvalidValue("Tier cannot be negative.");

        GeneralInformation = generalInformation;
        SpecialInformation = specialInformation;
        Version = version;
        Tier = tier;
    }

    public void Rename(string newName)
    {
        ValidateName(newName);
        Name = newName;
    }

    public void ChangeQrCode(string? newQrCode)
    {
        ValidateQrCode(newQrCode);
        QrCode = newQrCode;
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

    public void Show()
    {
        IsHidden = false;
    }

    public bool IsVisible() => !IsHidden;

    public bool IsEligibleForDisplay() => !IsHidden && IsSpecial;

    public bool IsValidForExhibition() =>
        !string.IsNullOrWhiteSpace(Name) &&
        !string.IsNullOrWhiteSpace(QrCode) &&
        !IsHidden;

    public void AssignCategory(Type category)
    {
        if (category == null)
            throw DomainException.InvalidValue(nameof(category));

        if (_objectTypes.Any(c => c.CategoryId == category.Id))
            return;

        var link = ObjectType.Create(this, category);
        _objectTypes.Add(link);
    }

    public void RemoveCategory(Type category)
    {
        if (category == null)
            throw DomainException.InvalidValue(nameof(category));

        var link = _objectTypes.FirstOrDefault(c => c.CategoryId == category.Id);
        if (link != null)
            _objectTypes.Remove(link);
    }

    public void ClearCategories()
    {
        _objectTypes.Clear();
    }

    public bool HasCategory(long categoryId) => _objectTypes.Any(c => c.CategoryId == categoryId);

    public void AssignHistoricalPeriod(HistoricalPeriod period)
    {
        if (period == null)
            throw DomainException.InvalidValue(nameof(period));

        if (_objectHistoricalPeriods.Any(ohp => ohp.HistoricalPeriodId == period.Id))
            return;

        var link = ObjectHistoricalPeriod.Create(this, period);
        _objectHistoricalPeriods.Add(link);
    }

    public void RemoveHistoricalPeriod(HistoricalPeriod period)
    {
        if (period == null)
            throw DomainException.InvalidValue(nameof(period));

        var link = _objectHistoricalPeriods.FirstOrDefault(ohp => ohp.HistoricalPeriodId == period.Id);
        if (link != null)
            _objectHistoricalPeriods.Remove(link);
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));
    }

    private void ValidateQrCode(string? qrCode)
    {
        if (string.IsNullOrWhiteSpace(qrCode))
            throw DomainException.InvalidValue(nameof(qrCode));
    }
}
