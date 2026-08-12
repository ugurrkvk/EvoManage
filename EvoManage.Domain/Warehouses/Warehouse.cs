using EvoManage.Domain.Common;
using EvoManage.Domain.Common.Exceptions;

namespace EvoManage.Domain.Warehouses;

public sealed class Warehouse : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Warehouse()
    {
    }

    public static Warehouse Create(
        string code,
        string name,
        string? address = null,
        string? description = null)
    {
        var normalized = ValidateAndNormalize(
            code,
            name,
            address,
            description);

        return new Warehouse
        {
            Code = normalized.Code,
            Name = normalized.Name,
            Address = normalized.Address,
            Description = normalized.Description,
            IsActive = true
        };
    }

    public void Update(
        string code,
        string name,
        string? address,
        string? description)
    {
        var normalized = ValidateAndNormalize(
            code,
            name,
            address,
            description);

        Code = normalized.Code;
        Name = normalized.Name;
        Address = normalized.Address;
        Description = normalized.Description;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static (
        string Code,
        string Name,
        string? Address,
        string? Description) ValidateAndNormalize(
        string code,
        string name,
        string? address,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Warehouse code cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Warehouse name cannot be empty.");

        code = code.Trim();
        name = name.Trim();
        address = NormalizeOptional(address);
        description = NormalizeOptional(description);

        if (code.Length > 50)
            throw new DomainException(
                "Warehouse code cannot exceed 50 characters.");

        if (name.Length > 200)
            throw new DomainException(
                "Warehouse name cannot exceed 200 characters.");

        if (address?.Length > 500)
            throw new DomainException(
                "Warehouse address cannot exceed 500 characters.");

        if (description?.Length > 1000)
            throw new DomainException(
                "Warehouse description cannot exceed 1000 characters.");

        return (code, name, address, description);
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}