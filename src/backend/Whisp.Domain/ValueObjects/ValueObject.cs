namespace Whisp.Domain.ValueObjects;

/// <summary>
/// Base class for value objects that are compared by their structural equality rather than identity.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Returns the components used for equality comparison.
    /// </summary>
    /// <returns>An enumerable of the equality components.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) =>
                HashCode.Combine(current, obj?.GetHashCode() ?? 0));
    }

    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
        => Equals(left, right);

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
        => !Equals(left, right);
}