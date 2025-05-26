namespace DotSpotifyWebWrapper.Types
{
    public readonly struct OneOf<T1, T2> where T1 : class
                                         where T2 : class
    {
        public T1? First { get; init; }

        public T2? Second { get; init; }

        public bool Is<T>()
            => typeof(T).IsAssignableTo(typeof(T1)) ? First != default :
            typeof(T).IsAssignableTo(typeof(T2)) ? Second != default :
            false;

        public T? Get<T>() where T : class
            => typeof(T).IsAssignableTo(typeof(T1)) ? First as T :
            typeof(T).IsAssignableTo(typeof(T2)) ? Second as T :
            default;
    }
}
