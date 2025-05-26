using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotSpotifyWebWrapper.ApiCalls.Shared;
using DotSpotifyWebWrapper.Types;
using DotSpotifyWebWrapper.Utilities;

namespace DotSpotifyWebWrapper.ApiCalls
{
    public sealed class GetPlaybackState : SpotifyApiCallBase
    {
        public record ResponseData
        {
            /// <summary>
            /// The device that is currently active.
            /// </summary>
            public DeviceObject Device { get; init; }

            /// <summary>
            /// Either off, track or context.
            /// </summary>
            public string? RepeatState { get; init; }

            /// <summary>
            /// If shuffle is on or off.
            /// </summary>
            public bool ShuffleState { get; init; }

            /// <summary>
            /// A Context Object. Can be null.
            /// </summary>
            public ContextObject? Context { get; init; }

            /// <summary>
            /// Unix Millisecond Timestamp when data was fetched.
            /// </summary>
            public long Timestamp { get; init; }

            /// <summary>
            /// Progress into the currently playing track or episode. Can be null.
            /// </summary>
            public int? ProgressMs { get; init; }

            /// <summary>
            /// If something is currently playing, return true.
            /// </summary>
            public bool IsPlaying { get; init; }

            /// <summary>
            /// The currently playing track or episode. Can be null.
            /// </summary>
            public OneOf<TrackObject, EpisodeObject>? TrackOrEpisode { get; set; }

            /// <summary>
            /// The object type of the currently playing item. Can be one of track, episode, ad or unknown
            /// </summary>
            public string? CurrentlyPlayingType { get; init; }

            /// <summary>
            ///  Allows to update the user interface based on which playback actions are available within the current context.
            /// </summary>
            public ActionsObject Actions { get; init; }
        }

        public ResponseData? Response { get; private set; }

        public override SpotifyApiCallType ApiCall => SpotifyApiCallType.GetPlaybackState;

        protected override HttpRequestMethod RequestMethod => HttpRequestMethod.Get;

        protected override List<AccessScopeType> Scopes => [AccessScopeType.UserReadPlaybackState];

        protected override string Endpoint => SpotifyEndpoint.PlayerBaseEndpoint;

        protected override async Task ParseResponse()
        {
            var (success, isEmpty, data) = await ReadAndDeserializeJsonResponse<ResponseData>(true, (r, s) =>
            {
                if (string.IsNullOrEmpty(r.CurrentlyPlayingType))
                    return new Exception("Empty response");

                var itemContent = s.TryGetContentForToken("item");
                if (string.IsNullOrEmpty(itemContent))
                    return new Exception("Got not find item token in response"); ;

                if (r.CurrentlyPlayingType.Equals("track", StringComparison.InvariantCultureIgnoreCase))
                {
                    var (track, e) = itemContent.DeserializeJsonString<TrackObject>(convertSnakeCaseToPascalCase: true);
                    if (e == default)
                        r.TrackOrEpisode = new() { First = track };
                    return e;
                }
                else if (r.CurrentlyPlayingType.Equals("episode", StringComparison.InvariantCultureIgnoreCase))
                {
                    var (episode, e) = itemContent.DeserializeJsonString<EpisodeObject>(convertSnakeCaseToPascalCase: true);
                    if (e == default)
                        r.TrackOrEpisode = new() { Second = episode };
                    return e;
                }
                else
                {
                    return new NotSupportedException($"'{r.CurrentlyPlayingType}' content is not supported");
                }
            });

            if (success)
                Response = data;
        }
    }
}
