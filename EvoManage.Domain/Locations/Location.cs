using EvoManage.Domain.Common;
using EvoManage.Domain.Common.Exceptions;

namespace EvoManage.Domain.Locations;

public sealed class Location : BaseEntity
{
    public int WarehouseId { get; private set; }
    public string Code { get; private set; } = null!;
    public string? GroupCode { get; private set; }
    public bool IsActive { get; private set; }

    private Location()
    {
    }

    public static Location Create(
        int warehouseId,
        string code,
        string? groupCode = null)
    {
        if (warehouseId <= 0)
            throw new DomainException(
                "Warehouse id must be greater than zero.");

        var normalized = ValidateAndNormalize(
            code,
            groupCode);

        return new Location
        {
            WarehouseId = warehouseId,
            Code = normalized.Code,
            GroupCode = normalized.GroupCode,
            IsActive = true
        };
    }

    public void Update(
        string code,
        string? groupCode)
    {
        var normalized = ValidateAndNormalize(
            code,
            groupCode);

        Code = normalized.Code;
        GroupCode = normalized.GroupCode;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static (
        string Code,
        string? GroupCode) ValidateAndNormalize(
        string code,
        string? groupCode)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException(
                "Location code cannot be empty.");

        code = code.Trim();
        groupCode = NormalizeOptional(groupCode);

        if (code.Length > 100)
            throw new DomainException(
                "Location code cannot exceed 100 characters.");

        if (groupCode?.Length > 50)
            throw new DomainException(
                "Location group code cannot exceed 50 characters.");

        return (code, groupCode);
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}