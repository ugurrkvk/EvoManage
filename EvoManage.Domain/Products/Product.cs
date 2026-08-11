using EvoManage.Domain.Common;
using EvoManage.Domain.Common.Exceptions;

namespace EvoManage.Domain.Products;

public sealed class Product : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public ProductTrackingType TrackingType { get; private set; }
    public bool IsActive { get; private set; }

    private Product()
    {
    }

    public static Product Create(
        string code,
        string name,
        ProductTrackingType trackingType)
    {
        var normalized = ValidateAndNormalize(
            code,
            name,
            trackingType);

        return new Product
        {
            Code = normalized.Code,
            Name = normalized.Name,
            TrackingType = trackingType,
            IsActive = true
        };
    }

    public void Update(
        string code,
        string name,
        ProductTrackingType trackingType)
    {
        var normalized = ValidateAndNormalize(
            code,
            name,
            trackingType);

        Code = normalized.Code;
        Name = normalized.Name;
        TrackingType = trackingType;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static (string Code, string Name) ValidateAndNormalize(
        string code,
        string name,
        ProductTrackingType trackingType)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Product code cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be empty.");

        code = code.Trim();
        name = name.Trim();

        if (code.Length > 50)
            throw new DomainException(
                "Product code cannot exceed 50 characters.");

        if (name.Length > 200)
            throw new DomainException(
                "Product name cannot exceed 200 characters.");

        if (!Enum.IsDefined(trackingType))
            throw new DomainException(
                "Invalid product tracking type.");

        return (code, name);
    }
}