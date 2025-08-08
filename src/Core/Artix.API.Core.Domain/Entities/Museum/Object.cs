namespace Artix.API.Core.Domain.Entities.Museum;

using Common;
using Exceptions;
using File;

public class Object : BaseEntity
{
    public string Name { get; private set; }
    public string? QrCode { get; private set; }
    public string? GeneralInformation { get; private set; }
    public string? SpecialInformation { get; private set; }
    public int? Version { get; private set; }
    public int? Tier { get; private set; }
    public bool IsSpecial { get; private set; } = false;
    public bool IsHidden { get; private set; } = false;


    private readonly List<ObjectFile> _objectFiles = new();
    public virtual IReadOnlyCollection<ObjectFile> ObjectFiles => _objectFiles.AsReadOnly();


    private readonly List<ObjectType> _objectTypes = new();
    public virtual IReadOnlyCollection<ObjectType> ObjectTypes => _objectTypes.AsReadOnly();


    private readonly List<ObjectHistoricalPeriod> _objectHistoricalPeriods = new();

    public virtual IReadOnlyCollection<ObjectHistoricalPeriod> ObjectHistoricalPeriods =>
        _objectHistoricalPeriods.AsReadOnly();

    // Protected constructor for EF Core
    protected Object()
    {
    }

    private Object(
        string name,
        string? qrCode,
        string? generalInformation,
        string? specialInformation,
        int? version,
        int? tier,
        bool isSpecial,
        bool isHidden,
        List<ObjectType> objectTypes,
        List<ObjectHistoricalPeriod> objectHistoricalPeriods)
    {
        ValidateName(name);
        ValidateQrCode(qrCode);
        ValidateVersion(version);
        ValidateTier(tier);

        Name = name;
        QrCode = qrCode;
        GeneralInformation = generalInformation;
        SpecialInformation = specialInformation;
        Version = version;
        Tier = tier;
        IsSpecial = isSpecial;
        IsHidden = isHidden;

        _objectTypes = objectTypes ?? new List<ObjectType>();
        _objectHistoricalPeriods = objectHistoricalPeriods ?? new List<ObjectHistoricalPeriod>();
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

    private void ValidateVersion(int? version)
    {
        if (version is < 0)
            throw DomainException.InvalidValue("Version cannot be negative.");
    }

    private void ValidateTier(int? tier)
    {
        if (tier is < 0)
            throw DomainException.InvalidValue("Tier cannot be negative.");
    }

    // Builder class
    public class ObjectBuilder
    {
        private string? _name;
        private string? _qrCode;
        private string? _generalInformation;
        private string? _specialInformation;
        private int? _version;
        private int? _tier;
        private bool _isSpecial;
        private bool _isHidden;
        private readonly List<ObjectType> _objectTypes = new();
        private readonly List<ObjectHistoricalPeriod> _objectHistoricalPeriods = new();

        public ObjectBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ObjectBuilder WithQrCode(string? qrCode)
        {
            _qrCode = qrCode;
            return this;
        }

        public ObjectBuilder WithGeneralInformation(string? generalInformation)
        {
            _generalInformation = generalInformation;
            return this;
        }

        public ObjectBuilder WithSpecialInformation(string? specialInformation)
        {
            _specialInformation = specialInformation;
            return this;
        }

        public ObjectBuilder WithVersion(int? version)
        {
            _version = version;
            return this;
        }

        public ObjectBuilder WithTier(int? tier)
        {
            _tier = tier;
            return this;
        }

        public ObjectBuilder AsSpecial()
        {
            _isSpecial = true;
            return this;
        }

        public ObjectBuilder AsHidden()
        {
            _isHidden = true;
            return this;
        }

        public ObjectBuilder WithObjectType(ObjectType objectType)
        {
            if (objectType != null && !_objectTypes.Any(ot => ot.TypeId == objectType.TypeId))
                _objectTypes.Add(objectType);
            return this;
        }

        public ObjectBuilder WithObjectTypes(IEnumerable<ObjectType> objectTypes)
        {
            if (objectTypes != null)
            {
                foreach (var objectType in objectTypes)
                {
                    if (!_objectTypes.Any(ot => ot.TypeId == objectType.TypeId))
                        _objectTypes.Add(objectType);
                }
            }

            return this;
        }

        public ObjectBuilder WithHistoricalPeriod(ObjectHistoricalPeriod historicalPeriod)
        {
            if (historicalPeriod != null &&
                !_objectHistoricalPeriods.Any(ohp => ohp.HistoricalPeriodId == historicalPeriod.HistoricalPeriodId))
                _objectHistoricalPeriods.Add(historicalPeriod);
            return this;
        }

        public ObjectBuilder WithHistoricalPeriods(IEnumerable<ObjectHistoricalPeriod> historicalPeriods)
        {
            if (historicalPeriods != null)
            {
                foreach (var period in historicalPeriods)
                {
                    if (!_objectHistoricalPeriods.Any(ohp => ohp.HistoricalPeriodId == period.HistoricalPeriodId))
                        _objectHistoricalPeriods.Add(period);
                }
            }

            return this;
        }

        public Object Build()
        {
            return new Object(
                _name!,
                _qrCode,
                _generalInformation,
                _specialInformation,
                _version,
                _tier,
                _isSpecial,
                _isHidden,
                _objectTypes,
                _objectHistoricalPeriods
            );
        }
    }

    // Static factory method to create a builder
    public static ObjectBuilder CreateBuilder()
    {
        return new ObjectBuilder();
    }

    private Object(
        string name,
        string? qrCode,
        string? generalInformation,
        string? specialInformation,
        int? version,
        int? tier,
        bool isSpecial,
        bool isHidden)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue("Name cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(qrCode))
            throw DomainException.InvalidValue("QrCode cannot be null or empty.");
        if (version is < 0)
            throw DomainException.InvalidValue("Version cannot be negative.");
        if (tier is < 0)
            throw DomainException.InvalidValue("Tier cannot be negative.");

        Name = name;
        QrCode = qrCode;
        GeneralInformation = generalInformation;
        SpecialInformation = specialInformation;
        Version = version;
        Tier = tier;
        IsSpecial = isSpecial;
        IsHidden = isHidden;
    }

    public static Object Create(
        string name,
        string qrCode,
        string? generalInformation = null,
        string? specialInformation = null,
        int? version = null,
        int? tier = null,
        bool isSpecial = false,
        bool isHidden = false)
    {
        return new Object(name, qrCode, generalInformation, specialInformation, version, tier, isSpecial, isHidden);
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

        if (_objectTypes.Any(c => c.TypeId == category.Id))
            return;

        var link = ObjectType.Create(this, category);
        _objectTypes.Add(link);
    }

    public void RemoveCategory(Type category)
    {
        if (category == null)
            throw DomainException.InvalidValue(nameof(category));

        var link = _objectTypes.FirstOrDefault(c => c.TypeId == category.Id);
        if (link != null)
            _objectTypes.Remove(link);
    }

    public void ClearCategories()
    {
        _objectTypes.Clear();
    }

    public bool HasCategory(long categoryId) => _objectTypes.Any(c => c.TypeId == categoryId);

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

    public void Assign3DModel(File file, string[] allowedMimeTypes)
    {
        if (file is null)
            throw DomainException.InvalidValue(nameof(file));


        if (!allowedMimeTypes.Contains(file.MimeType))
            throw DomainException.InvalidValue(nameof(file.MimeType));

        var existingModel = _objectFiles
            .FirstOrDefault(of => allowedMimeTypes.Contains(of.File.MimeType));

        if (existingModel is not null)
            _objectFiles.Remove(existingModel);

        _objectFiles.Add(ObjectFile.Create(this, file));
    }

    public void Remove3DModel()
    {
        var existingModel = _objectFiles.FirstOrDefault(of => Is3DModel(of.File));

        if (existingModel is not null)
            _objectFiles.Remove(existingModel);
    }

    public File? Get3DModel()
    {
        return _objectFiles
            .FirstOrDefault(of => Is3DModel(of.File))
            ?.File;
    }

    public bool Has3DModel()
    {
        return _objectFiles.Exists(of => Is3DModel(of.File));
    }

    private static bool Is3DModel(File file)
    {
        var allowedMimeTypes = new[] { "model/gltf-binary", "model/obj", "model/gltf+json" };

        return file is not null && allowedMimeTypes.Contains(file.MimeType);
    }
}
