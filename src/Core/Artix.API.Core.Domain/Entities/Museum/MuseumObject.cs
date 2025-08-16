namespace Artix.API.Core.Domain.Entities.Museum;

using Common;
using Exceptions;
using Object;

public class MuseumObject : BaseEntity
{
    public long MuseumId { get; private set; }
    public virtual Museum Museum { get; private set; }

    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }

    public string Name { get; private set; }
    public string QRCode { get; private set; }
    public bool IsSpecial { get; private set; }
    public bool IsHidden { get; private set; }

    protected MuseumObject() { }

    private MuseumObject(Object obj, Museum museum, string qrCode, bool isSpecial, bool isHidden)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));
        if (museum == null)
            throw DomainException.InvalidValue(nameof(museum));

        ValidateName(obj.Name);
        ValidateQRCode(qrCode);

        Object = obj;
        ObjectId = obj.Id;
        Museum = museum;
        MuseumId = museum.Id;
        Name = obj.Name;
        QRCode = qrCode;
        IsSpecial = isSpecial;
        IsHidden = isHidden;
    }

    public static MuseumObject Create(Object obj, Museum museum, string qrCode, bool isSpecial = false, bool isHidden = false)
    {
        return new MuseumObject(obj, museum, qrCode, isSpecial, isHidden);
    }

    public void UpdateDetails(string? qrCode, bool? isSpecial = null, bool? isHidden = null)
    {
        if (qrCode != null)
        {
            ValidateQRCode(qrCode);
            QRCode = qrCode;
        }

        if (isSpecial.HasValue)
            IsSpecial = isSpecial.Value;

        if (isHidden.HasValue)
            IsHidden = isHidden.Value;
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
