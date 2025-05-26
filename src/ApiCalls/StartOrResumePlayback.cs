using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DotSpotifyWebWrapper.ApiCalls.Shared;
using DotSpotifyWebWrapper.Types;
using DotSpotifyWebWrapper.Utilities;

namespace DotSpotifyWebWrapper.ApiCalls
{
    public sealed class StartOrResumePlayback(string deviceId = "") : SpotifyApiCallBase
    {
        /// <summary>
        /// Optional. Spotify URI of the context to play. Valid contexts are albums, artists & playlists.
        /// </summary>
        public string? ContextUri { get; init; }

        /// <summary>
        /// Optional. A JSON array of the Spotify track URIs to play.
        /// </summary>
        public string[]? Uris { get; init; }

        /// <summary>
        /// Optional. Indicates from where in the context playback should start. Only available when context_uri corresponds to an album or playlist object.
        /// </summary>
        public OffsetObject Offset { get; init; }

        /// <summary>
        /// Integer.
        /// </summary>
        public int PositionMs { get; init; }

        public override SpotifyApiCallType ApiCall => SpotifyApiCallType.StartOrResumePlayback;

        protected override HttpRequestMethod RequestMethod => HttpRequestMethod.Put;

        protected override List<AccessScopeType> Scopes => [AccessScopeType.UserModifyPlaybackState];

        protected override string Endpoint =>
          SpotifyEndpoint.PlayerBaseEndpoint +
            "/play" +
            HttpRequestUriUtilities.GetQueryStringOrEmpty("device_id", _deviceId);

        private readonly string _deviceId = deviceId;
        private static readonly JsonSerializerOptions s_serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        private readonly struct StartOrResumeBody
        {
            public readonly struct UriOffset
            {
                public string Uri { get; init; }
            }

            public string ContextUri { get; init; }

            public UriOffset Offset { get; init; }

            public int PositionMs { get; init; }
        }

        protected override void AddBodyToRequestIfNeeded(HttpRequestMessage request)
        {
            var hasContextUri = !string.IsNullOrEmpty(ContextUri);
            var hasUris = Uris != default && Uris.Length > 0;

            if (hasUris)
                throw new NotImplementedException("StartOrResumePlayback does not yet support adding URIs to the Put request");

            if (hasContextUri)
            {
                var body = new StartOrResumeBody()
                {
                    ContextUri = ContextUri!,
                    Offset = new() { Uri = Offset.Uri ?? string.Empty },
                    PositionMs = PositionMs
                };

                var jsonBody = JsonContent.Create(body, options: s_serializerOptions);
                request.Content = jsonBody;
            }
        }
    }
}
