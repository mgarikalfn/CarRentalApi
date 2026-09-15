namespace Domain.Common;

/// <summary>
/// Guard helpers for enforcing domain invariants. Enums are backed by their
/// underlying integer type with no runtime enforcement — (FuelType)999
/// compiles and assigns without error, so any enum arriving from outside
/// the process (deserialized JSON, a DB read, an external API) must be
/// validated explicitly before it's trusted inside the domain.
/// </summary>
public static class EnumGuard
{
    /// <summary>
    /// Validates that <paramref name="value"/> is a defined member of enum
    /// type <typeparamref name="TEnum"/>. Throws <see cref="DomainException"/>
    /// if not.
    /// </summary>
    public static TEnum ValidateDefined<TEnum>(TEnum value, string? fieldName = null)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
        {
            var name = fieldName ?? typeof(TEnum).Name;
            throw new DomainException($"Invalid {name} value: '{value}'.");
        }

        return value;
    }
}