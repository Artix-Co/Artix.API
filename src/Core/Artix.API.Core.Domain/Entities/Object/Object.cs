namespace Artix.API.Core.Domain.Entities.Object;

using Collection;
using Common;
using Enums;
using File;
using Events;
using User;
using Exceptions;
using Museum;

public class Object : AggregateRoot
{
    public string Name { get; private set; }
    public string? QrCode { get; private set; }
    public string? GeneralInformation { get; private set; }
    public string? SpecialInformation { get; private set; }
    public int? Version { get; private set; }
    public int? Tier { get; private set; }
    public bool IsSpecial { get; private set; } = false;
    public bool IsHidden { get; private set; } = false;
    public ObjectSaleType ObjectSaleType { get; private set; }


    private readonly List<ObjectModel> _objectModels = new();
    public virtual IReadOnlyCollection<ObjectModel> ObjectModels => this._objectModels.AsReadOnly();


    private readonly List<ObjectImage> _objectImages = new();
    public virtual IReadOnlyCollection<ObjectImage> ObjectImages => this._objectImages.AsReadOnly();


    private readonly List<ObjectType> _objectTypes = new();
    public virtual IReadOnlyCollection<ObjectType> ObjectTypes => this._objectTypes.AsReadOnly();


    private readonly List<ObjectHistoricalPeriod> _objectHistoricalPeriods = new();

    public virtual IReadOnlyCollection<ObjectHistoricalPeriod> ObjectHistoricalPeriods =>
        this._objectHistoricalPeriods.AsReadOnly();


