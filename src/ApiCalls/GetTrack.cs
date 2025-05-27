using System.Collections.Generic;
using DotSpotifyWebWrapper.ApiCalls.Shared;
using DotSpotifyWebWrapper.Types;
using DotSpotifyWebWrapper.Utilities;

namespace DotSpotifyWebWrapper.ApiCalls
{
    public sealed class GetTrack(string trackId, string market = "") : SpotifyApiCallBase
    {
        public record ResponseData
        {
            /// <summary>
            /// The album on which the track appears.
            /// </summary>
            public AlbumObject Album { get; init; }

            /// <summary>
            /// The artists who performed the track.
            /// </summary>
            public List<SimplifiedArtistObject>? Artists { get; init; }

            /// <summary>
            ///A list of the countries in which the track can be played, identified by their ISO 3166-1 alpha-2 code.
            /// </summary>
            public List<string>? AvailableMarkets { get; init; }

            /// <summary>
            /// The disc number (usually 1 unless the album consists of more than one disc).
            /// </summary>
            public int DiscNumber { get; init; }

            /// <summary>
            /// The track length in milliseconds.
            /// </summary>
            public int DurationMs { get; init; }

            /// <summary>
            /// Whether or not the track has explicit lyrics.
            /// </summary>
            public bool Explicit { get; init; }

            /// <summary>
            /// Known external IDs for the track.
            /// </summary>
            public ExternalIdsObject ExternalIds { get; init; }

            /// <summary>
            /// Known external URLs for this track.
            /// </summary>
            public ExternalUrlsObject ExternalUrls { get; init; }

            /// <summary>
            /// A link to the Web API endpoint providing full details of the track.
            /// </summary>
            public string? Href { get; init; }

            /// <summary>
            ///  The Spotify ID for the track.
            /// </summary>
            public string? Id { get; init; }

            /// <summary>
            /// Part of the response when Track Relinking is applied. If true, the track is playable in the given market. 
            /// </summary>
            public bool IsPlayable { get; init; }

            /// <summary>
            /// Part of the response when Track Relinking is applied, and the requested track has been replaced with different track.
            /// </summary>
            public ResponseData? LinkedFrom { get; init; }

            /// <summary>
            /// Included in the response when a content restriction is applied.
            /// </summary>
            public RestrictionsObject Restrictions { get; init; }

            /// <summary>
            /// The name of the track.
            /// </summary>
            public string? Name { get; init; }

            /// <summary>
            /// The popularity of the track. The value will be between 0 and 100, with 100 being the most popular.
            /// </summary>
            public int Popularity { get; init; }

            /// <summary>
            /// The number of the track. If an album has several discs, the track number is the number on the specified disc.
            /// </summary>
            public int TrackNumber { get; init; }

            /// <summary>
            /// The object type: "track".
            /// </summary>
            public string? Type { get; init; }

            /// <summary>
            /// The Spotify URI for the track.
            /// </summary>
            public string? Uri { get; init; }

            /// <summary>
            /// Whether or not the track is from a local file.
            /// </summary>
            public bool IsLocal { get; init; }

        }

        public ResponseData? Response { get; private set; }

        public override SpotifyApiCallType ApiCall => SpotifyApiCallType.GetTrack;

        protected override HttpRequestMethod RequestMethod => HttpRequestMethod.Get;

        protected override List<AccessScopeType> Scopes => [];

        protected override string Endpoint =>
            SpotifyEndpoint.TracksEndpoint +
            "/" + _trackId +
            HttpRequestUriUtilities.GetQueryStringOrEmpty("market", _market);

        private readonly string _trackId = trackId;
        private readonly string _market = market;

        protected override void ParseResponse()
        {
            var (success, data) = ReadAndDeserializeJsonResponse<ResponseData>();
            if (success)
                Response = data;
        }
    }
}
