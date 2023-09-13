

public static class MaybeExtensions
{
    public static Maybe<T> ToMaybe<T>(this T value) where T : class
    {
        return new Maybe<T>(value);
    }
}

public struct Maybe<T> where T : class
{
    private readonly T _value;

    public bool HasValue => _value != null;

    public Maybe(T value)
    {
        _value = value;
    }

    public TResult Match<TResult>(Func<T, TResult> some, TResult none)
    {
        return HasValue ? some(_value) : none;
    }
}