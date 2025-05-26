
namespace DotSpotifyWebWrapper.ApiCalls.Shared
{
    // Note: The documentation also states that this object "supports free form additional properties". What they are is unclear, however.
    public readonly struct OffsetObject
    {
        ///<summary>
        /// "position" is zero based and can’t be negative.
        ///</summary>
        public uint Position { get; init; }

        ///<summary>
        /// "uri" is a string representing the uri of the item to start at.
        ///</summary>
        public string? Uri { get; init; }

        public bool IsDefault => string.IsNullOrEmpty(Uri) && Position == default;
    }
}