    private readonly List<UserObject> _userObjects = new();
    public virtual IReadOnlyCollection<UserObject> UserObjects => _userObjects.AsReadOnly();

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
        List<ObjectHistoricalPeriod> objectHistoricalPeriods,
        ObjectSaleType objectSaleType
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));
        if (string.IsNullOrWhiteSpace(qrCode))
            throw DomainException.InvalidValue(nameof(qrCode));
        if (version is < 0)
            throw DomainException.InvalidValue("Version cannot be negative.");
        if (tier is < 0)
            throw DomainException.InvalidValue("Tier cannot be negative.");

        this.Name = name;
        this.QrCode = qrCode;
        this.GeneralInformation = generalInformation;
        this.SpecialInformation = specialInformation;
        this.Version = version;
        this.Tier = tier;
        this.IsSpecial = isSpecial;
        this.IsHidden = isHidden;
        this._objectTypes = objectTypes;
        this._objectHistoricalPeriods = objectHistoricalPeriods;
        this.ObjectSaleType = objectSaleType;
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
        ObjectSaleType objectSaleType)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue("Name cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(qrCode))
            throw DomainException.InvalidValue("QrCode cannot be null or empty.");
        if (version is < 0)
            throw DomainException.InvalidValue("Version cannot be negative.");
        if (tier is < 0)
            throw DomainException.InvalidValue("Tier cannot be negative.");

        this.Name = name;
        this.QrCode = qrCode;
        this.GeneralInformation = generalInformation;
        this.SpecialInformation = specialInformation;
        this.Version = version;
        this.Tier = tier;
        this.IsSpecial = isSpecial;
        this.IsHidden = isHidden;
        this.ObjectSaleType = objectSaleType;
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
        private ObjectSaleType _objectSaleType;
        private readonly List<ObjectType> _objectTypes = new();
        private readonly List<ObjectHistoricalPeriod> _objectHistoricalPeriods = new();

        public ObjectBuilder WithName(string name)
        {
            this._name = name;
            return this;
        }

        public ObjectBuilder WithQrCode(string? qrCode)
        {
            this._qrCode = qrCode;
            return this;
        }

        public ObjectBuilder WithGeneralInformation(string? generalInformation)
        {
            this._generalInformation = generalInformation;
            return this;
        }

        public ObjectBuilder WithObjectSaleType(ObjectSaleType objectSaleType)
        {
            this._objectSaleType = objectSaleType;
            return this;
        }

        public ObjectBuilder WithSpecialInformation(string? specialInformation)
        {
            this._specialInformation = specialInformation;
            return this;
        }

        public ObjectBuilder WithVersion(int? version)
        {
            this._version = version;
            return this;
        }

        public ObjectBuilder WithTier(int? tier)
        {
            this._tier = tier;
            return this;
        }

        public ObjectBuilder AsSpecial()
        {
            this._isSpecial = true;
            return this;
        }

        public ObjectBuilder AsHidden()
        {
            this._isHidden = true;
            return this;
        }

        public ObjectBuilder WithObjectType(ObjectType objectType)
        {
            if (objectType != null && !this._objectTypes.Any(ot => ot.TypeId == objectType.TypeId))
                this._objectTypes.Add(objectType);
            return this;
        }

        public ObjectBuilder WithObjectTypes(IEnumerable<ObjectType> objectTypes)
        {
            if (objectTypes != null)
            {
                foreach (var objectType in objectTypes)
                {
                    if (!this._objectTypes.Any(ot => ot.TypeId == objectType.TypeId))
                        this._objectTypes.Add(objectType);
                }
            }

            return this;
        }

        public ObjectBuilder WithHistoricalPeriod(ObjectHistoricalPeriod historicalPeriod)
        {
            if (historicalPeriod != null &&
                !this._objectHistoricalPeriods.Any(ohp =>
                    ohp.HistoricalPeriodId == historicalPeriod.HistoricalPeriodId))
                this._objectHistoricalPeriods.Add(historicalPeriod);
            return this;
        }

        public ObjectBuilder WithHistoricalPeriods(IEnumerable<ObjectHistoricalPeriod> historicalPeriods)
        {
            if (historicalPeriods != null)
            {
                foreach (var period in historicalPeriods)
                {
                    if (!this._objectHistoricalPeriods.Any(ohp => ohp.HistoricalPeriodId == period.HistoricalPeriodId))
                        this._objectHistoricalPeriods.Add(period);
                }
            }

            return this;
        }

        public Object Build()
        {
            return new Object(
                this._name!,
                this._qrCode,
                this._generalInformation,
                this._specialInformation,
                this._version,
                this._tier,
                this._isSpecial,
                this._isHidden,
                this._objectTypes,
                this._objectHistoricalPeriods,
                this._objectSaleType
            );
        }
    }

    // Static factory method to create a builder
    public static ObjectBuilder CreateBuilder()
    {
        return new ObjectBuilder();
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
        return new Object(name,
            qrCode,
            generalInformation,
            specialInformation,
            version,
            tier,
            isSpecial,
            isHidden,
            ObjectSaleType.Free);
    }

    public void UpdateDetails(string? generalInformation, string? specialInformation, int? version, int? tier)
    {
        if (version is < 0)
            throw DomainException.InvalidValue("Version cannot be negative.");
        if (tier is < 0)
            throw DomainException.InvalidValue("Tier cannot be negative.");

        this.GeneralInformation = generalInformation;
        this.SpecialInformation = specialInformation;
        this.Version = version;
        this.Tier = tier;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw DomainException.InvalidValue("Name cannot be null or empty.");

        this.Name = newName;
    }


    public void MarkAsSpecial()
    {
        this.IsSpecial = true;
    }

    public void UnmarkAsSpecial()
    {
        this.IsSpecial = false;
    }

    public void Hide()
    {
        this.IsHidden = true;
    }

    public void Show()
    {
        this.IsHidden = false;
    }

    public bool IsVisible() => !this.IsHidden;

    public bool IsEligibleForDisplay() => !this.IsHidden && this.IsSpecial;

    public bool IsValidForExhibition() =>
        !string.IsNullOrWhiteSpace(this.Name) &&
        !string.IsNullOrWhiteSpace(this.QrCode) &&
        !this.IsHidden;

    public void AssignCategory(Type category)
    {
        if (category == null)
            throw DomainException.InvalidValue(nameof(category));

        if (this._objectTypes.Any(c => c.TypeId == category.Id))
            return;

        var link = ObjectType.Create(this, category);
        this._objectTypes.Add(link);
    }

    public void RemoveCategory(Type category)
    {
        if (category == null)
            throw DomainException.InvalidValue(nameof(category));

        var link = this._objectTypes.FirstOrDefault(c => c.TypeId == category.Id);
        if (link != null)
            this._objectTypes.Remove(link);
    }

    public void ClearCategories()
    {
        this._objectTypes.Clear();
    }

    public bool HasCategory(long categoryId) => this._objectTypes.Any(c => c.TypeId == categoryId);

    public void AssignHistoricalPeriod(HistoricalPeriod period)
    {
        if (period == null)
            throw DomainException.InvalidValue(nameof(period));

        if (this._objectHistoricalPeriods.Any(ohp => ohp.HistoricalPeriodId == period.Id))
            return;

        var link = ObjectHistoricalPeriod.Create(this, period);
        this._objectHistoricalPeriods.Add(link);
    }

    public void RemoveHistoricalPeriod(HistoricalPeriod period)
    {
        if (period == null)
            throw DomainException.InvalidValue(nameof(period));

        var link = this._objectHistoricalPeriods.FirstOrDefault(ohp => ohp.HistoricalPeriodId == period.Id);
        if (link != null)
            this._objectHistoricalPeriods.Remove(link);
    }

    public void AddToCollection(Collection collection)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        if (!collection.Items.Any(ci => ci.ObjectId == this.Id))
        {
            var collectionItem = CollectionItem.Create(collection, this);
            collection.AddItem(collectionItem);
        }
    }


    public void Assign3DModel(File file, string[] allowedMimeTypes)
    {
        if (file is null)
            throw DomainException.InvalidValue(nameof(file));


        if (!allowedMimeTypes.Contains(file.MimeType))
            throw DomainException.InvalidValue(nameof(file.MimeType));

        var existingModel = this._objectModels
            .FirstOrDefault(of => allowedMimeTypes.Contains(of.File.MimeType));

        if (existingModel is not null)
            this._objectModels.Remove(existingModel);

        this._objectModels.Add(ObjectModel.Create(this, file));
    }


    public void AssignImage(File file, string[] allowedMimeTypes)
    {
        if (file is null)
            throw DomainException.InvalidValue(nameof(file));


        if (!allowedMimeTypes.Contains(file.MimeType))
            throw DomainException.InvalidValue(nameof(file.MimeType));

        var existingModel = this._objectImages
            .FirstOrDefault(of => allowedMimeTypes.Contains(of.File.MimeType));

        if (existingModel is not null)
            this._objectImages.Remove(existingModel);

        this._objectImages.Add(ObjectImage.Create(this, file));
    }

    public void Remove3DModel()
    {
        var existingModel = this._objectModels.FirstOrDefault(of => Is3DModel(of.File));

        if (existingModel is not null)
            this._objectModels.Remove(existingModel);
    }

    public File? Get3DModel()
    {
        return this._objectModels
            .FirstOrDefault(of => Is3DModel(of.File))
            ?.File;
    }

    public bool Has3DModel()
    {
        return this._objectModels.Exists(of => Is3DModel(of.File));
    }

    private static bool Is3DModel(File file)
    {
        var allowedMimeTypes = new[] { "model/gltf-binary", "model/obj", "model/gltf+json" };

        return allowedMimeTypes.Contains(file.MimeType);
    }

    public void FirstTimeUserScan(long userId)
    {
        var userObject = UserObject.Create(userId, Id);
        userObject.AssignToUser(DateTime.UtcNow);
        _userObjects.Add(userObject);
        RaiseDomainEvent(new FirstUserScanEvent(BusinessId, userId, this.Id, 1, DateTime.UtcNow, true));
    }

    public void RepeatUserScan(UserObject userObject)
    {
        // TODO: user layer exception
        if (userObject == null)
            throw DomainException.InvalidValue(nameof(userObject));

        if (!_userObjects.Contains(userObject))
            _userObjects.Add(userObject);

        userObject.Upgrade();
        RaiseDomainEvent(new RepeatUserScanEvent(BusinessId, userObject.UserId, userObject.ObjectId,
            userObject.ScanCount, true));
    }
}
